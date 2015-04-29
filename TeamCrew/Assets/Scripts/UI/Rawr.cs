using UnityEngine;
using System.Collections;

public class Rawr : MonoBehaviour 
{
    private Rigidbody2D body;

	void Start () 
    {
        body = GetComponent<Rigidbody2D>();
        body.centerOfMass = Vector2.zero;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDrawGizmos()
    {
        Rigidbody2D body = GetComponent<Rigidbody2D>();
        if (body)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(body.worldCenterOfMass, 0.1f);
        }   
    }
}
