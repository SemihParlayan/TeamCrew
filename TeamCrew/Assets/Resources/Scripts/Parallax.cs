using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour
{
    //Static
    public static float LevelHeight;

    //Data
    public bool active;

    public float startOffset;
    public float endOffset;

    //Components

    //References
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (!active)
        {
            //Set parallaxes to active
            if (LevelHeight != 0)
            {
                active = true;
            }

            return;
        }

        //We are now active, update the parallaxes to correct height
        UpdateParallaxes();
    }

    public float delta;
    public float cameraNormal;
    public float cameraOffset;
    public float yPos;
    public float x;

    private void UpdateParallaxes()
    {
        cameraNormal = 1 - ((cam.transform.position.y - cam.orthographicSize / 2) / LevelHeight);

        float biggestNumber = (startOffset > endOffset) ? startOffset : endOffset;
        float lowestNumber = (startOffset == biggestNumber) ? endOffset : startOffset;
        delta = biggestNumber - lowestNumber;

        x = cameraNormal * delta;
        cameraOffset = biggestNumber - x;

        yPos = cam.transform.position.y + cameraOffset;

        Vector3 pos = transform.position;
        pos.y = yPos;
        transform.position = pos;
    }

}