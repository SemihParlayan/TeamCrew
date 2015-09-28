using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public float movementSpeed;
    public float zoomSpeed;

    public float maxZoom = 12;
    public float minZoom = 6;
    public float absoluteZoomValue = 8;
    public float finalStretchZoomValue = 11;

    private Camera cam;

    public bool absoluteZoom;
    public bool absoluteFinalStretchZoom;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        FollowFrogs();
        //if (GameManager.players[0] && GameManager.players[1])
        //{
        //    FollowTopFrog();
        //}
        //else if (GameManager.players[0] == null && GameManager.players[1])
        //{
        //    FollowFrog(GameManager.players[1]);
        //}
        //else if (GameManager.players[0] && GameManager.players[1] == null)
        //{
        //    FollowFrog(GameManager.players[0]);
        //}
    }

    void FollowFrogs()
    {
        //Set target position to frogs feet
        Vector3 topfrogPosition = GetTopfrogPosition();
        if (topfrogPosition == Vector3.zero)
        {
            return;
        }

        Vector3 targetPosition = transform.position;
        targetPosition.y = topfrogPosition.y - 3;
        targetPosition.x = GetMedianXPositionOfFrogs();

        //Move towards target
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * movementSpeed);

        //Set target size
        float targetSize = absoluteZoomValue;

        if (absoluteFinalStretchZoom)
        {
            targetSize = finalStretchZoomValue;
        }

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * zoomSpeed);

        //Fix Z position
        Vector3 pos = cam.transform.position;
        pos.z = (cam.orthographicSize - minZoom) / (maxZoom - minZoom) * -5 - 10;
        pos.y = Mathf.Clamp(pos.y, GameManager.LevelHeight + 7, int.MaxValue);
        cam.transform.position = pos;
    }
    void FollowFrog(Transform frog)
    {
        //Set target position to frogs feet
        Vector3 targetPosition = transform.position;
        targetPosition.y = frog.position.y - 2;
        targetPosition.x = frog.position.x;

        //Move towards target
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * movementSpeed);

        //Set target size
        float targetSize = absoluteZoomValue;

        if (absoluteFinalStretchZoom)
        {
            targetSize = finalStretchZoomValue;
        }

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * zoomSpeed);

        //Fix Z position
        Vector3 pos = cam.transform.position;
        pos.z = (cam.orthographicSize - minZoom) / (maxZoom - minZoom) * -5 - 10;
        pos.y = Mathf.Clamp(pos.y, GameManager.LevelHeight + 7, int.MaxValue);
        cam.transform.position = pos;
    }
    void FollowTopFrog()
    {
        //Aquire top frogs y position
        Vector3 topFrogPosition = (GameManager.players[0].position.y > GameManager.players[1].position.y) ? GameManager.players[0].position : GameManager.players[1].position;
        float topFrogY = topFrogPosition.y;

        //Set target position to frogs feet
        Vector3 targetPosition = transform.position;
        targetPosition.y = topFrogY - 2;
        if (!absoluteZoom)
            targetPosition.x = (GameManager.players[0].position.x + GameManager.players[1].position.x) / 2;
        else
            targetPosition.x = topFrogPosition.x;


        //Move towards target
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * movementSpeed);

        //Set target size
        float targetSize = Mathf.Clamp(Mathf.Abs(GameManager.players[0].position.y - GameManager.players[1].position.y), minZoom, maxZoom);

        if (absoluteFinalStretchZoom)
        {
            targetSize = finalStretchZoomValue;
        }
        else if (absoluteZoom)
        {
            targetSize = absoluteZoomValue;
        }

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * zoomSpeed);

        //Fix Z position
        Vector3 pos = cam.transform.position;
        pos.z = (cam.orthographicSize - minZoom) / (maxZoom - minZoom) * -5 - 10;
        pos.y = Mathf.Clamp(pos.y, GameManager.LevelHeight + 7, int.MaxValue);
        cam.transform.position = pos;
    }
    Vector3 GetTopfrogPosition()
    {
        int index = -1;
        float topY = -float.MaxValue;

        for (int i = 0; i < GameManager.players.Length; i++)
        {
            if (GameManager.players[i] != null)
            {
                float frogY = GameManager.players[i].position.y;
                if (frogY > topY)
                {
                    index = i;
                    topY = frogY;
                }
            }
        }

        if (index != -1)
        {
            return GameManager.players[index].position;
        }
        else
        {
            return Vector3.zero;
        }
    }
    float GetMedianXPositionOfFrogs()
    {
        int frogCount = 0;
        float totalX = 0;
        for (int i = 0; i < GameManager.players.Length; i++)
        {
            if (GameManager.players[i] != null)
            {
                frogCount++;
                totalX += GameManager.players[i].position.x;
            }
        }

        return totalX / frogCount;
    }
    public void SetAbsoluteZoom(bool state)
    {
        absoluteZoom = state;
    }
}
