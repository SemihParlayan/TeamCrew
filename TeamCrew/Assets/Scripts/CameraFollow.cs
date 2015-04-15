using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour 
{
    public float smooth = 1.0f;
    public float movementSpeed;
    public float zoomSpeed;

    public float maxZoom = 12;
    public float minZoom = 6;

    private Camera cam;
    private Transform frogOne;
    private Transform frogTwo;

    public bool absoluteZoom;
	void Start () 
    {
        cam = GetComponent<Camera>();
        frogOne = GameObject.FindWithTag("PlayerOne").transform;
        frogTwo = GameObject.FindWithTag("PlayerTwo").transform;

        cam.orthographicSize = (maxZoom + minZoom) / 2;
	}


	void Update () 
    {
        if (frogOne == null || frogTwo == null)
            return;

        FollowTopFrog();
	}

    void FollowTopFrog()
    {
        //Aquire top frogs y position
        float topFrogY = (frogOne.position.y > frogTwo.position.y) ? frogOne.position.y : frogTwo.position.y;

        //Set target position to frogs feet
        Vector3 targetPosition = transform.position;
        targetPosition.y = topFrogY - 2;


        //Move towards target
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * movementSpeed);

        //Set target size
        float targetSize = Mathf.Clamp(Mathf.Abs(frogOne.position.y - frogTwo.position.y), minZoom, maxZoom);
        if (absoluteZoom)
        {
            targetSize = minZoom;
        }
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * zoomSpeed);


        //float targetY = 0;
        //if (cam.orthographicSize >= maxZoom - 1)
        //{
        //    targetY = (frogOne.position.y > frogTwo.position.y) ? frogOne.position.y : frogTwo.position.y;
        //    targetY -= maxZoom / 2;
        //}
        //else
        //{
        //    targetY = (frogOne.position.y + frogTwo.position.y) / 2;
        //}
        //float currentY = transform.position.y;


        //transform.position += new Vector3(0, (targetY - currentY) * smooth, 0);



        //float frogOneY = frogOne.position.y;
        //float frogTwoY = frogTwo.position.y;

        //float targetSize = Mathf.Clamp(Mathf.Abs(frogOneY - frogTwoY), minZoom, maxZoom);

        //cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * 2);
    }

    public void SetAbsoluteZoom(bool state)
    {
        absoluteZoom = state;
    }
}
