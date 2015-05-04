using UnityEngine;
using System.Collections;

public class Line : MonoBehaviour 
{
    public Transform rope;
    public Transform hinge;
    public Transform hook;

    SpringJoint2D joint;
    Rigidbody2D body;

    float maxY;
    Vector2 startPosition;
    public float tutorialLength;

	void Start ()
    {
        body = GetComponent<Rigidbody2D>();
        joint = GetComponent<SpringJoint2D>();

        joint.connectedAnchor = new Vector2(hinge.position.x, hinge.position.y + tutorialLength);
        joint.distance = maxY = Vector2.Distance(hinge.position, joint.connectedAnchor);

        startPosition = hinge.position;
        rope.position = joint.connectedAnchor;
        hook.position = rope.position;
	}

	void Update ()
    {
        //Set joint distance
        float distance = Vector2.Distance(hinge.position, joint.connectedAnchor);
        float startDistance = Vector2.Distance(startPosition, joint.connectedAnchor);

        if (distance < maxY)
        {
            maxY = distance;
            joint.distance = maxY;
        }

        //Scale rope
        Vector3 scale = rope.localScale;
        scale.y = distance / startDistance;
        scale.y -= 0.23f * scale.y;
        rope.localScale = scale;

        //Rotate rope
        Vector3 angle = rope.eulerAngles;
        angle.z = Mathf.Atan2(hinge.position.y - rope.position.y, hinge.position.x - rope.position.x) * Mathf.Rad2Deg + 90;
        rope.rotation = Quaternion.Euler(angle);

        //Remove rope;
        if (joint.distance < 2.5f)
        {
            Remove();
        }
	}

    public void Remove()
    {
        GetComponent<SpringJoint2D>().enabled = false;
        Destroy(hinge.gameObject);
        Destroy(rope.gameObject);
        Destroy(this);
    }
}
