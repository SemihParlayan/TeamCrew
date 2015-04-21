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
	}

    void Update()
    {
        if (cameraPanScript.enabled)
        {
            if (cameraPanScript.Complete())
            {
                StartGame();
            }
        }
    }

    void StartGame()
    {
        cameraFollowScript.enabled = true;
        respawnScript.enabled = true;
        terrainScript.enabled = false;
    }

    public void ActivateCameraPan()
    {
        cameraPanScript.enabled = true;
    }
}
