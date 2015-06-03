using UnityEngine;
using System.Collections;

public class TopFrog : MonoBehaviour 
{
    public string player;

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
        float angle = GetAngleFromInput(input);

        if ((input.x != 0 || input.y != 0)) //If hand is moving and not on a grip
        {
            //Move towards joystick Direction
            Vector3 dir = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle));

            if (!GameManager.DigitalInput)
            {
                dir.x *= Mathf.Abs(input.x * 1.3f);
                dir.y *= Mathf.Abs(input.y * 1.3f);
            }

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
    float GetAngleFromInput(Vector3 input)
    {
        float angle = Mathf.Rad2Deg * (float)Mathf.Atan2(input.x, input.y);

        //Convert angle to stay between 0-360 degrees
        if (angle < 0)
        {
            angle = 180 + (180 - Mathf.Abs(angle));
        }

        if (GameManager.DigitalInput)
        {
            //Set angle to snap each 45 degree
            float i = (int)(angle / 45.0f);
            angle = (45 * i);
        }

        angle *= Mathf.Deg2Rad;

        return angle;
    }
    public void RemoveChampagne()
    {
        champagne.transform.parent.gameObject.SetActive(false);
        neutral.SetActive(true);
    }
}
