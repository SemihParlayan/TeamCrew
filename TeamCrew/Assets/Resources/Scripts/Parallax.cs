using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour
{
    //Static
    public static float LevelHeight;

    //Data
    public bool active;
    [Range(-1, 1)]
    public float minmumPercentage;
    [Range(-1, 1)]
    public float maximumPercentage;

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
                transform.position = new Vector3(transform.position.x, LevelHeight - (LevelHeight * minmumPercentage), transform.position.z);
            }

            return;
        }

        //We are now active, update the parallaxes to correct height
        UpdateParallaxes();
    }

    private void UpdateParallaxes()
    {
        float minY = LevelHeight - (LevelHeight * minmumPercentage);
        float maxY = LevelHeight - (LevelHeight * maximumPercentage);
        float diff = Mathf.Abs(minY) - Mathf.Abs(maxY);
        float cameraNormal = 1 - ((cam.transform.position.y - cam.orthographicSize / 2) / LevelHeight);
        Vector3 targetpos = transform.position;

        targetpos.y = minY + (diff * cameraNormal);
        Debug.Log(cameraNormal);

        transform.position = targetpos;
    }

}