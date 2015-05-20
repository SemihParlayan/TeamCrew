using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool joysticks = false;
    public static bool UsingJoysticks;
    public static Vector3 GetInput(string horizontalInput, string verticalInput)
    {
        if (!UsingJoysticks)
        {
            horizontalInput += "X";
            verticalInput   += "X";
        }
        Vector3 input = new Vector3(Input.GetAxis(horizontalInput), Input.GetAxis(verticalInput));
        return input;
    }

    public GameObject fireWorks;
    public Animator finalStretch;
    public static Transform playerOne, playerTwo;
    public static float LevelHeight;

    private TutorialBubbles tutorialBubbles;
    private TopFrogSpawner  topfrogSpawnerScript;
    public LevelGeneration  generatorScript;
    public Respawn  respawnScript;
    public CameraFollow cameraFollowScript;
    public CameraFollowTerrain  terrainScript;
    public CameraPan cameraPanScript;
    public MainMenu mainMenuScript;

    private Vector3             cameraDefaultPosition;
    private Transform           cameraTransform;
    public bool                 gameActive;

    public LayerMask            mask;
    private bool                tutorilBubblesSpawned;
    public string singlePlayerStarted;

    public bool tutorialComplete;
    public bool hacks = true;

	void Start ()
    {
        Application.targetFrameRate = 200;

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

        topfrogSpawnerScript = GetComponent<TopFrogSpawner>();
        topfrogSpawnerScript.SpawnFrog(Random.Range(1, 3), 0f);

        tutorialBubbles = GetComponent<TutorialBubbles>();

        cameraTransform = Camera.main.transform;
        cameraDefaultPosition = cameraTransform.transform.position;
        UsingJoysticks = joysticks;
	}

    private FrogPrototype playerOneScript, playerTwoScript;
    void Update()
    {
        //Start GAME!
        if (!cameraFollowScript.enabled)
        {
            mainMenuScript.playerOneReady.gameObject.SetActive(false);
            mainMenuScript.playerTwoReady.gameObject.SetActive(false);

            if (!mainMenuScript.goReady)
            {
                bool playerOneReady = false;
                bool playerTwoReady = false;

                //Check for player one ready
                if (playerOneScript)
                {
                    if (playerOneScript.Ready)
                    {
                        if (singlePlayerStarted == string.Empty)
                            mainMenuScript.playerOneReady.gameObject.SetActive(true);
                        playerOneReady = true;
                    }
                }

                //Check for player two ready
                if (playerTwoScript)
                {
                    if (playerTwoScript.Ready)
                    {
                        if (singlePlayerStarted == string.Empty)
                            mainMenuScript.playerTwoReady.gameObject.SetActive(true);
                        playerTwoReady = true;
                    }
                }
                

                //Start in multiplayer
                if (singlePlayerStarted == string.Empty)
                {
                    if (playerOneReady && playerTwoReady)
                    {
                        mainMenuScript.StartGoImage(generatorScript.GetReadySetGoSpriteRenderes());  
                    }
                }
                else // Start singleplayer
                {
                    if (playerOneReady || playerTwoReady)
                    {
                        mainMenuScript.StartGoImage(generatorScript.GetReadySetGoSpriteRenderes());
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

            bool started = false;
            if (Input.GetKeyDown(KeyCode.B) || Input.GetButtonDown("Select"))
            {
                started = true;
            }
            else if (mainMenuScript.playerOneReadyInput.ready && mainMenuScript.playerTwoReadyInput.ready)
            {
                started = true;
                singlePlayerStarted = string.Empty;
            }
            else if (mainMenuScript.playerOneReadyInput.singlePlayerReady)
            {
                started = true;
                singlePlayerStarted = "P1";
            }
            else if (mainMenuScript.playerTwoReadyInput.singlePlayerReady)
            {
                started = true;
                singlePlayerStarted = "P2";
            }

            if (started)
            {
                ActivateCameraPan();
                generatorScript.Generate();
                mainMenuScript.DisableUI();
            }
        }

        ///////////////////////////////////////////////////////////////////////////
        //                      Inactivity
        ///////////////////////////////////////////////////////////////////////////
        if (gameActive)
        {
            playerOneInactivityTimer += Time.deltaTime;
            playerTwoInactivityTimer += Time.deltaTime;

            if (playerOneInactivityTimer >= 5 && playerTwoInactivityTimer >= 5)
            {
                inactivityText.transform.parent.gameObject.SetActive(true);
                inactivityTimer -= Time.deltaTime;

                inactivityText.text = "Inactivity! \n Returning to main menu in " + Mathf.RoundToInt(inactivityTimer) + "...";

                if (inactivityTimer <= 0)
                {
                    GoBackToMenu();
                    playerOneInactivityTimer = 0;
                    playerTwoInactivityTimer = 0;
                }
            }
            else
            {
                inactivityText.transform.parent.gameObject.SetActive(false);
                inactivityTimer = 5;
            }
        }


        DeactivateInactivityCounter();
        respawnScript.playerOne.inactive = PlayerOneInactive();
        respawnScript.playerTwo.inactive = PlayerTwoInactive();


        ///////////////////////////////////////////////////////////////////////////
        //                      Final stretch
        ///////////////////////////////////////////////////////////////////////////
        float height = Mathf.Abs(LevelHeight);
        float climbedNormalDistance = (Camera.main.transform.position.y + height) / height;

        if (climbedNormalDistance >= 0.8f && !playedFinalStretch && gameActive)
        {
            playedFinalStretch = true;
            finalStretch.SetTrigger("Play");
        }



        ///////////////////////////////////////////////////////////////////////////
        //                      Singleplayer
        ///////////////////////////////////////////////////////////////////////////
        if (singlePlayerStarted == "P1")
        {
            playerTwoInactivityTimer = 10;
        }
        else if (singlePlayerStarted == "P2")
        {
            playerOneInactivityTimer = 10;
        }
    }
    private bool playedFinalStretch = true;

    float inactivityTimer = 5;
    void StartGame()
    {
        playedFinalStretch = false;
        gameActive = true;
        terrainScript.enabled = false;
        respawnScript.enabled = true;
        mainMenuScript.goReady = false;

        GameObject player = GameObject.FindWithTag("PlayerOne");
        if (player)
        {
            playerOne = player.transform;
            playerOneScript = playerOne.GetComponent<FrogPrototype>();
        }

        player = GameObject.FindWithTag("PlayerTwo");
        if (player)
        {
            playerTwo = player.transform;
            playerTwoScript = playerTwo.GetComponent<FrogPrototype>();
        }

        respawnScript.playerOne.deathCount = 0;
        respawnScript.playerTwo.deathCount = 0;

        fireWorks.SetActive(false);

        tutorialBubbles.EnableScript();
    }

    private void TutorialComplete()
    {
        cameraFollowScript.enabled = true;
        tutorialComplete = true;

        if (playerOne != null)
            playerOne.GetComponent<Line>().Remove();

        if (playerTwo != null)
            playerTwo.GetComponent<Line>().Remove();

        tutorialBubbles.DisableScript();

        generatorScript.ActivateEasyBlock();
    }
    public void ActivateCameraPan()
    {
        cameraPanScript.enabled = true;
        Camera.main.orthographicSize = 7.5f;
    }
    public void Win(int frogNumber)
    {
        mainMenuScript.StartMenuCycle(frogNumber);

        inactivityText.transform.parent.gameObject.SetActive(false);
        cameraFollowScript.enabled = false;
        respawnScript.enabled = false;
        terrainScript.enabled = true;
        tutorialComplete = false;
        gameActive = false;
        respawnScript.ResetRespawns();
        GetComponent<FlySpawner>().RemoveFly();
        fireWorks.SetActive(true);
        fireWorks.GetComponent<Fireworks>().Reset();

        Invoke("DestroyFrogs", 3f);
        topfrogSpawnerScript.SpawnFrog(frogNumber, 3f);
    }
    private void GoBackToMenu()
    {
        mainMenuScript.EnableUI();
        generatorScript.ActivateEasyBlock();

        inactivityText.transform.parent.gameObject.SetActive(false);
        cameraFollowScript.enabled = false;
        respawnScript.enabled = false;
        terrainScript.enabled = true;
        tutorialComplete = false;
        gameActive = false;
        respawnScript.ResetRespawns();
        GetComponent<FlySpawner>().RemoveFly();

        Invoke("DestroyFrogs", 3f);
        topfrogSpawnerScript.SpawnFrog(Random.Range(1, 3), 0f);
    }
    private void CreateNewFrogs()
    {
        DestroyFrogs();
        GetComponent<BandageManager>().ResetBandages();



        if (singlePlayerStarted == string.Empty)
        {
            playerOne = (Instantiate(respawnScript.playerOne.prefab, generatorScript.GetPlayerOneSpawnPosition(), Quaternion.identity) as Transform).FindChild("body");
            playerTwo = (Instantiate(respawnScript.playerTwo.prefab, generatorScript.GetPlayerTwoSpawnPosition(), Quaternion.identity) as Transform).FindChild("body");
        }
        else if (singlePlayerStarted == "P1")
        {
            playerOne = (Instantiate(respawnScript.playerOne.prefab, generatorScript.GetPlayerOneSpawnPosition(), Quaternion.identity) as Transform).FindChild("body");
        }
        else if (singlePlayerStarted == "P2")
        {
            playerTwo = (Instantiate(respawnScript.playerTwo.prefab, generatorScript.GetPlayerTwoSpawnPosition(), Quaternion.identity) as Transform).FindChild("body");
        }


        topfrogSpawnerScript.RemoveFrog();
        if (!tutorilBubblesSpawned)
        {
            tutorilBubblesSpawned = true;
        }
    }
    private void DestroyFrogs()
    {
        if (playerOne != null)
            Destroy(playerOne.parent.gameObject);
        if (playerTwo != null)
            Destroy(playerTwo.parent.gameObject);
    }

    public Vector2 GetFrogDeathCount()
    {
        return new Vector2(respawnScript.playerOne.deathCount, respawnScript.playerTwo.deathCount);
    }
    public Text inactivityText;
    public float inactivityTime = 5;
    public float playerOneInactivityTimer;
    public float playerTwoInactivityTimer;
    private void DeactivateInactivityCounter()
    {
        Vector3 input = GetInput("P1HL", "P1VL");
        Vector3 input2 = GetInput("P1HR", "P1VR");
        bool button = Input.GetButton("P1GL");
        bool button2 = Input.GetButton("P1GR");

        if (input != Vector3.zero || input2 != Vector3.zero || button || button2)
        {   
            if (singlePlayerStarted == string.Empty || singlePlayerStarted == "P1")
                playerOneInactivityTimer = 0;
            else if (tutorialComplete)
            {
                playerOneInactivityTimer = 0;
                singlePlayerStarted = string.Empty;
            }
        }

        input = GetInput("P2HL", "P2VL");
        input2 = GetInput("P2HR", "P2VR");
        button = Input.GetButton("P2GL");
        button2 = Input.GetButton("P2GR");
        if (input != Vector3.zero || input2 != Vector3.zero || button || button2)
        {
            if (singlePlayerStarted == string.Empty || singlePlayerStarted == "P2")
                playerTwoInactivityTimer = 0;
            else if (tutorialComplete)
            {
                playerTwoInactivityTimer = 0;
                singlePlayerStarted = string.Empty;
            }
        }
    }
    public bool PlayerOneInactive()
    {
        return playerOneInactivityTimer >= 7.5f;
    }
    public bool PlayerTwoInactive()
    {
        return playerTwoInactivityTimer >= 7.5f;
    }
}
