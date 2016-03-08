using UnityEngine;
using System.Collections;

public enum SymbolState { NoInput, HandUp, Gripping, PullingDown}
public class TutorialSymbol : MonoBehaviour 
{
    //Publics
    public GripSide arm;
    public float movementSpeed = 3f;
    public Vector2 offset = new Vector2(1, 1);

    //Locals
    private GameObject[] children;
    private FrogPrototype frog;
    private SymbolState currentState;

    void Start()
    {
        offset = new Vector2(0.87f, 0.44f);
        currentState = SymbolState.NoInput;
        frog = transform.parent.GetComponentInChildren<FrogPrototype>();

        children = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i).gameObject;
            children[i].gameObject.SetActive(false);
            children[i].GetComponent<SpriteRenderer>().color = frog.respawnArrowColor;
        }
    }
    void Update()
    {
        if (!FollowHand())
        {
            SwitchState(SymbolState.NoInput);
            return;
        }

        if (!HandIsUp() && !Gripping())
        {
            SwitchState(SymbolState.NoInput);
            return;
        }

        if (Gripping())
        {
            if (HandIsDown())
            {
                SwitchState(SymbolState.PullingDown);
                return;
            }

            SwitchState(SymbolState.Gripping);
            return;
        }

        if (HandIsUp() && !Gripping())
        {
            SwitchState(SymbolState.HandUp);
            return;
        }

    }

    bool FollowHand()
    {
        Transform hand = (arm == GripSide.Left) ? frog.leftGripScript.transform : frog.rightGripScript.transform;
        int side = (arm == GripSide.Left) ? -1 : 1;

        Vector3 localOffset = new Vector3(Mathf.Abs(offset.x) * side, Mathf.Abs(offset.y));
        Vector3 targetPosition = hand.position + localOffset;


        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * movementSpeed);
        return hand.gameObject.activeInHierarchy;
    }
    void SwitchState(SymbolState state)
    {
        if (currentState == state)
            return;
        currentState = state;

        if (state == SymbolState.NoInput)
        {
            DeactivateAllChildren();
        }
        else
        {
            ActivateChild(state.ToString());
        }
    }
    void ActivateChild(string nameOfChild)
    {
        DeactivateAllChildren();
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].name.Contains(nameOfChild))
            {
                children[i].SetActive(true);
            }
        }
    }
    void DeactivateAllChildren()
    {
        for (int i = 0; i < children.Length; i++)
        {
            children[i].SetActive(false);
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
        Vector2 direction = GameManager.GetThumbStick(stick, frog.player);

        return direction.y > 0;
    }
    bool HandIsDown()
    {
        XboxThumbStick stick = (arm == GripSide.Left) ? XboxThumbStick.Left : XboxThumbStick.Right;
        Vector2 direction = GameManager.GetThumbStick(stick, frog.player);

        return direction.y < 0;
    }
}
