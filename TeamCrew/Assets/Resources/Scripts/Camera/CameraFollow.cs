using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public float movementSpeed;
    public float zoomSpeed;

    public float maxZoom = 12;
    public float minZoom = 6;
    public float finalStretchZoomValue = 11;

    private Camera cam;

    public bool absoluteFinalStretchZoom;

    public float maxYReached;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void OnEnable()
    {
        maxYReached = 0;
    }

    void Update()
    {
        FollowFrogs();
    }

    public Transform bottomfrog;
    public Transform topfrog;
    void FollowFrogs()
    {
        //Set target position to frogs feet
        topfrog = GameManager.GetTopFrog();
        bottomfrog = GameManager.GetBottomFrog();
        if (!topfrog)
            return;

        Vector3 topfrogPosition = topfrog.position;
        if (topfrogPosition == Vector3.zero)
        {
            return;
        }

        Vector3 targetPosition = transform.position;
        targetPosition.y = topfrogPosition.y - 1.5f;
        targetPosition.x = GetMedianXPositionOfFrogs();

        //Move towards target
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * movementSpeed);

        //Set maximum Y reached
        //if (transform.position.y > maxYReached)
        //    maxYReached = transform.position.y;


        //Lock camera from going downwards
        bool lockCamera = true;
        for (int i = 0; i < GameManager.players.Length; i++)
        {
            if (GameManager.players[i] == null)
                continue;

            FrogPrototype frog = GameManager.players[i].GetComponent<FrogPrototype>();
            if (frog.leftGripScript.isOnGrip || frog.rightGripScript.isOnGrip)
            {
                lockCamera = false;
                break;
            }
        }
        if (lockCamera)
        {
            if (transform.position.y < maxYReached - 2)
            {
                Vector3 p = transform.position;
                p.y = targetPosition.y = maxYReached - 2;

                transform.position = p;
            }
        }
        else
        {
            maxYReached = transform.position.y;
        }

        //Set target size
        float targetSize = (minZoom + maxZoom) / 2;
        if (bottomfrog != topfrog)
        {
            float distance = Vector3.Distance(topfrog.position, bottomfrog.position);
            float value = Mathf.Clamp(distance / 10, 0, 1);

            targetSize = (maxZoom - minZoom) * value;
            targetSize += minZoom;

        }

        if (absoluteFinalStretchZoom)
        {
            targetSize = finalStretchZoomValue;
        }

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * zoomSpeed);

        //Fix Z position
        Vector3 pos = cam.transform.position;
        pos.z = (cam.orthographicSize - minZoom) / (maxZoom - minZoom) * -5 - 10;
        pos.y = Mathf.Clamp(pos.y, -42 + cam.orthographicSize, int.MaxValue);
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
}
