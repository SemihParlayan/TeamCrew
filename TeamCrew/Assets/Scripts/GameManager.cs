using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static float LevelHeight;

    public LevelGeneration generatorScript;
    public Respawn respawnScript;
    public CameraFollow cameraFollowScript;
    public CameraFollowTerrain terrainScript;
    public CameraPan cameraPanScript;
    public MainMenu mainMenuScript;

    private Vector3 cameraDefaultPosition;
    private Transform cameraTransform;
    private bool gameStarted;

    public LayerMask mask;

	void Start ()
    {
        if (generatorScript == null)
            Debug.LogError("Attach a generator script to GameManager.cs!");

        if (respawnScript == null)
            Debug.LogError("Attach a respawn script to GameManager.cs!");

        if (cameraFollowScript == null)
            Debug.LogError("Attach a camera follow script to GameManager.cs!");

        if (cameraPanScript == null)
            Debug.LogError("Attach a camera pan script to GameManager.cs!");

        if (terrainScript == null)
            Debug.LogError("Attach a camera terrain script to GameManager.cs!");

        if (mainMenuScript == null)
            Debug.LogError("Attach a main menu script to GameManager.cs!");

        cameraTransform = Camera.main.transform;
        cameraDefaultPosition = cameraTransform.transform.position;
	}

    void Update()
    {
        //Check for camera pan complete
        if (cameraPanScript.enabled)
        {
            if (cameraPanScript.Complete())
            {
                StartGame();
            }
        }
        else if (!gameStarted)
        {
            //Move camera to default
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, cameraDefaultPosition, Time.deltaTime);
        }
    }

    void StartGame()
    {
        gameStarted = true;
        cameraFollowScript.ActivateScript();
        respawnScript.enabled = true;
        terrainScript.enabled = false;
    }

    public void ActivateCameraPan()
    {
        cameraPanScript.enabled = true;
    }

    public void Win()
    {
        mainMenuScript.EnableUI();

        cameraFollowScript.enabled = false;
        respawnScript.enabled = false;
        terrainScript.enabled = true;
        gameStarted = false;

        respawnScript.RemoveFrogs();
    }
}
