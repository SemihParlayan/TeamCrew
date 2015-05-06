using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static Transform playerOne, playerTwo;
    public static float LevelHeight;

    public LevelGeneration generatorScript;
    public Respawn respawnScript;
    public CameraFollow cameraFollowScript;
    public CameraFollowTerrain terrainScript;
    public CameraPan cameraPanScript;
    public MainMenu mainMenuScript;

    private Vector3 cameraDefaultPosition;
    private Transform cameraTransform;
    public bool gameActive;

    public LayerMask mask;

    public bool tutorialComplete;
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

    private FrogPrototype playerOneScript, playerTwoScript;
    void Update()
    {
        if (!cameraFollowScript.enabled)
        {
            if (playerOneScript != null || playerTwoScript != null)
            {
                mainMenuScript.playerOneReady.gameObject.SetActive(false);
                mainMenuScript.playerTwoReady.gameObject.SetActive(false);

                if (!mainMenuScript.goReady)
                {
                    if (playerOneScript.Ready)
                    {
                        mainMenuScript.playerOneReady.gameObject.SetActive(true);
                    }
                    if (playerTwoScript.Ready)
                    {
                        mainMenuScript.playerTwoReady.gameObject.SetActive(true);
                    }

                    if (playerOneScript.Ready && playerTwoScript.Ready)
                    {
                        mainMenuScript.StartGoImage();
                    }
                }
            }
            if (mainMenuScript.goReady)
            {
                cameraFollowScript.enabled = true;
                tutorialComplete = true;
            }
        }
        //Check for camera pan complete
        if (cameraPanScript.enabled)
        {
            if (cameraPanScript.Halfway())
            {
                CreateNewFrogs();
            }
            if (cameraPanScript.Complete())
            {
                StartGame();
            }
        }
        else if (!gameActive)
        {
            //Move camera to default
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, cameraDefaultPosition, Time.deltaTime);

            if (mainMenuScript.playerOneReadyInput.ready && mainMenuScript.playerTwoReadyInput.ready)
            {
                ActivateCameraPan();
                generatorScript.Generate();
                mainMenuScript.DisableUI();
            }
        }

        if (gameActive)
        {
            playerOneInactivityTimer += Time.deltaTime;
            playerTwoInactivityTimer += Time.deltaTime;

            if (playerOneInactivityTimer >= 5 && playerTwoInactivityTimer >= 5)
            {
                inactivityText.transform.parent.gameObject.SetActive(true);
                fadeTimer -= Time.deltaTime;

                inactivityText.text = "Inactivity! \n Returning to main menu in " + Mathf.RoundToInt(fadeTimer) + "...";

                if (fadeTimer <= 0)
                {
                    Win();
                    playerOneInactivityTimer = 0;
                    playerTwoInactivityTimer = 0;
                }
            }
            else
            {
                inactivityText.transform.parent.gameObject.SetActive(false);
                fadeTimer = 10;
            }
        }
    }

    float fadeTimer = 10;
    void StartGame()
    {
        gameActive = true;
        terrainScript.enabled = false;
        respawnScript.enabled = true;
        playerOne = GameObject.FindWithTag("PlayerOne").transform;
        playerTwo = GameObject.FindWithTag("PlayerTwo").transform;

        playerOneScript = playerOne.GetComponent<FrogPrototype>();
        playerTwoScript = playerTwo.GetComponent<FrogPrototype>();
    }

    public void ActivateCameraPan()
    {
        cameraPanScript.enabled = true;
    }
    public void Win()
    {
        mainMenuScript.EnableUI();

        inactivityText.transform.parent.gameObject.SetActive(false);
        cameraFollowScript.enabled = false;
        respawnScript.enabled = false;
        terrainScript.enabled = true;
        gameActive = false;
        respawnScript.ResetRespawns();
    }
    private void CreateNewFrogs()
    {
        if (playerOne != null)
            Destroy(playerOne.parent.gameObject);
        if (playerTwo != null)
            Destroy(playerTwo.parent.gameObject);

        playerOne = (Instantiate(respawnScript.playerOne.prefab, generatorScript.GetPlayerOneSpawnPosition(), Quaternion.identity) as Transform).FindChild("body");
        playerTwo = (Instantiate(respawnScript.playerTwo.prefab, generatorScript.GetPlayerTwoSpawnPosition(), Quaternion.identity) as Transform).FindChild("body");
    }

    public Text inactivityText;
    public float inactivityTime = 5;
    public float playerOneInactivityTimer;
    public float playerTwoInactivityTimer;
    public void DeactivateInactivityCounter(string frogname)
    {
        if (frogname.Contains("1"))
        {
            playerOneInactivityTimer = 0;
        }
        else if (frogname.Contains("2"))
        {
            playerTwoInactivityTimer = 0;
        }
    }
}
