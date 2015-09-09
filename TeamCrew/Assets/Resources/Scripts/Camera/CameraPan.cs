using UnityEngine;
using System.Collections;

public class CameraPan : MonoBehaviour 
{
    public float movementSpeed = 10;
    private bool halfway;
	
	void Update () 
    {
        //Vector3 velocity = -Vector3.up * Time.deltaTime;
        //velocity *= Mathf.Abs(transform.position.y - GameManager.LevelHeight);
        //velocity *= movementSpeed;

        //transform.position += velocity;

        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, GameManager.LevelHeight, transform.position.z), Time.deltaTime * movementSpeed);
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
        bool complete = (transform.position.y <= GameManager.LevelHeight + 4.5f);
        
        if (complete)
        {
            this.enabled = false;
            halfway = false;
        }
        return complete;
    }
}
