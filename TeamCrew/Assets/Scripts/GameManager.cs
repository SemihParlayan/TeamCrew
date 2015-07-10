using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static Vector3 GetInput(string horizontalInput, string verticalInput)
    {
        if (Xbox)
        {
            horizontalInput += "X";
            verticalInput += "X";
        }
        else if (PS4)
        {
            horizontalInput += "PS";
            verticalInput += "PS";
        }
        Vector3 input = new Vector3(Input.GetAxis(horizontalInput), Input.GetAxis(verticalInput));
        return input;
    }
    public static bool GetGrip(string axis)
    {
        bool button = Input.GetButton(axis);
        if (button)
        {
            return true;
        }

        float trigger = 0.0f;

        if (Xbox)
            trigger = Input.GetAxis(axis + "X");
        else if (PS4)
            trigger = Input.GetAxis(axis + "PS");

        if (trigger >= 0.2f)
        {
            return true;
        }

        return false;
    }
    public static bool GetCheatButton()
    {
        if (!staticHacks)
            return false;

        return ((Xbox && Input.GetButtonDown("SelectX")) || (PS4 && Input.GetButtonDown("SelectPS")) || (Input.GetKeyDown(KeyCode.B)));
    }

    public bool xbox = false;
    public static bool Xbox;

    public bool ps4 = false;
    public static bool PS4;

    public bool digitalInput = false;
    public static bool DigitalInput;

    public GameObject fireWorks;
    public Animator finalStretch;
    public static Transform playerOne, playerTwo;
    public static float LevelHeight = -60.2f;

    private TutorialBubbles tutorialBubbles;
    private TopFrogSpawner topfrogSpawnerScript;
    public LevelGeneration generatorScript;
    public Respawn respawnScript;
    public CameraFollow cameraFollowScript;
    public CameraFollowTerrain  terrainScript;
    public CameraPan cameraPanScript;
    public MainMenu mainMenuScript;
    private Vector3 cameraDefaultPosition;
    private Transform cameraTransform;
    public bool gameActive;
    public MenuMusicController menuMusicController;
    public FinalMusic finalMusicCotroller;

    public LayerMask mask;
    private bool tutorilBubblesSpawned;
    public string singlePlayerStarted;

    public bool tutorialComplete;
    public bool hacks = true;
    private static bool staticHacks;

    void Awake()
    {
        //Set static variables
        Xbox = xbox;
        PS4 = ps4;
        staticHacks = hacks;
        DigitalInput = digitalInput;
    }

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
		topfrogSpawnerScript.accessoriesCount = -5;
        topfrogSpawnerScript.SpawnFrog(Random.Range(1, 3), 0f);

        tutorialBubbles = GetComponent<TutorialBubbles>();

        cameraTransform = Camera.main.transform;
        cameraDefaultPosition = cameraTransform.transform.position;
	}

    private FrogPrototype playerOneScript, playerTwoScript;
    void Update()
    {
        //TEMPORARY RESTART
        if (GameManager.Xbox)
        {
            if (Input.GetButtonDown("StartX"))
                Application.LoadLevel(Application.loadedLevel);
        }
        else if (GameManager.PS4)
        {
            if (Input.GetButtonDown("StartPS"))
                Application.LoadLevel(Application.loadedLevel);
        }

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

            if (mainMenuScript.goReady && !tutorialComplete && gameActive || (playerOne && playerTwo && hacks ? GetCheatButton() || Input.GetKeyDown(KeyCode.B) : false))
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
                Debug.Log("Camera pan complete");
                StartGame();
                generatorScript.DeactivateEasyBlock();
            }
        }
        else if (!gameActive)
        {
            //Move camera to default
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, cameraDefaultPosition, Time.deltaTime);

            //Zoom to default
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 7.5f, Time.deltaTime / 2);

            bool started = true;
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                mainMenuScript.playerOneReadyInput.singlePlayerReady = true;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                mainMenuScript.playerTwoReadyInput.singlePlayerReady = true;
            }
            else if(!Input.GetKeyDown(KeyCode.B))
            {
                started = false;
            }


            /////// CHEAT WITH SELECT
            if (GetCheatButton())
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
        if (gameActive)
            cameraFollowScript.absoluteMaxZoom = playedFinalStretch;

        


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
        finalMusicCotroller.enabled = true;

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
        menuMusicController.ChangeFadeState(Fade.outs);
        cameraPanScript.enabled = true;
        mainMenuScript.exitImage.gameObject.SetActive(false);
        Camera.main.orthographicSize = 7.5f;
    }
    public void Win(int frogNumber)
    {
        finalMusicCotroller.ChangeFadeState(Fade.outs);

        Vector2 deathCount = GetFrogDeathCount();

        int v = 0;
        if (frogNumber == 1)
        {
            v = (int)deathCount.y - (int)deathCount.x;
        }
        else
        {
            v = (int)deathCount.x - (int)deathCount.y;
        }
        
        mainMenuScript.StartMenuCycle(frogNumber, v);

        mainMenuScript.exitImage.gameObject.SetActive(true);
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
        topfrogSpawnerScript.accessoriesCount = v;
        topfrogSpawnerScript.SpawnFrog(frogNumber, 3f, true);
    }
    private void GoBackToMenu()
    {
        mainMenuScript.EnableUI();
        generatorScript.ActivateEasyBlock();

        mainMenuScript.exitImage.gameObject.SetActive(true);
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



        
        if (singlePlayerStarted == "P1")
        {
            playerOne = (Instantiate(respawnScript.playerOne.prefab, generatorScript.GetPlayerOneSpawnPosition(), Quaternion.identity) as Transform).FindChild("body");
        }
        else if (singlePlayerStarted == "P2")
        {
            playerTwo = (Instantiate(respawnScript.playerTwo.prefab, generatorScript.GetPlayerTwoSpawnPosition(), Quaternion.identity) as Transform).FindChild("body");
        }
        else
        {
            playerOne = (Instantiate(respawnScript.playerOne.prefab, generatorScript.GetPlayerOneSpawnPosition(), Quaternion.identity) as Transform).FindChild("body");
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
        bool button = GetGrip("P1GL");
        bool button2 = GetGrip("P1GR");

        if (input != Vector3.zero || input2 != Vector3.zero || button || button2 || (hacks && Input.GetMouseButton(0)))
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
        button = GetGrip("P2GL");
        button2 = GetGrip("P2GR");
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
