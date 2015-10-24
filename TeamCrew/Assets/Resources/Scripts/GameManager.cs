using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //Static variables and properties
    public static Vector3 GetInput(string horizontalInput, string verticalInput)
    {
        if (!horizontalInput.Contains("P") || !verticalInput.Contains("P"))
            return Vector3.zero;

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
        if (!axis.Contains("P"))
            return false;

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
    public static bool GetButtonDown(string buttonName)
    {
        if (Xbox)
        {
            return Input.GetButtonDown(buttonName + "X");
        }
        else if (PS4)
        {
            return Input.GetButtonDown(buttonName + "PS");
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

    public static Transform[] players = new Transform[4];
    public InactivityController inactivityController;
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

    private TutorialBubbles tutorialBubbles;
    private TopFrogSpawner topfrogSpawnerScript;
    private LevelGeneration generatorScript;
    private Respawn respawnScript;
    private CameraFollow cameraFollowScript;
    public MenuMusicController menuMusicController;
    public FinalMusic finalStretchMusic;
    public Animator finalStretch;
    public FrogPrototype[] playerScripts = new FrogPrototype[4];
    [HideInInspector]
    public bool[] frogsReady = new bool[4];
    public EndgameScreen endGameScreen;

    private bool playerFinalStretchAnimation = true;
    private Transform topFrogPrefab;
    private int victoryFrogNumber;

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

        //Aquire topFrogSpawner script
        topfrogSpawnerScript = GetComponent<TopFrogSpawner>();
        if (topfrogSpawnerScript == null)
        {
            Debug.LogError("Can't find a TopFrogSpawner script on GameManager object");
        }
        else
        {
            topfrogSpawnerScript.accessoriesCount = -5;
        }
		
        //Aquire tutorialBubbles script
        tutorialBubbles = GetComponent<TutorialBubbles>();
        if (tutorialBubbles == null)
        {
            Debug.LogError("Can't find a TutorialBubbles script on GameManager object");
        }

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
        //CheckForCameraPanComplete();
        CheckForFinalStretch();
        //CheckForPlayersReadyInMenu();
        SetPlayerScripts();
    }


    //Single use methods
    /// <summary>
    /// Deletes old frogs and creates either one or two new frogs depending on which game mode the game is currently in.
    /// </summary>
    public void CreateNewFrogs()
    {
        //Remove old frogs.
        DestroyFrogs();

        //Reset the bandage counter.
        GetComponent<BandageManager>().ResetBandages();

        for (int i = 0; i < players.Length; i++)
        {
            if (frogsReady[i])
            {
                Vector3 spawnPosition = generatorScript.GetPlayerSpawnPosition(i + 1);
                players[i] = (Instantiate(respawnScript.respawnScripts[i].prefab, spawnPosition, Quaternion.identity) as Transform).FindChild("body");
            }
        }
    }

    /// <summary>
    /// Deletes the current active frogs.
    /// </summary>
    public void DestroyFrogs()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != null)
            {
                Destroy(players[i].parent.gameObject);
            }
        }
    }

    /// <summary>
    /// Sets the tutorial to complete and more such as disabling tutorial bubbles, remove safety lines etc...
    /// </summary>
    private void TutorialComplete()
    {
        if (tutorialComplete)
            return;

        //Set flags
        tutorialComplete = true;
        cameraFollowScript.enabled = true;

        //Disable tutorial bubbles;
        tutorialBubbles.DisableScript();

        //Reactivate the easy block thats above the tutorial.
        //generatorScript.ActivateEasyBlock();

        //Remove players safety line
        for (int i = 0; i < players.Length; i++)
        {
            Transform player = players[i];

            if (player != null)
            {
                Line line = player.GetComponent<Line>();

                if (line)
                {
                    line.Remove();
                }
            }
        }   
    }

    /// <summary>
    /// Sets the camera in motion to pan down the mountain.
    /// </summary>
    public void ActivateCameraPan()
    {
        //activate woosh
        Camera.main.transform.GetComponent<AudioSource>().enabled = true;
        Camera.main.transform.GetComponent<FallSoundCam>().enabled = true;

        //Set menu music to start fading out
        menuMusicController.ChangeFadeState(FadeState.OUT);
           
        //Activate cameraPan script
        //cameraPanScript.enabled = true;
        //cameraFollowTerrainScript.enabled = true;

        //Set the start zoom value for the camera when panning down.
        //Camera.main.orthographicSize = 7.5f;
    }

    /// <summary>
    /// Return the frogs total deathcount. X = PlayerOne death count. Y = PlayerTwo death count.
    /// </summary>
    public Vector4 GetFrogDeathCount()
    {
        return new Vector4(respawnScript.respawnScripts[0].deathCount, respawnScript.respawnScripts[1].deathCount, respawnScript.respawnScripts[2].deathCount, respawnScript.respawnScripts[3].deathCount);
    }

    /// <summary>
    /// Resets the game variables and returns the camera to the menu (Top of the mountain).
    /// </summary>
    public void GoBackToMenu()
    {
        GameObject.FindWithTag("MenuManager").GetComponent<M_ScreenManager>().enabled = true;
        M_ScreenManager.SwitchScreen(endGameScreen);
        endGameScreen.OnEnter(generatorScript.GetTopPosition());

        ////Enable UI
        //mainMenuScript.EnableUI();

        ////Reactivate block above tutorial
        //generatorScript.ActivateEasyBlock();

        ////Reset menu variables
        //ResetVariablesToMenu();

        ////Destroy old frogs and spawn new topfrog
        //Invoke("DestroyFrogs", 3f);
        //topfrogSpawnerScript.SpawnFrog(Random.Range(1, 3), 0f);

        ////Enable menu music
        //menuMusicController.Play();
        //menuMusicController.ChangeFadeState(FadeState.IN);
    }

    /// <summary>
    /// Resets all boolean flags needed to return to menu for a safe restart of the game. Enables exit button in menu & removes fly.
    /// </summary>
    private void ResetGameVariables()
    {
        //Disable inactivity controller
        inactivityController.inactivityText.transform.parent.gameObject.SetActive(false);

        //Enable fall sound
        Camera.main.transform.GetComponent<AudioSource>().enabled = true;
        Camera.main.transform.GetComponent<FallSoundCam>().enabled = true;

        //Reset boolean flags
        cameraFollowScript.enabled = false;
        respawnScript.enabled = false;
        playerFinalStretchAnimation = false;
        tutorialComplete = false;
        gameActive = false;

        //Reset spawn timers
        respawnScript.ResetRespawns();

        //Remove any active fly
        GetComponent<LadybugSpawner>().RemoveFly();
    }

    /// <summary>
    /// Enables the menu cycle that shows which frog won, deathcount and sets accessories for the winning frog. Also activates FireWorks.
    /// </summary>
    /// <param name="frogNumber">What frog won the game? 1 or 2?</param>
    public void Win(Transform topfrogPrefab, int victoryFrogNumber)
    {
        this.victoryFrogNumber = victoryFrogNumber;
        this.topFrogPrefab = topfrogPrefab;

        GoBackToMenu();

        //Fade out finalmusic
        finalStretchMusic.SetFadeState(FadeState.OUT);

        //Aquire death count for both frogs
        Vector2 deathCount = GetFrogDeathCount();

        //Which frog won? store the number of accessories for winning frog in variable 'v'.
        //int v = 0;
        //if (frogNumber == 1)
        //{
        //    v = (int)deathCount.y - (int)deathCount.x;
        //}
        //else
        //{
        //    v = (int)deathCount.x - (int)deathCount.y;
        //}

        //Reset menu variables
        ResetGameVariables();

        //Destroy old frogs and spawn new topfrog
        //topfrogSpawnerScript.accessoriesCount = v;
        //topfrogSpawnerScript.SpawnFrog(frogNumber, 3f, true);

        //Enable fireworks
        fireWorks.transform.position = generatorScript.GetTopPosition() + new Vector3(0, -18, 0);
        fireWorks.SetActive(true);
        fireWorks.GetComponent<Fireworks>().Reset();
    }

    /// <summary>
    /// Sets the game to active, meaning that the frogs can move and the tutorial is started.
    /// </summary>
    public void StartGame()
    {
        //Enables respawning
        respawnScript.enabled = true;

        //Sets the game to be active (Camera pan has just reached the bottom)
        gameActive = true;

        //Reset variables
        playerFinalStretchAnimation = false;

        //Find frogs
        for (int i = 1; i < 5; i++)
        {
            GameObject player = GameObject.FindWithTag("Player" + i.ToString());
            if (player)
            {
                players[i - 1] = player.transform;
                playerScripts[i - 1] = player.GetComponent<FrogPrototype>();
            }
        }

        inactivityController.OnGameStart();

        //Reset deathcounter
        respawnScript.ResetDeathcount();

        //Disable fireworks
        fireWorks.SetActive(false);

        //Enable tutorial bubbles
        tutorialBubbles.EnableScript();
    }

    /// <summary>
    /// Return how many frogs was ready in the menu
    /// </summary>
    /// <returns></returns>
    public int GetFrogReadyCount()
    {
        int count = 0;
        for (int i = 0; i < frogsReady.Length; i++)
        {
            if (frogsReady[i])
            {
                count++;
            }
        }

        return count;
    }

    /// <summary>
    /// Spawns the victory frog on top of the mountain
    /// </summary>
    public void SpawnTopFrog()
    {
        topfrogSpawnerScript.spawnPosition = generatorScript.GetTopPosition();
        topfrogSpawnerScript.SpawnFrog(topFrogPrefab, victoryFrogNumber, 0f);
    }

    //Static methods
    /// <summary>
    /// Returns the frog transform that is currently at the highest Y position
    /// </summary>
    /// <returns></returns>
    public static Transform GetTopFrog()
    {
        Transform frog = null;

        int topFrogIndex = -1;
        float topY = -float.MaxValue;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == null)
                continue;

            if (players[i].position.y > topY)
            {
                topFrogIndex = i;
                topY = players[i].position.y;
            }
        }

        if (topFrogIndex != -1)
        {
            frog = players[topFrogIndex];
        }

        return frog;
    }
    /// <summary>
    /// Returns the frog transform that is currently at the lowest Y position
    /// </summary>
    /// <returns></returns>
    public static Transform GetBottomFrog()
    {
        Transform frog = null;

        int bottomFrogIndex = -1;
        float bottomY = float.MaxValue;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == null)
                continue;

            if (players[i].position.y < bottomY)
            {
                bottomFrogIndex = i;
                bottomY = players[i].position.y;
            }
        }

        if (bottomFrogIndex != -1)
        {
            frog = players[bottomFrogIndex];
        }

        return frog;
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

        int completedTutorialCount = 0;
        for (int i = 0; i < playerScripts.Length; i++)
        {
            if (playerScripts[i])
            {
                if (playerScripts[i].IsGrippingTutorial)
                {
                    completedTutorialCount++;
                }
            }
        }

        if (completedTutorialCount == GetFrogReadyCount())
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
        //if (!cameraPanScript.enabled)
        //    return;
        ///*
        // * We are currently in panning state which means we are currently panning down the mountain in the code below.
        //*/


        ////We have panned halfway down
        //if (cameraPanScript.Halfway())
        //{
        //    //Spawn new frogs at the bottom of the mountain
        //    CreateNewFrogs();

        //    //Delete the topfrog
        //    topfrogSpawnerScript.RemoveFrog();
        //}

        ////We have panned all the way to the bottom
        //if (cameraPanScript.Complete())
        //{
        //    Camera.main.transform.GetComponent<AudioSource>().enabled = false; //Disable fall sound
        //    Camera.main.transform.GetComponent<FallSoundCam>().enabled = false;
        //    StartGame();
        //    generatorScript.DeactivateEasyBlock();
        //}
    }

    /// <summary>
    /// Checks to see if the camera has reached the final stretch marker, if so it activates the final stretch text.
    /// </summary>
    private void CheckForFinalStretch()
    {
        if (!gameActive)
            return;

        //Play finalstretch animation if we have climbed 80% of the mountain.
        bool reachedStretchMarker = (Camera.main.transform.position.y >= LevelHeight - 15);
        cameraFollowScript.absoluteFinalStretchZoom = reachedStretchMarker;

        if(reachedStretchMarker)
        {
            if (!playerFinalStretchAnimation)
            { 
                playerFinalStretchAnimation = true;
                finalStretch.SetTrigger("Play");
            }
            finalStretchMusic.SetFadeState(FadeState.IN);
        }
        else
        {
            finalStretchMusic.SetFadeState(FadeState.OUT);
        }
    }

    /// <summary>
    /// This method finds which frogs are active and which are not and sets the player scripts accordingly
    /// </summary>
    private void SetPlayerScripts()
    {
        for (int i = 0; i < playerScripts.Length; i++)
        {
            if (GameManager.players[i] != null)
            {
                if (playerScripts[i] == null)
                    playerScripts[i] = GameManager.players[i].GetComponent<FrogPrototype>();
            }
            else
            {
                playerScripts[i] = null;
            }
        }
    }
}
