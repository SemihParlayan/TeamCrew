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
    }

    void FollowFrogs()
    {
        //Set target position to frogs feet
        Vector3 topfrogPosition = GameManager.GetTopFrog().position;
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
        //pos.y = Mathf.Clamp(pos.y, GameManager.LevelHeight + 7, int.MaxValue);
        cam.transform.position = pos;
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
