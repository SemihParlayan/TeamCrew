using UnityEngine;
using System.Collections;

public class GripMagnet : MonoBehaviour
{
    public Vector3 magnetDir;
    public Grip gripScript;
    private Transform grip;

	void Start () 
    {
	
	}

	void Update () 
    {
        magnetDir = Vector3.zero;
        if (grip && Input.GetButton(gripScript.axis))
        {
            magnetDir = grip.position - transform.position;
            magnetDir.Normalize();
            magnetDir.y = 0;
        }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Grip")
        {
            if (grip != null)
            {
                float gripDist = Vector2.Distance(grip.position, transform.position);
                float otherDist = Vector2.Distance(other.transform.position, transform.position);
                
                if (otherDist < gripDist)
                {
                    grip = other.transform;
                }
            }
            else
            {
                grip = other.transform;
            }
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Grip")
        {
            grip = null;
        }
    }
}
