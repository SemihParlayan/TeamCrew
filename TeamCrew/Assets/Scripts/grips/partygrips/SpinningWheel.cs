using UnityEngine;
using System.Collections;

public class SpinningWheel : MonoBehaviour 
{
    public bool spinRight = true;
    [Range(0, 500)]
    public float spinSpeed = 25f;

    private Rigidbody2D body;
	void Start () 
	{
        body = GetComponent<Rigidbody2D>();
        GetComponent<HingeJoint2D>().connectedAnchor = transform.position;
	}

	void FixedUpdate () 
	{
        int dir = (spinRight) ? -1 : 1;
        body.angularVelocity = spinSpeed * dir;
	}
}
