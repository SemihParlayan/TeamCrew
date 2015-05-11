﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject fireWorks;
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
    public bool CANPRESSMENU = true;
    private bool tutorilBubblesSpawned;

    public bool tutorialComplete;
    public bool hacks = true;
	void Start ()
    {
        Application.targetFrameRate = 60;

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
        
        //Start GAME!
        if (!cameraFollowScript.enabled)
        {
            if (playerOneScript != null && playerTwoScript != null)
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
            if (mainMenuScript.goReady && !tutorialComplete && gameActive || (playerOne && playerTwo && hacks ? Input.GetButtonDown("Select") : false))
            {
                TutorialComplete();
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
                generatorScript.DeactivateEasyBlock();
            }
        }
        else if (!gameActive)
        {
            //Move camera to default
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, cameraDefaultPosition, Time.deltaTime);

            if (((mainMenuScript.playerOneReadyInput.ready && mainMenuScript.playerTwoReadyInput.ready) || Input.GetKeyDown(KeyCode.B) || Input.GetButtonDown("Select")) && CANPRESSMENU)
            {
                CANPRESSMENU = false;
                ActivateCameraPan();
                generatorScript.Generate();
                mainMenuScript.DisableUI();
            }
        }

        //Inactivity
        if (gameActive)
        {
            //playerOneInactivityTimer += Time.deltaTime;
            //playerTwoInactivityTimer += Time.deltaTime;

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
        mainMenuScript.goReady = false;

        playerOne = GameObject.FindWithTag("PlayerOne").transform;
        playerTwo = GameObject.FindWithTag("PlayerTwo").transform;

        playerOneScript = playerOne.GetComponent<FrogPrototype>();
        playerTwoScript = playerTwo.GetComponent<FrogPrototype>();

        fireWorks.SetActive(false);
    }

    private void TutorialComplete()
    {
        cameraFollowScript.enabled = true;
        tutorialComplete = true;
        playerOne.GetComponent<Line>().Remove();
        playerTwo.GetComponent<Line>().Remove();

        TutorialBubbles bubbles = GetComponent<TutorialBubbles>();

        if (bubbles)
        {
            bubbles.Remove();
        }

        generatorScript.ActivateEasyBlock();
    }
    public void ActivateCameraPan()
    {
        cameraPanScript.enabled = true;
        Camera.main.orthographicSize = 7.5f;
    }
    public void Win()
    {
        mainMenuScript.EnableUI();

        inactivityText.transform.parent.gameObject.SetActive(false);
        cameraFollowScript.enabled = false;
        respawnScript.enabled = false;
        terrainScript.enabled = true;
        tutorialComplete = false;
        gameActive = false;
        respawnScript.ResetRespawns();
        fireWorks.SetActive(true);
        GetComponent<FlySpawner>().RemoveFly();

        Invoke("ENABLEMENU", 6f);
    }
    private void ENABLEMENU()
    {
        CANPRESSMENU = true;
    }
    private void CreateNewFrogs()
    {
        if (playerOne != null)
            Destroy(playerOne.parent.gameObject);
        if (playerTwo != null)
            Destroy(playerTwo.parent.gameObject);

        playerOne = (Instantiate(respawnScript.playerOne.prefab, generatorScript.GetPlayerOneSpawnPosition(), Quaternion.identity) as Transform).FindChild("body");
        playerTwo = (Instantiate(respawnScript.playerTwo.prefab, generatorScript.GetPlayerTwoSpawnPosition(), Quaternion.identity) as Transform).FindChild("body");

        if (!tutorilBubblesSpawned)
        {
            tutorilBubblesSpawned = true;
        }
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
