using UnityEngine;
using System.Collections;

public class SpinningWheel : MonoBehaviour 
{
    [Range(0, 500)]
    public float spinSpeed = 25f;
    public bool spinRight = true;

    private Rigidbody2D body;
    private HingeJoint2D joint;
	void Start () 
	{
        body = GetComponent<Rigidbody2D>();
        joint = GetComponent<HingeJoint2D>();

        if (joint.connectedBody == null)
        {
            joint.connectedAnchor = transform.position;
        }

        //for (int i = 0; i < transform.childCount; i++)
        //{
        //    Debug.Log("Rawr");
        //    MovingGrip g = transform.GetChild(i).GetComponent<MovingGrip>();

        //    if (g)
        //    {
        //        g.GetComponent<HingeJoint2D>().connectedAnchor = g.transform.localPosition;
        //    }
        //}
	}

    void Update()
    {
        if (joint.connectedBody != null)
            joint.connectedAnchor = transform.parent.localPosition;
    }

	void FixedUpdate () 
	{
        int dir = (spinRight) ? -1 : 1;
        body.angularVelocity = spinSpeed * dir;
	}
}
