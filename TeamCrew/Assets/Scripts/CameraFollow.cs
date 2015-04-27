using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public float movementSpeed;
    public float zoomSpeed;

    public float maxZoom = 12;
    public float minZoom = 6;
    public float absoluteZoomValue = 8;

    private Camera cam;

    public bool absoluteZoom;

    void Start()
    {
        cam = GetComponent<Camera>();
    }


    void Update()
    {
        if (GameManager.playerOne == null || GameManager.playerTwo == null)
            return;

        FollowTopFrog();
    }

    void FollowTopFrog()
    {
        //Aquire top frogs y position
        float topFrogY = (GameManager.playerOne.position.y > GameManager.playerTwo.position.y) ? GameManager.playerOne.position.y : GameManager.playerTwo.position.y;

        //Set target position to frogs feet
        Vector3 targetPosition = transform.position;
        targetPosition.y = topFrogY - 2;
        targetPosition.x = (GameManager.playerOne.position.x + GameManager.playerTwo.position.x) / 2;


        //Move towards target
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * movementSpeed);

        //Set target size
        float targetSize = Mathf.Clamp(Mathf.Abs(GameManager.playerOne.position.y - GameManager.playerTwo.position.y), minZoom, maxZoom);
        if (absoluteZoom)
        {
            targetSize = absoluteZoomValue;
        }

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * zoomSpeed);

        //Fix Z position
        Vector3 pos = cam.transform.position;
        pos.z = (cam.orthographicSize - minZoom) / (maxZoom - minZoom) * -20 - 10;
        pos.y = Mathf.Clamp(pos.y, GameManager.LevelHeight + 8, int.MaxValue);
        cam.transform.position = pos;
    }
    public void SetAbsoluteZoom(bool state)
    {
        absoluteZoom = state;
    }
}
