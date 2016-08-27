using UnityEngine;
using System.Collections;

public class Line : MonoBehaviour 
{
    public Transform rope;
    public Transform hinge;
    public Transform hook;

    SpringJoint2D joint;

    Vector2 startPosition;
    public float tutorialLength;
    private FrogPrototype frog;

	void Start ()
    {
        frog = GetComponent<FrogPrototype>();
        joint = GetComponent<SpringJoint2D>();

        joint.connectedAnchor = new Vector2(hinge.position.x, hinge.position.y + tutorialLength);
        joint.distance = Vector2.Distance(hinge.position, joint.connectedAnchor);
        joint.frequency = 5;

        startPosition = hinge.position;
        rope.position = joint.connectedAnchor;
        hook.position = rope.position;
	}

	void Update ()
    {
        //Set joint distance
        float distance = Vector2.Distance(hinge.position, joint.connectedAnchor);
        float startDistance = Vector2.Distance(startPosition, joint.connectedAnchor);

        bool isGripping = frog.leftGripScript.isOnGrip || frog.rightGripScript.isOnGrip;
        if (isGripping)
        {
            joint.distance = distance + 0.3f;
        }
        if (joint.distance >= 13f)
            joint.distance = 13f;

        joint.enabled = !isGripping;

        //Scale rope
        Vector3 scale = rope.localScale;
        scale.y = distance / startDistance;
        scale.y -= 0.29f * scale.y;
        rope.localScale = scale;

        //Rotate rope
        Vector3 angle = rope.eulerAngles;
        angle.z = Mathf.Atan2(hinge.position.y - rope.position.y, hinge.position.x - rope.position.x) * Mathf.Rad2Deg + 90;
        rope.rotation = Quaternion.Euler(angle);

	}

    public void Remove()
    {
        GetComponent<SpringJoint2D>().enabled = false;
        Destroy(hinge.gameObject);
        Destroy(rope.gameObject);
        Destroy(hook.gameObject);
        Destroy(this);
    }
}
