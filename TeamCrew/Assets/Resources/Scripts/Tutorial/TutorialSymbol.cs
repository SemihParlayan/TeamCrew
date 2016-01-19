using UnityEngine;
using System.Collections;

public enum SymbolState { NoInput, HandUp, Gripping}
public class TutorialSymbol : MonoBehaviour 
{
    //Publics
    public GripSide arm;
    public float movementSpeed = 3f;
    public Vector2 offset = new Vector2(1, 1);

    //Locals
    private GameObject[] children;
    private SymbolState currentState;
    private FrogPrototype frog;
    private SpriteRenderer renderer;


    void Start()
    {
        currentState = SymbolState.NoInput;
        frog = transform.parent.GetComponentInChildren<FrogPrototype>();
        renderer = GetComponent<SpriteRenderer>();

        children = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i).gameObject;
            children[i].gameObject.SetActive(false);
        }
    }
    void Update()
    {
        FollowHand();

        if (!HandIsUp() && !Gripping())
        {
            SwitchState(SymbolState.NoInput);
            return;
        }

        if (Gripping())
        {
            SwitchState(SymbolState.Gripping);
            return;
        }

        if (HandIsUp() && !Gripping())
        {
            SwitchState(SymbolState.HandUp);
            return;
        }
    }

    void FollowHand()
    {
        Transform hand = (arm == GripSide.Left) ? frog.leftGripScript.transform : frog.rightGripScript.transform;
        int side = (arm == GripSide.Left) ? -1 : 1;
        Vector3 localOffset = new Vector3(Mathf.Abs(offset.x) * side, Mathf.Abs(offset.y));
        Vector3 targetPosition = hand.position + localOffset;


        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * movementSpeed);
    }
    void SwitchState(SymbolState state)
    {
        if (currentState == state)
            return;
        currentState = state;

        ActivateChild(state.ToString());
    }
    void ActivateChild(string nameOfChild)
    {
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].name.Contains(nameOfChild))
            {
                children[i].SetActive(true);
            }
            else
            {
                children[i].SetActive(false);
            }
        }
    }
    bool Gripping()
    {
        if (arm == GripSide.Left)
        {
            return frog.leftGripScript.isOnGrip;
        }
        else
        {
            return frog.rightGripScript.isOnGrip;
        }
    }
    bool HandIsUp()
    {
        XboxThumbStick stick = (arm == GripSide.Left) ? XboxThumbStick.Left : XboxThumbStick.Right;
        Vector2 direction = GameManager.GetThumbStick(stick);

        return direction.y > 0;
    }
}
