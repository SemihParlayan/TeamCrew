using UnityEngine;
using System.Collections;

public class SpinningWheel : MonoBehaviour 
{
    [Range(0, 500)]
    public float spinSpeed = 75f;
    public bool spinRight = true;

    protected Rigidbody2D body;

	void Start () 
	{
        body = GetComponent<Rigidbody2D>();
        HingeJoint2D joint = GetComponent<HingeJoint2D>();

        if (joint.connectedBody == null)
        {
            joint.connectedAnchor = transform.position;
        }

        BaseStart();
	}
    protected virtual void BaseStart()
    {

    }

    void Update()
    {
        BaseUpdate();
    }
    protected virtual void BaseUpdate()
    {

    }

	void FixedUpdate () 
	{
        BaseFixedUpdate();
	}
    protected virtual void BaseFixedUpdate()
    {
        int dir = (spinRight) ? -1 : 1;
        body.angularVelocity = spinSpeed * dir;
    }
}
