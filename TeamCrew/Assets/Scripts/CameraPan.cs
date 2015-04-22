using UnityEngine;
using System.Collections;

public class CameraPan : MonoBehaviour 
{
    public float movementSpeed = 10;

	void Start () 
    {
	}
	
	void Update () 
    {
        Vector3 velocity = -Vector3.up * Time.deltaTime;
        velocity *= Mathf.Abs(transform.position.y - GameManager.LevelHeight);
        velocity *= movementSpeed;

        transform.position += velocity;
	}

    public bool Complete()
    {
        bool complete = (transform.position.y <= GameManager.LevelHeight + 5);
        
        if (complete)
        {
            this.enabled = false;
        }
        return complete;
    }
}
