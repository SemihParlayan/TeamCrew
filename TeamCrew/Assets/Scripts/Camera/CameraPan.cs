using UnityEngine;
using System.Collections;

public class CameraPan : MonoBehaviour 
{
    public float movementSpeed = 10;
    private bool halfway;

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

    public bool Halfway()
    {
        if (!halfway)
            return halfway = ((transform.position.y <= GameManager.LevelHeight / 2) && !halfway);
        else
            return false;
    }
    public bool Complete()
    {
        bool complete = (transform.position.y <= GameManager.LevelHeight + 7);
        
        if (complete)
        {
            this.enabled = false;
            halfway = false;
        }
        return complete;
    }
}
