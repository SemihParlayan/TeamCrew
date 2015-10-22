using UnityEngine;
using System.Collections;

public class MoveToward : MonoBehaviour
{

    /*variables:
     * Target - position or object with a position
     * TargetOffset
     * Speed
     * Mode:
     *  Faster when approximating position
     *  Slower when approximating position
     *  Constant speed
     *  Teleport
     *  
     * */

    public Transform TargetTransform;
    public Vector3 position;

	void Start ()
    {
        position = transform.position;
	}

	void Update ()
    {
        //Teleport
        //position = target.position;

        //move towards
        
        position += 0.05f * (Camera.main.ScreenToWorldPoint(Input.mousePosition) - position);

        position.z = 0;
        transform.position = position;
        //Todo add delta time

	}
}
