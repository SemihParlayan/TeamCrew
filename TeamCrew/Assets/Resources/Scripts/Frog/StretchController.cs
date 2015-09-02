using UnityEngine;
using System.Collections;

public class StretchController : MonoBehaviour 
{
    //A limit that decides when to release hands
    [Range(1, 10)]
    public float breakLimit = 4.5f;

    //Reference to which frog to controll
    private FrogPrototype frog;

    //The distance between both hands
    private float distanceBetweenHands;

	void Start () 
    {
        frog = GetComponent<FrogPrototype>();

        if (frog == null)
        {
            Debug.LogError("StretchController: Could not find FrogPrototype!");
        }
	}

	void Update () 
    {
        //Calculate distance between hands
        Vector2 leftHandPos = frog.leftHand.position;
        Vector2 rightHandPos = frog.rightHand.position;

        distanceBetweenHands = Vector2.Distance(leftHandPos, rightHandPos);


        //Release a hand if necessary
        if (distanceBetweenHands > breakLimit)
        {
            if (frog.leftGripScript.isGripping && frog.rightGripScript.isGripping)
            {
                HandGrip hand = (frog.leftGripScript.lastGripTime > frog.rightGripScript.lastGripTime) ? frog.rightGripScript : frog.leftGripScript;
                hand.ReleaseGrip(1f);
            }
        }
	}
}
