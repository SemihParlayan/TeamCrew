using UnityEngine;
using System.Collections;

public class TopFrog : MonoBehaviour 
{
    public string player;

    public GameObject taunt;
    public GameObject neutral;
    public ParticleSystem champagne;
    private float timer = 0;

    public Transform leftHand;
    public Transform leftHandNeutral;
    public Transform leftHandOrigin;

    public Transform rightHand;
    public Transform rightHandNeutral;
    public Transform rightHandOrigin;

    private Rigidbody2D leftBody, rightBody;

    void OnBecameVisible()
    {
        enabled = true;
    }

    void OnBecameInvisible()
    {
        enabled = false;
    }

    void Start()
    {
        leftBody = leftHand.GetComponent<Rigidbody2D>();
        rightBody = rightHand.GetComponent<Rigidbody2D>();
    }

	void Update () 
    {
        if (!champagne.gameObject.activeInHierarchy)
        {
            timer += Time.deltaTime;
            if (timer >= 4)
            {
                champagne.gameObject.SetActive(true);
            }
        }
        ControlHand(GameManager.GetInput(player + "HL", player + "VL"), leftBody, leftHand, leftHandNeutral, leftHandOrigin);
        ControlHand(GameManager.GetInput(player + "HR", player + "VR"), rightBody, rightHand, rightHandNeutral, rightHandOrigin);
	}

    void ControlHand(Vector3 input, Rigidbody2D body, Transform hand, Transform handNeutral, Transform handOrigin)
    {
        float angle = Mathf.Rad2Deg * (float)Mathf.Atan2(input.x, input.y);
        if (angle < 0)
        {
            angle = 180 + (180 - Mathf.Abs(angle));
        }
        float i = (int)(angle / 45.0f);
        angle = (45 * i) * Mathf.Deg2Rad;

        if ((input.x != 0 || input.y != 0)) //If hand is moving and not on a grip
        {
            //Move towards joystick Direction
            Vector3 dir = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle));
            Vector3 targetPosition = handOrigin.position + dir * 2.0f;
            body.velocity = (targetPosition - hand.position) * 10f;
        }
        else //If hand is not moving and not on grip
        {
            //Move towards neutral position
            Vector3 targetPosition = handNeutral.position;
            body.velocity = (targetPosition - hand.position) * 10f;
        }
    }

    public void RemoveChampagne()
    {
        Debug.Log("Remove champ");
        champagne.transform.parent.gameObject.SetActive(false);
        taunt.SetActive(false);
        neutral.SetActive(true);
    }
}
