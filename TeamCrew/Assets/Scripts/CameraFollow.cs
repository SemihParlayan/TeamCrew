using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour 
{
    public Transform frogOne;
    public Transform frogTwo;
    public float smooth = 1.0f;

	void Start () 
    {
	
	}


	void Update () 
    {
        if (frogOne == null || frogTwo == null)
            return;

        float targetY = (frogOne.position.y > frogTwo.position.y) ? frogOne.position.y : frogTwo.position.y;
        float currentY = transform.position.y;


        transform.position += new Vector3(0, (targetY - currentY) * smooth, 0);
	}
}
