using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //Static variables and properties
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
        if (!Hacks)
            return false;

        return ((Xbox && Input.GetButtonDown("SelectX")) || (PS4 && Input.GetButtonDown("SelectPS")) || (Input.GetKeyDown(KeyCode.B)));
    }
    public static bool Hacks;
    public static bool Xbox;
    public static bool PS4;
    public static bool DigitalInput;

    public static Transform playerOne, playerTwo;
    private static float levelHeight = 1337;
    public static float LevelHeight
    {
        set
        {
            Parallax.LevelHeight = value;
            levelHeight = value;
        }
        get
        {
            return levelHeight;
        }
    }

    //////////////////////////


    public bool designTestingEnabled;
    [HideInInspector] public bool gameActive;
    [HideInInspector] public bool tutorialComplete;
    public bool hacks = true;
    public bool xbox = false;
    public bool ps4 = false;
    public bool digitalInput = false;

    private GameObject fireWorks;
    public Animator finalStretch;
    

    private TutorialBubbles tutorialBubbles;
    private TopFrogSpawner topfrogSpawnerScript;
    private LevelGeneration generatorScript;
    private Respawn respawnScript;
    private CameraFollow cameraFollowScript;
    private CameraFollowTerrain  cameraFollowTerrainScript;
    private CameraPan cameraPanScript;
    public MainMenu mainMenuScript;
    private Vector3 cameraDefaultPosition;
    public MenuMusicController menuMusicController;
    public FinalMusic finalMusicCotroller;
    private InactivityController inactivityController;
    private FrogPrototype playerOneScript, playerTwoScript;    

    private bool playedFinalStretch = true;

    [HideInInspector]
    public string singlePlayerStarted;
    public bool IsInMultiplayerMode { get { return (singlePlayerStarted == string.Empty); } }

    void Awake()
    {
        //Set static variables to their coresponding public property.
        Xbox = xbox;
        PS4 = ps4;
        Hacks = hacks;
        DigitalInput = digitalInput;

        //Activate design testing
        if (designTestingEnabled)
        {
            gameActive = true;
            tutorialComplete = true;
        }
    }
	void Start ()
    {
        Application.targetFrameRate = 200;

        //Aquire LevelGenerator script
        generatorScript = GetComponent<LevelGeneration>();
        if (generatorScript == null)
        {
            Debug.LogError("Can't find a generator script on GameManager object");
        }

        //Aquire Respawn script
        respawnScript = GetComponent<Respawn>();
        if (respawnScript == null)
        {
            Debug.LogError("Can't find a respawn script on GameManager object");
        }
        
        //Aquire CameraFollow script
        Camera mainCamera = Camera.main;
        cameraFollowScript = mainCamera.GetComponent<CameraFollow>();
        if (cameraFollowScript == null)
        {
            Debug.LogError("Can't find a CameraFollowScript script on MainCamera!");
        }

        //Aquire CameraPan script
        cameraPanScript = mainCamera.GetComponent<CameraPan>();
        if (cameraPanScript == null)
        {
            Debug.LogError("Can't find a Camerapan script on MainCamera!");
        }

        //Aquire CameraFollowTerrain script
        cameraFollowTerrainScript = mainCamera.GetComponent<CameraFollowTerrain>();
        if (cameraFollowTerrainScript == null)
        {
            Debug.LogError("Can't find a CameraFollowTerrainScript script on MainCamera!");
        }
        
        //Aquire fireworks gameobject
        fireWorks = GameObject.FindWithTag("Fireworks");
        if (fireWorks == null)
        {
            Debug.LogError("Can't find an object with tag: Fireworks");
        }
        else
        {
            fireWorks.SetActive(false);
        }

        //Aquire inactivityController script
        inactivityController = GetComponent<InactivityController>();
        if (inactivityController == null)
        {
            Debug.LogError("Can't find a InactivityController script on GameManager object");
        }

        //Check if mainmenuScript is applied
        if (mainMenuScript == null)
        {
            Debug.LogError("Attach a main menu script to GameManager.cs!");
        }

        //Aquire topFrogSpawner script
        topfrogSpawnerScript = GetComponent<TopFrogSpawner>();
        if (topfrogSpawnerScript == null)
        {
            Debug.LogError("Can't find a TopFrogSpawner script on GameManager object");
        }
        else
        {
            topfrogSpawnerScript.accessoriesCount = -5;
            topfrogSpawnerScript.SpawnFrog(Random.Range(1, 3), 0f);
        }
		
        //Aquire tutorialBubbles script
        tutorialBubbles = GetComponent<TutorialBubbles>();
        if (tutorialBubbles == null)
        {
            Debug.LogError("Can't find a TutorialBubbles script on GameManager object");
        }

        //Set cameras default position
        cameraDefaultPosition = mainCamera.transform.position;

        //menuMusicController = ;

        //Enable menu music
        menuMusicController.Play();
        menuMusicController.ChangeFadeState(FadeState.IN);
	}

    //Update method
    void Update()
    {
        RestartGame();
        CheckForTutorialComplete();
        CheckForCameraPanComplete();
        CheckForFinalStretch();
        CheckForPlayersReadyInMenu();
    }
    



    //Single use methods
    /// <summary>
    /// Deletes old frogs and creates either one or two new frogs depending on which game mode the game is currently in.
    /// </summary>
    private void CreateNewFrogs()
    {
        //Remove old frogs.
        DestroyFrogs();

        //Reset the bandage counter.
        GetComponent<BandageManager>().ResetBandages();

        //Aquire spawn positions for frogs
        Vector3 pOneSpawnPos = generatorScript.GetPlayerOneSpawnPosition();
        Vector3 pTwoSpawnPos = generatorScript.GetPlayerTwoSpawnPosition();

        //Spawn frogs depending on which mode the game was started in.
        if (singlePlayerStarted == "P1")
        {
            playerOne = (Instantiate(respawnScript.playerOne.prefab, pOneSpawnPos, Quaternion.identity) as Transform).FindChild("body");
        }
        else if (singlePlayerStarted == "P2")
        {
            playerTwo = (Instantiate(respawnScript.playerTwo.prefab, pTwoSpawnPos, Quaternion.identity) as Transform).FindChild("body");
        }
        else
        {
            playerOne = (Instantiate(respawnScript.playerOne.prefab, pOneSpawnPos, Quaternion.identity) as Transform).FindChild("body");
            playerTwo = (Instantiate(respawnScript.playerTwo.prefab, pTwoSpawnPos, Quaternion.identity) as Transform).FindChild("body");
        }
    }

    /// <summary>
    /// Deletes the current active frogs.
    /// </summary>
    private void DestroyFrogs()
    {
        //Remove player one frog
        if (playerOne != null)
            Destroy(playerOne.parent.gameObject);

        //Remove player two frog
        if (playerTwo != null)
            Destroy(playerTwo.parent.gameObject);
    }

    /// <summary>
    /// Sets the tutorial to complete and more such as disabling tutorial bubbles, remove safety lines etc...
    /// </summary>
    private void TutorialComplete()
    {
        //Set flags
        tutorialComplete = true;
        cameraFollowScript.enabled = true;

        //Disable tutorial bubbles;
        tutorialBubbles.DisableScript();

        //Reactivate the easy block thats above the tutorial.
        generatorScript.ActivateEasyBlock();

        //Disable ready signs
        mainMenuScript.playerOneReady.gameObject.SetActive(false);
        mainMenuScript.playerTwoReady.gameObject.SetActive(false);


        //Remove player one safety line
        if (playerOne != null)
        {
            if (playerOne.GetComponent<Line>() != null)
                playerOne.GetComponent<Line>().Remove();
        }
            
        //Remove player two safety line
        if (playerTwo != null)
        {
            if (playerTwo.GetComponent<Line>() != null)
                playerTwo.GetComponent<Line>().Remove();
        }
            
    }

    /// <summary>
    /// Sets the camera in motion to pan down the mountain.
    /// </summary>
    public void ActivateCameraPan()
    {
        //activate woosh
        Camera.main.transform.GetComponent<AudioSource>().enabled = true;

        //Set menu music to start fading out
        menuMusicController.ChangeFadeState(FadeState.OUT);
           
        //Activate cameraPan script
        cameraPanScript.enabled = true;
        cameraFollowTerrainScript.enabled = true;

        //Disable exitbuttonImage
        mainMenuScript.exitImage.gameObject.SetActive(false);

        //Set the start zoom value for the camera when panning down.
        Camera.main.orthographicSize = 7.5f;
    }

    /// <summary>
    /// Return the frogs total deathcount. X = PlayerOne death count. Y = PlayerTwo death count.
    /// </summary>
    public Vector2 GetFrogDeathCount()
    {
        return new Vector2(respawnScript.playerOne.deathCount, respawnScript.playerTwo.deathCount);
    }

    /// <summary>
    /// Resets the game variables and returns the camera to the menu (Top of the mountain).
    /// </summary>
    public void GoBackToMenu()
    {
        //Enable UI
        mainMenuScript.EnableUI();

        //Enable menu music
        menuMusicController.Play();
        menuMusicController.ChangeFadeState(FadeState.IN);

        //Reactivate block above tutorial
        generatorScript.ActivateEasyBlock();

        //Reset menu variables
        ResetVariablesToMenu();

        //Destroy old frogs and spawn new topfrog
        Invoke("DestroyFrogs", 3f);
        topfrogSpawnerScript.SpawnFrog(Random.Range(1, 3), 0f);
    }

    /// <summary>
    /// Resets all boolean flags needed to return to menu for a safe restart of the game. Enables exit button in menu & removes fly.
    /// </summary>
    private void ResetVariablesToMenu()
    {
        //Disable inactivity controller
        inactivityController.inactivityText.transform.parent.gameObject.SetActive(false);

        //Enable fall sound
        Camera.main.transform.GetComponent<AudioSource>().enabled = true;

        //Enable exit button in menu again
        mainMenuScript.exitImage.gameObject.SetActive(true);

        //Reset boolean flags
        cameraFollowTerrainScript.enabled = true;
        cameraFollowScript.enabled = false;
        respawnScript.enabled = false;
        playedFinalStretch = false;
        tutorialComplete = false;
        gameActive = false;

        //Reset spawn timers
        respawnScript.ResetRespawns();

        //Remove any active fly
        GetComponent<FlySpawner>().RemoveFly();
    }

    /// <summary>
    /// Enables the menu cycle that shows which frog won, deathcount and sets accessories for the winning frog. Also activates FireWorks.
    /// </summary>
    /// <param name="frogNumber">What frog won the game? 1 or 2?</param>
    public void Win(int frogNumber)
    {
        //Fade out finalmusic
        finalMusicCotroller.SetFadeState(FadeState.OUT);

        //Aquire death count for both frogs
        Vector2 deathCount = GetFrogDeathCount();

        //Which frog won? store the number of accessories for winning frog in variable 'v'.
        int v = 0;
        if (frogNumber == 1)
        {
            v = (int)deathCount.y - (int)deathCount.x;
        }
        else
        {
            v = (int)deathCount.x - (int)deathCount.y;
        }

        //Start menu cycle, who won?, deathcount etc...
        mainMenuScript.StartMenuCycle(frogNumber, v);

        //Reset menu variables
        ResetVariablesToMenu();

        //Destroy old frogs and spawn new topfrog
        Invoke("DestroyFrogs", 3f);
        topfrogSpawnerScript.accessoriesCount = v;
        topfrogSpawnerScript.SpawnFrog(frogNumber, 3f, true);

        //Enable fireworks
        fireWorks.SetActive(true);
        fireWorks.GetComponent<Fireworks>().Reset();
    }

    /// <summary>
    /// Sets the game to active, meaning that the frogs can move and the tutorial is started.
    /// </summary>
    void StartGame()
    {
        //Enables respawning
        respawnScript.enabled = true;

        //Sets the game to be active (Camera pan has just reached the bottom)
        gameActive = true;

        //Reset variables
        playedFinalStretch = false;
        cameraFollowTerrainScript.enabled = false;
        mainMenuScript.goReady = false;

        //Find frog number one
        GameObject player = GameObject.FindWithTag("PlayerOne");
        if (player)
        {
            playerOne = player.transform;
            playerOneScript = playerOne.GetComponent<FrogPrototype>();
        }

        //Find frog number two
        player = GameObject.FindWithTag("PlayerTwo");
        if (player)
        {
            playerTwo = player.transform;
            playerTwoScript = playerTwo.GetComponent<FrogPrototype>();
        }

        //Reset deathcounter
        respawnScript.ResetDeathcount();

        //Disable fireworks
        fireWorks.SetActive(false);

        //Enable tutorial bubbles
        tutorialBubbles.EnableScript();
    }




    //Methods called from Update constantly
    /// <summary>
    /// Reloads the current scene loaded.
    /// </summary>
    private void RestartGame()
    {
        //Restart game with Xbox 360 Controller
        if (GameManager.Xbox)
        {
            if (Input.GetButtonDown("StartX"))
                Application.LoadLevel(Application.loadedLevel);
        }
        //Restart game with PS4 controller
        else if (GameManager.PS4)
        {
            if (Input.GetButtonDown("StartPS"))
                Application.LoadLevel(Application.loadedLevel);
        }
    }

    /// <summary>
    /// Check to see if the frogs/frog has reached the tutorial grips. If so start the game!
    /// </summary>
    private void CheckForTutorialComplete()
    {
        if (!gameActive || tutorialComplete)
            return;

        /*
         * We are currently inside of tutorial state for all the code below. Tutorial state means that the frogs are active and
         * are currently climbing the tutorial block. They have not yet completed the tutorial. The code below
         * will start the game whenever the frogs have completed the tutorial.
        */

        mainMenuScript.playerOneReady.gameObject.SetActive(false);
        mainMenuScript.playerTwoReady.gameObject.SetActive(false);

        bool playerOneCompletedTutorial = false;
        bool playerTwoCompletedTutorial = false;

        //Check for player one ready
        if (playerOneScript && playerOneScript.IsGrippingTutorial)
        {
            playerOneCompletedTutorial = true;
            if (IsInMultiplayerMode)
                mainMenuScript.playerOneReady.gameObject.SetActive(true);
        }
        //Check for player two ready
        if ((playerTwoScript && playerTwoScript.IsGrippingTutorial) || Input.GetKey(KeyCode.Space))
        {
            playerTwoCompletedTutorial = true;
            if (IsInMultiplayerMode)
                mainMenuScript.playerTwoReady.gameObject.SetActive(true);
        }


        //Start in multiplayer
        if (IsInMultiplayerMode)
        {
            if (playerOneCompletedTutorial && playerTwoCompletedTutorial)
            {
                mainMenuScript.StartGoImage(generatorScript.GetReadySetGoSpriteRenderes());
            }
        }
        //Start singleplayer
        else
        {
            if (playerOneCompletedTutorial || playerTwoCompletedTutorial)
            {
                mainMenuScript.StartGoImage(generatorScript.GetReadySetGoSpriteRenderes());
            }
        }

        //Set tutorial complete
        if (mainMenuScript.goReady && !tutorialComplete)
        {
            TutorialComplete();
        }
        //Set tutorial complete with HACKS
        else if (GetCheatButton())
        {
            TutorialComplete();
        }
    }

    /// <summary>
    /// Checks to see if the pan has reached halfway and when it is complete.
    /// </summary>
    private void CheckForCameraPanComplete()
    {
        if (!cameraPanScript.enabled)
            return;
        /*
         * We are currently in panning state which means we are currently panning down the mountain in the code below.
        */


        //We have panned halfway down
        if (cameraPanScript.Halfway())
        {
            //Spawn new frogs at the bottom of the mountain
            CreateNewFrogs();

            //Delete the topfrog
            topfrogSpawnerScript.RemoveFrog();
        }

        //We have panned all the way to the bottom
        if (cameraPanScript.Complete())
        {
            Camera.main.transform.GetComponent<AudioSource>().enabled = false; //Disable fall sound
            StartGame();
            generatorScript.DeactivateEasyBlock();
        }
    }

    /// <summary>
    /// Checks to see if the camera has reached the final stretch marker, if so it activates the final stretch text.
    /// </summary>
    private void CheckForFinalStretch()
    {
        if (!gameActive)
            return;

        //Aquire the height of the mountain.
        float height = Mathf.Abs(LevelHeight);
        
        /*
         * Calculate how far the camera is up the mountain. The value will be between 0 and 1.
         * 0 = Bottom
         *1 = Top
        */
        //float climbedNormalDistance = (Camera.main.transform.position.y + height) / height;

        //Play finalstretch animation if we have climbed 80% of the mountain.
        bool reachedStretchMarker = (Camera.main.transform.position.y+height >= height - 15);
        cameraFollowScript.absoluteFinalStretchZoom = reachedStretchMarker;

        if(reachedStretchMarker)
        {
            if (!playedFinalStretch)
            { 
                playedFinalStretch = true;
                finalStretch.SetTrigger("Play");
            }

            if (!finalMusicCotroller.enabled)
            {
                finalMusicCotroller.enabled = true;
            }

        }
        else
        {
            if(finalMusicCotroller.enabled)
            {
                finalMusicCotroller.enabled = false;
                //finalMusicCotroller.SetFadeState(FadeState.OUT);
            }
            
        }
    }

    /// <summary>
    /// Checks to see if the players are ready inside of the menu. This method starts the game in either singleplayer or multiplaye mode.
    /// </summary>
    private void CheckForPlayersReadyInMenu()
    {
        if (gameActive || cameraPanScript.enabled)
            return;

        if (designTestingEnabled)
            return;
        /*
         * The code below is currently in MenuState which means thats we are at the top of the mountain.
         * We are checking for the player to click ready in order to start the camerapan.
         */


        //Lerp cameras position towards the default position at the top of the mountain.
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraDefaultPosition, Time.deltaTime);

        //Lerp cameras zoom to default zoom distance at the top of the mountain.
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 7.5f, Time.deltaTime / 2);

        //Set flag to start game with only playerOne
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            mainMenuScript.playerOneReadyInput.singlePlayerReady = true;
        }
        //Set flag to start game with only playerTwo
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            mainMenuScript.playerTwoReadyInput.singlePlayerReady = true;
        }

        bool started = false;

        //Both frogs are ready, set flag to start in multiplayer mode.
        if (mainMenuScript.playerOneReadyInput.ready && mainMenuScript.playerTwoReadyInput.ready)
        {
            started = true;
            singlePlayerStarted = string.Empty;
        }
        //Only playerOne is ready, playerOne started the game in singleplayer mode.
        else if (mainMenuScript.playerOneReadyInput.singlePlayerReady)
        {
            started = true;
            singlePlayerStarted = "P1";
        }
        //Only playerTwo is ready, playerTwo started the game in singleplayer mode.
        else if (mainMenuScript.playerTwoReadyInput.singlePlayerReady)
        {
            started = true;
            singlePlayerStarted = "P2";
        }

        //Start game with cheatbutton. This will start the game in multiplayerMode.
        if (GetCheatButton())
        {
            started = true;
        }

        /*
         * If the flag has been set to start game in either singleplayer or multiplayed mode we
         * activate the camera pan, generate a new level to climb and disable the UI.
         */
        if (started)
        {
            ActivateCameraPan();
            generatorScript.Generate();
            mainMenuScript.DisableUI();
        }
    }
}
