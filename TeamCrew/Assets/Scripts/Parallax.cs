using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour
{
    public float cameraToRockLenghts;
    public float maxParallax;

    private Vector2 origin;
    private Camera cam;
    private float levelHeight;
    

    /* For when it was not in the camera
     * public float distance;
     */

	void Start ()
    {
        //maxParallax = 235;
        levelHeight = Mathf.Abs(GameManager.LevelHeight);
        origin = transform.position;
        origin -= new Vector2(0, levelHeight);
        
        cam = Camera.main;
        
	}

	void Update ()
    {

        Vector2 camPos = cam.transform.position;
        float normClimbDist = (camPos.y + levelHeight) / levelHeight;

        float paralPosY = normClimbDist * maxParallax;
        //Vector2 relOrig = origin - camPos;
        
        //// Fixing y pos so that the parallax is at max parralax state at the top
        //relOrig.y = paralPosY;
        Vector2 targetPos = origin;
        targetPos.x = camPos.x;
        targetPos.y = paralPosY;

        // The scalar based on distance
        float relDist = 1 / cameraToRockLenghts;

        //transform.position = relDist * relOrig + origin ;
        transform.position = targetPos + new Vector2(0, -levelHeight + 16);
	}
}
