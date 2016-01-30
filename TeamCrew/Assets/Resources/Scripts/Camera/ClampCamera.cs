using UnityEngine;
using System.Collections;

public class ClampCamera : MonoBehaviour 
{
    private Transform cameraTransform;

    void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (cameraTransform.position.y + Camera.main.orthographicSize / 2 <= -44)
        {
            cameraTransform.position = new Vector3(cameraTransform.position.x, -44, cameraTransform.position.z);
        }
    }
}
