using UnityEngine;
using System.Collections;

public class CenterOfMass : MonoBehaviour 
{
    public Vector2 centerOfMass;
    private Rigidbody2D body;

	void Start () 
    {
        body = GetComponent<Rigidbody2D>();
        //body.centerOfMass = Vector2.zero;
	}

    void OnDrawGizmos()
    {
        Rigidbody2D body = GetComponent<Rigidbody2D>();
        if (body)
        {
            body.centerOfMass = centerOfMass;
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(body.worldCenterOfMass, 0.075f);
        }   
    }
}
