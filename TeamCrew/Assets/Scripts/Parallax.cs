using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour
{
    private Transform mainCamera;
    private Vector2 neutralParallaxPosition;
    private float levelHeight;

    public float cameraToRockLenghts;
    public float maxParallax;

    //-Settings-
    public bool debugTransparent = false;


	void Start ()
    {
        //-Init-
        neutralParallaxPosition = transform.position;
        mainCamera = Camera.main.transform;
        
        //-Debugging cases-
        if(debugTransparent)
            transform.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, .5f);
	}

	void Update ()
    {
        if (isUpdateRedundant()) return;

        //calculate next position
        Vector3 nextPosition;
        levelHeight = Mathf.Abs(GameManager.LevelHeight); //shouldn't be in update :(
        float climbingProgress = (mainCamera.position.y + levelHeight) / levelHeight;
        float parallaxProgress = climbingProgress * maxParallax; 


        //set position
        
        //// Fixing y pos so that the parallax is at max parralax state at the top
        //relOrig.y = paralPosY;
        Vector2 targetPos = new Vector2(
            (mainCamera.position.x / levelHeight) * maxParallax,
            parallaxProgress
        );
        

        // The scalar based on distance
        //float relDist = 1 / mainCameraToRockLenghts; //Never used

        //transform.position = relDist * relOrig + origin ;
        nextPosition = targetPos + neutralParallaxPosition + new Vector2(0, -levelHeight + 10);

        transform.position = nextPosition;
	}
    
    bool isUpdateRedundant()
    {
        if (GameManager.LevelHeight == 0)
            return true;
        else
            return false;
    }

    void OnBecameVisible()
    {
        enabled = true;

    }

    void OnBecameInvisible()
    {
        enabled = false;
    }
    void Exit()
    {
        
    }
}
