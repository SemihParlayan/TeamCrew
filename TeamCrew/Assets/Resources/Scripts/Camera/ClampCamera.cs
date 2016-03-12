using UnityEngine;
using System.Collections;

public class ClampCamera : MonoBehaviour 
{
    private Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        if (cam.transform.position.y <= 4)
        {
            cam.transform.position = new Vector3(cam.transform.position.x, 4, cam.transform.position.z);
        }
    }
}
