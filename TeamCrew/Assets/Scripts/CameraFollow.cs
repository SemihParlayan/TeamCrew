using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour 
{
    public Transform frogOne;
    public Transform frogTwo;
    public float smooth = 1.0f;

    public float maxZoom = 12;
    public float minZoom = 6;

    private Camera cam;
	void Start () 
    {
        cam = GetComponent<Camera>();

        cam.orthographicSize = (maxZoom + minZoom) / 2;
	}


	void Update () 
    {
        if (frogOne == null || frogTwo == null)
            return;

        float targetY = 0;
        if (cam.orthographicSize >= maxZoom - 1)
        {
            targetY = (frogOne.position.y > frogTwo.position.y) ? frogOne.position.y : frogTwo.position.y;
            targetY -= maxZoom / 2;
        }
        else
        {
            targetY = (frogOne.position.y + frogTwo.position.y) / 2;
        }
        float currentY = transform.position.y;


        transform.position += new Vector3(0, (targetY - currentY) * smooth, 0);



        float frogOneY = frogOne.position.y;
        float frogTwoY = frogTwo.position.y;

        float targetSize = Mathf.Clamp(Mathf.Abs(frogOneY - frogTwoY), minZoom, maxZoom);

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * 2);
        Vector3 pos = cam.transform.position;
        pos.z = (cam.orthographicSize - minZoom) / (maxZoom - minZoom) * -20 - 10;
        cam.transform.position = pos;
	}
}
