using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour
{
    private Transform mainCamera;
    private Vector2 neutralParallaxPosition;
    private static float levelHeight;
    private static float stupidHeight = 67.2f; //the normal height when 3 blocks + tutorial... 


    public float cameraToRockLenghts;
    public float maxParallax;

    //-Settings-
    public bool debugTransparent = false;
    public float ManualProgress = 0;
    private float lastManualProgress = 0;
    public bool IsManual = false;
    private bool lastIsManual = false;

    //Distributing local settings to
    private static float manualProgress = 0;
    private static bool isManual = false;

	void Start ()
    {
        //-Init-
        neutralParallaxPosition = transform.position;
        mainCamera = Camera.main.transform;
        
        //-Debugging cases-
        if(debugTransparent)
            transform.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, .5f);
	}

    bool IsUpdateRedundant()
    {
        if (isManual) return false;

        if (levelHeight == 0) return true; // There is no level

        return false;
    }

    void CheckChangedSettings()
    {
        if (IsManual != lastIsManual)
        {
            lastIsManual = IsManual;
            isManual = IsManual;
        }
        if (ManualProgress != lastManualProgress)
        {
            lastManualProgress = ManualProgress;
            manualProgress = ManualProgress;
        }
    }

	void Update ()
    {
        CheckChangedSettings(); //This function is stupid but I don't know of any easier alternatives
        if (IsUpdateRedundant() == true) return;
        SetCamera(GetParallaxProgress());
        //Debug.Log("Level Height: "+levelHeight);
        //Debug.Log(mainCamera.position);
	}

    float GetParallaxProgress()
    {
        float cameraProgress = 0;
        if (isManual)
        {
            cameraProgress = manualProgress;
        }
        else
        {
            //Inside the parenthesis levelHeight is compensation for level top being at y=0;
            cameraProgress = (mainCamera.position.y + levelHeight) / levelHeight;
        }
        return cameraProgress * maxParallax;// * (levelHeight / stupidHeight);
    }
    void SetCamera(float parallaxProgress)
    {
        if (levelHeight == 0)
        {
            Debug.Log("Error: Division with 0");
            return;
        }

        float xParallax = (mainCamera.position.x / levelHeight) * maxParallax;
        float yParallax = parallaxProgress;
        Vector2 targetPos = new Vector2( xParallax, yParallax);
        transform.position = targetPos + neutralParallaxPosition+ new Vector2(0, -levelHeight *.85f); //this +13 might be changed to * .85, we will see how it works when level height exists.
    }

    public static void SetLevelHeight(float inputHeight)
    {
        levelHeight = Mathf.Abs(inputHeight);
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