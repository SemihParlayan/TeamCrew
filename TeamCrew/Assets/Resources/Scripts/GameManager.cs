using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using XInputDotNetPure;
using UnityEngine.SceneManagement;
using Rewired;

public enum GripSide
{
    Both,
    Left,
    Right
}
public enum XboxButton
{
    A,
    B,
    Back,
    Guide,
    LeftShoulder,
    LeftStick,
    RightShoulder,
    RightStick,
    Start,
    X,
    Y
}
public enum XboxThumbStick
{
    Left,
    Right
}
public class Controller
{
    public PlayerIndex player;
    public GamePadState currentState;
    public GamePadState previousState;

    public Controller(GamePadState state, PlayerIndex player)
    {
        this.currentState = state;
        this.player = player;
    }

    public bool GetButtonPress(XboxButton button)
    {
        switch (button)
        {
            case XboxButton.A:
                return previousState.Buttons.A == ButtonState.Released && currentState.Buttons.A == ButtonState.Pressed;
            case XboxButton.B:
                return previousState.Buttons.B == ButtonState.Released && currentState.Buttons.B == ButtonState.Pressed;
            case XboxButton.Back:
                return previousState.Buttons.Back == ButtonState.Released && currentState.Buttons.Back == ButtonState.Pressed;
            case XboxButton.Guide:
                return previousState.Buttons.Guide == ButtonState.Released && currentState.Buttons.Guide == ButtonState.Pressed;
            case XboxButton.LeftShoulder:
                return previousState.Buttons.LeftShoulder == ButtonState.Released && currentState.Buttons.LeftShoulder == ButtonState.Pressed;
            case XboxButton.LeftStick:
                return previousState.Buttons.LeftStick == ButtonState.Released && currentState.Buttons.LeftStick == ButtonState.Pressed;
            case XboxButton.RightShoulder:
                return previousState.Buttons.RightShoulder == ButtonState.Released && currentState.Buttons.RightShoulder == ButtonState.Pressed;
            case XboxButton.RightStick:
                return previousState.Buttons.RightStick == ButtonState.Released && currentState.Buttons.RightStick == ButtonState.Pressed;
            case XboxButton.Start:
                return previousState.Buttons.Start == ButtonState.Released && currentState.Buttons.Start == ButtonState.Pressed;
            case XboxButton.X:
                return previousState.Buttons.X == ButtonState.Released && currentState.Buttons.X == ButtonState.Pressed;
            case XboxButton.Y:
                return previousState.Buttons.Y == ButtonState.Released && currentState.Buttons.Y == ButtonState.Pressed;
            default:
                return false;
        }
    }
    public bool GetButtonRelease(XboxButton button)
    {
        switch (button)
        {
            case XboxButton.A:
                return previousState.Buttons.A == ButtonState.Pressed && currentState.Buttons.A == ButtonState.Released;
            case XboxButton.B:
                return previousState.Buttons.B == ButtonState.Pressed && currentState.Buttons.B == ButtonState.Released;
            case XboxButton.Back:
                return previousState.Buttons.Back == ButtonState.Pressed && currentState.Buttons.Back == ButtonState.Released;
            case XboxButton.Guide:
                return previousState.Buttons.Guide == ButtonState.Pressed && currentState.Buttons.Guide == ButtonState.Released;
            case XboxButton.LeftShoulder:
                return previousState.Buttons.LeftShoulder == ButtonState.Pressed && currentState.Buttons.LeftShoulder == ButtonState.Released;
            case XboxButton.LeftStick:
                return previousState.Buttons.LeftStick == ButtonState.Pressed && currentState.Buttons.LeftStick == ButtonState.Released;
            case XboxButton.RightShoulder:
                return previousState.Buttons.RightShoulder == ButtonState.Pressed && currentState.Buttons.RightShoulder == ButtonState.Released;
            case XboxButton.RightStick:
                return previousState.Buttons.RightStick == ButtonState.Pressed && currentState.Buttons.RightStick == ButtonState.Released;
            case XboxButton.Start:
                return previousState.Buttons.Start == ButtonState.Pressed && currentState.Buttons.Start == ButtonState.Released;
            case XboxButton.X:
                return previousState.Buttons.X == ButtonState.Pressed && currentState.Buttons.X == ButtonState.Released;
            case XboxButton.Y:
                return previousState.Buttons.Y == ButtonState.Pressed && currentState.Buttons.Y == ButtonState.Released;
            default:
                return false;
        }
    }
}
public class GameManager : MonoBehaviour
{
    public static Player defaultPlayer;
    private static Player[] rewiredPlayers = new Player[4];
    public static Player GetPlayer(int id)
    {
        if (id == -1)
            return defaultPlayer;
        else if (id >= 0 && id < rewiredPlayers.Length)
            return rewiredPlayers[id];
        else return null;
    }
    public static bool GetGrip(int player, GripSide button = GripSide.Both)
    {
        if (player < 0 || player > rewiredPlayers.Length - 1)
            return false;

        switch (button)
        {
            case GripSide.Left:
                return (rewiredPlayers[player].GetButton("LeftShoulder") || rewiredPlayers[player].GetAxis("LeftTrigger") > 0);

            case GripSide.Right:
                return (rewiredPlayers[player].GetButton("RightShoulder") || rewiredPlayers[player].GetAxis("RightTrigger") > 0);

            case GripSide.Both:
                {
                    bool gripped = false;
                    if ((rewiredPlayers[player].GetButton("LeftShoulder") || rewiredPlayers[player].GetAxis("LeftTrigger") > 0))
                        gripped = true;
                    if ((rewiredPlayers[player].GetButton("RightShoulder") || rewiredPlayers[player].GetAxis("RightTrigger") > 0))
                        gripped = true;

                    return gripped;
                }

            default:
                return false;
        }
    }

    public static void PlayAudioSource(AudioSource source, float minPitch = 1f, float maxPitch = 1f, bool overrideIsPlaying = true)
    {
        if (source == null)
            return;

        float previousPitch = source.pitch;
        float pitch = Random.Range(minPitch, maxPitch);

        source.pitch = pitch;
        if (overrideIsPlaying)
        {
            source.Play();
        }
        else if (!source.isPlaying)
        {
            source.Play();
        }
    }
    public static AudioSource PlayClipAtPoint(AudioClip clip, Vector3 position)
    {
        position.z = 0;

        GameObject tmp = new GameObject("GameManager_OneShotAudio");
        tmp.transform.position = position;

        AudioSource source = tmp.AddComponent<AudioSource>();
        source.clip = clip;
        source.Play();

        Destroy(tmp, clip.length);
        return source;
    }
    
    
    //Static variables and properties
    public static bool GetCheatButton()
    {
        if (!Hacks)
            return false;

        return defaultPlayer.GetButtonDown("Button Y");
    }
    public static bool Hacks;
    public static bool Xbox;
    public static bool DigitalInput;
    public static bool ReturnToMenuWhenInactive;
    public static bool UseMouseAsInput;
    public static int DailyMountainPlayerID;
    public static bool AllowSounds = false;

    public static Transform[] players = new Transform[4];
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
    public static GameMode CurrentGameMode;

    //////////////////////////


    public bool designTestingEnabled;
    [HideInInspector] public bool gameActive;
    [HideInInspector] public bool tutorialComplete;
    public bool hacks = true;
    public bool xbox = false;
    public bool disableMouseCursor = false;
    public bool digitalInput = false;
    public bool useMouseAsInput = false;
    public bool returnMenuInactive = true;

    private GameObject fireWorks;

    public GameModifiers gameModifier;
    private List<Transform> hangingFrogs = new List<Transform>();
    private BandageManager bandageManager;
    public InactivityController inactivityController;
    public PoffMountain poffMountainScript;
    private TutorialBubbles tutorialBubbles;
    private TopFrogSpawner topfrogSpawnerScript;
    private LevelGeneration generatorScript;
    private Respawn respawnScript;
    private CameraFollow cameraFollowScript;
    public MenuMusicController menuMusicController;
    public FinalMusic finalStretchMusic;
    public Animator finalStretch;
    public FrogPrototype[] playerScripts = new FrogPrototype[4];
    //[HideInInspector]
    public bool[] frogsReady = new bool[4];
    public EndgameScreen endGameScreen;
    public DailyEndgameScreen dailyEndGameScreen;
    public ReadySetGo readySetGo;
    private ConnectFrogs connectFrogs;
    private TopNumbers topNumbers;
    private TopStats topstats;//seb
    [HideInInspector]
    public List<Transform> transformOrder = new List<Transform>();
    public Transform frontParallaxes;

    private bool playedFinalStretchAnimation = true;
    private Transform topFrogPrefab;
    private int victoryFrogNumber;

    [HideInInspector]
    public string singlePlayerStarted;
    //[HideInInspector]
    public bool winIsCalled;
    public bool IsInMultiplayerMode { get { return (singlePlayerStarted == string.Empty); } }
    public bool isInDailyMountain;
    private bool hangingFrogsSpawned;
    private Vector4 playerEndPlacements; //Seb. This is what position the players have. Needed for the stats screen. 
    

    void Awake()
    {
        Debug.Log("GameManager Awake Called");

        //Set controllers
        rewiredPlayers[0] = ReInput.players.GetPlayer("Player0");
        rewiredPlayers[1] = ReInput.players.GetPlayer("Player1");
        rewiredPlayers[2] = ReInput.players.GetPlayer("Player2");
        rewiredPlayers[3] = ReInput.players.GetPlayer("Player3");
        defaultPlayer = ReInput.players.GetPlayer("DefaultPlayer");

        foreach (Joystick j in ReInput.controllers.Joysticks)
        {
            defaultPlayer.controllers.AddController(j, false);
        }

        ReInput.ControllerConnectedEvent += OnControllerConnected;
        ReInput.ControllerPreDisconnectEvent += OnControllerPreDisconnect;


        //Set static variables to their coresponding public property.
        Xbox = xbox;
        Hacks = hacks;
        DigitalInput = digitalInput;
        ReturnToMenuWhenInactive = returnMenuInactive;
        UseMouseAsInput = useMouseAsInput;

        //Activate design testing
        if (designTestingEnabled)
        {
            gameActive = true;
            tutorialComplete = true;
        }
    }
    void OnControllerConnected(ControllerStatusChangedEventArgs args)
    {
        Joystick joystick = ReInput.controllers.GetJoystick(args.controllerId);

        if (!defaultPlayer.controllers.ContainsController(joystick))
        {
            defaultPlayer.controllers.AddController(joystick, false);
        }
    }
    void OnControllerPreDisconnect(ControllerStatusChangedEventArgs args)
    {
        Joystick joystick = ReInput.controllers.GetJoystick(args.controllerId);

        if (defaultPlayer.controllers.ContainsController(joystick))
        {
            defaultPlayer.controllers.RemoveController(joystick);
        }
    }
	void Start ()
    {
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
		
        //Aquire tutorialBubbles script
        tutorialBubbles = GetComponent<TutorialBubbles>();
        if (tutorialBubbles == null)
        {
            Debug.LogError("Can't find a TutorialBubbles script on GameManager object");
        }

        bandageManager = GetComponent<BandageManager>();
        connectFrogs = GetComponent<ConnectFrogs>();

        readySetGo = GetComponent<ReadySetGo>();
        topNumbers = GetComponent<TopNumbers>();
        topstats = GetComponent<TopStats>();//seb

        //Enable menu music
        menuMusicController.Play();
        menuMusicController.ChangeFadeState(FadeState.IN);


        //Disable mouse cursor
        if (disableMouseCursor)
        {
            Cursor.visible = false;
        }

        //Load player prefs
        Invoke("LoadPlayerPrefs", 0.2f);
	}

    //Update method
    void Update()
    {
        Application.targetFrameRate = 120;
        CheckForTutorialComplete();
        CheckForFinalStretch();
        SetPlayerScripts();
    }


    //Single use methods
    public void ActivateTopNumbers()
    {
        topstats.showStats();//seb

        if (GetFrogReadyCount() > 1)
            topNumbers.ActivateNumbers(transformOrder.ToArray());
    }
    public void DeActivateNumbers()
    {
        topstats.hideStats();//seb
        topNumbers.DeActivateNumbers();
    }



    public void SpawnHangingFrogs()
    {
        if (hangingFrogsSpawned)
            return;

        KOTH koth = gameModifier.GetComponent<KOTH>();
        hangingFrogsSpawned = true;

        List<FrogPrototype> order = new List<FrogPrototype>();
        List<int> playersAdded = new List<int>();

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == null)
                continue;

            order.Add(players[i].GetComponent<FrogPrototype>());
            playersAdded.Add(players[i].GetComponent<FrogPrototype>().player);
        }



        //Sort by Y value
        if (koth.enabled)
        {
            bool changed = true;
            while (changed)
            {
                changed = false;
                for (int i = 0; i < order.Count - 1; i++)
                {
                    ScoreKeeper keeper = koth.keepers[order[i].player];
                    ScoreKeeper nextKeeper = koth.keepers[order[i + 1].player];

                    if (nextKeeper.targetScore > keeper.targetScore)
                    {
                        FrogPrototype tmp = order[i];
                        order[i] = order[i + 1];
                        order[i + 1] = tmp;
                        changed = true; 
                    }
                }
            }
        }
        else
        {
            bool changed = true;
            while (changed)
            {
                changed = false;
                for (int i = 0; i < order.Count - 1; i++)
                {
                    float y = order[i].transform.position.y;
                    float nY = order[i + 1].transform.position.y;

                    if (nY > y)
                    {
                        FrogPrototype tmp = order[i];
                        order[i] = order[i + 1];
                        order[i + 1] = tmp;
                        changed = true;


                    }
                }
            }
        }


        //seb
        //order is sorted according to the frog's victory order. Here I sort playerEndPlacements. 
        //Idea for playerEndPlacements is that position 0 represents player 1, pos 2 player 2 and so on. Numbers represent their end rank in the latest race. This is so that I can display top stats more easily. 
        //This sets playerEndPlacements to be accurate. 
        for (int i=0; i<order.Count;i++)
        {
            //playerEndPlacements[0] = order[i].player;
            //This is wrong. Investigate. 
            switch (order[i].player)
            {
                case 0:
                    playerEndPlacements[0] = i;
                    break;
                case 1:
                    playerEndPlacements[1] = i;
                    break;
                case 2:
                    playerEndPlacements[2] = i;
                    break;
                case 3:
                    playerEndPlacements[3] = i;
                    break;
            }

        }
        //end of seb 


        for (int i = 0; i < frogsReady.Length; i++)
        {
            if (frogsReady[i])
            {
                FrogPrototype frog = respawnScript.respawnScripts[i].prefab.FindChild("body").GetComponent<FrogPrototype>();

                if (!playersAdded.Contains(frog.player))
                {
                    order.Add(frog);
                }
                
            }
        }


        int spawnedFrogs = 0;
        Vector3 topPosition = generatorScript.GetTopPosition() - new Vector3(0.5f, 2.7f, 0);
        Transform previousFrog = null;
        transformOrder.Clear();

        if (koth.enabled)
        {
            this.topFrogPrefab = respawnScript.respawnScripts[order[0].player].prefab.GetComponentInChildren<FrogPrototype>().topPrefab;
            this.victoryFrogNumber = order[0].player;
        }
        for (int i = 0; i < order.Count; i++)
        {
            FrogPrototype frog = order[i];

            if (frog.player == victoryFrogNumber)
                continue;

            if (spawnedFrogs == 0)
            {
                previousFrog = connectFrogs.SpawnFrog(respawnScript.respawnScripts[frog.player].prefab, topPosition, false);
                Transform body = previousFrog.FindChild("body");
                FrogPrototype newFrog = body.GetComponent<FrogPrototype>();
                newFrog.leftGripScript.LockHand(int.MaxValue);
                newFrog.leftGripScript.SetForcedGrip(true, false);
            }
            else
            {
                Hand hand = (i % 2 == 0) ? Hand.Right : Hand.Left;
                previousFrog = connectFrogs.SpawnFrogConnected(respawnScript.respawnScripts[frog.player].prefab, previousFrog, hand, VersusGripPoint.Foot);
            }

            Transform b = previousFrog.FindChild("body");
            transformOrder.Add(b);

            hangingFrogs.Add(previousFrog);
            spawnedFrogs++;
        }


        DestroyFrogs();
        SpawnTopFrog();
    }
    public void ResetWinVariable()
    {
        this.winIsCalled = false;
    }

    /// <summary>
    /// Returns true if the any frog has reached final stretch
    /// </summary>
    /// <returns></returns>
    public bool HasReachedFinalStretch()
    {
        return playedFinalStretchAnimation;
    }

    /// <summary>
    /// Deletes old frogs and creates either one or two new frogs depending on which game mode the game is currently in.
    /// </summary>
    public void CreateNewFrogs(int forcedPlayerIndex = -1)
    {
        //Remove old frogs.
        DestroyFrogs();

        //Reset the bandage counter.
        bandageManager.ResetBandages();

        int frogSpawnCount = 0;
        for (int i = 0; i < players.Length; i++)
        {
            if (frogsReady[i])
            {
                frogSpawnCount++;
                Vector3 spawnPosition = generatorScript.GetPlayerSpawnPosition(frogSpawnCount);
                spawnPosition.z = 0;

                if (forcedPlayerIndex != -1)
                {
                    FrogPrototype frog = respawnScript.respawnScripts[forcedPlayerIndex].prefab.GetComponentInChildren<FrogPrototype>();
                    if (frog != null)
                    {
                        frog.player = forcedPlayerIndex;
                        respawnScript.respawnScripts[forcedPlayerIndex].arrow.color = frog.respawnArrowColor;
                    }
                }

                if (forcedPlayerIndex != -1)
                {
                    players[forcedPlayerIndex] = (Instantiate(respawnScript.respawnScripts[forcedPlayerIndex].prefab, spawnPosition, Quaternion.identity) as Transform).FindChild("body");
                }
                else
                {
                    players[i] = (Instantiate(respawnScript.respawnScripts[i].prefab, spawnPosition, Quaternion.identity) as Transform).FindChild("body");
                }
                
                
            }
        }
        readySetGo.ResetLights();
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

    public void DestroyTopFrog()
    {
        if (topfrogSpawnerScript == null)
        {
            topfrogSpawnerScript = GetComponent<TopFrogSpawner>();
        }
        if (topfrogSpawnerScript != null)
        {
            topfrogSpawnerScript.RemoveFrog();
        }
        DestroyHangingFrogs();
    }

    private void DestroyHangingFrogs()
    {
        for (int i = 0; i < hangingFrogs.Count; i++)
        {
            Destroy(hangingFrogs[i].gameObject);
        }

        hangingFrogs.Clear();
    }

    /// <summary>
    /// Sets the tutorial to complete and more such as disabling tutorial bubbles, remove safety lines etc...
    /// </summary>
    /// 
    public void TutorialComplete()
    {
        if (tutorialComplete)
            return;

        //Set flags
        tutorialComplete = true;
        cameraFollowScript.enabled = true;

        //Disable tutorial bubbles;
        tutorialBubbles.Disable(playerScripts);

        //Remove players safety line && unlock frog hands
        for (int i = 0; i < players.Length; i++)
        {
            Transform player = players[i];

            if (player != null)
            {
                //Line
                Line line = player.GetComponent<Line>();
                if (line)
                {
                    line.Remove();
                }

                //Hands
                HandGrip[] handGrips = player.parent.GetComponentsInChildren<HandGrip>();

                foreach (HandGrip grip in handGrips)
                {
                    StartCoroutine(grip.DeLockHand(0f));
                }
            }
        }

        gameModifier.transform.GetComponent<KOTH>().ActivateKeepers();
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
    }

    /// <summary>
    /// Return the frogs final race position . X = PlayerOne race pos. Y = PlayerTwo race pos. 
    /// </summary>
    public Vector4 GetEndPlacements() //function by seb, needed by topstats :) 
    {
        return playerEndPlacements;
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

        if (!isInDailyMountain)
        {
            M_ScreenManager.SwitchScreen(endGameScreen);
            endGameScreen.OnEnter(generatorScript.GetTopPosition(), victoryFrogNumber);
        }
        else
        {
            M_ScreenManager.SwitchScreen(dailyEndGameScreen);
            dailyEndGameScreen.OnEnter(generatorScript.GetTopPosition());
        }
    }

    /// <summary>
    /// Resets all boolean flags needed to return to menu for a safe restart of the game. Enables exit button in menu & removes fly.
    /// </summary>
    public void ResetGameVariables()
    {
        //Enable fall sound
        Camera.main.transform.GetComponent<AudioSource>().enabled = true;
        Camera.main.transform.GetComponent<FallSoundCam>().enabled = true;

        //Reset boolean flags
        cameraFollowScript.enabled = false;
        respawnScript.enabled = false;
        playedFinalStretchAnimation = false;
        tutorialComplete = false;
        gameActive = false;
        isInDailyMountain = false;

        //Reset spawn timers
        respawnScript.ResetRespawns();

        //Remove any active fly
        GetComponent<LadybugSpawner>().RemoveFly();

        inactivityController.ResetGameStartDelay();

    }

    /// <summary>
    /// Enables the menu cycle that shows which frog won, deathcount and sets accessories for the winning frog. Also activates FireWorks.
    /// </summary>
    /// <param name="frogNumber">What frog won the game? 1 or 2?</param>
    public void Win(Transform topfrogPrefab, int victoryFrogNumber)
    {
        if (this.winIsCalled)
            return;
        this.winIsCalled = true;
        this.victoryFrogNumber = victoryFrogNumber;
        this.topFrogPrefab = topfrogPrefab;

        GoBackToMenu();

        //Fade out finalmusic
        finalStretchMusic.SetStarted(false);
        finalStretchMusic.SetFadeState(FadeState.OUT);

        ResetGameVariables();

        //Enable fireworks
        fireWorks.transform.position = generatorScript.GetTopPosition() + new Vector3(0, -18, 0);
        fireWorks.SetActive(true);
        fireWorks.GetComponent<Fireworks>().Reset();

        frontParallaxes.gameObject.SetActive(true);
    }

    /// <summary>
    /// Sets the game to active, meaning that the frogs can move and the tutorial is started.
    /// </summary>
    public void StartGame()
    {
        gameModifier.OnGameStart();
        hangingFrogsSpawned = false;

        //Enables respawning
        respawnScript.enabled = true;
        respawnScript.GameStarting();

        if (isInDailyMountain)
        {
            respawnScript.SetRespawnTime(respawnScript.dailyMountainRespawnTime);
        }
        else
        {
            respawnScript.SetRespawnTime(respawnScript.defaultRespawnTime);
        }

        //Sets the game to be active (Camera pan has just reached the bottom)
        gameActive = true;

        inactivityController.OnCameraReachedBottom();

        //Reset variables
        playedFinalStretchAnimation = false;

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
        tutorialBubbles.Enable(playerScripts);

        readySetGo.lights = generatorScript.GetReadySetGoSpriteRenderes();
        frontParallaxes.gameObject.SetActive(false);
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

    /// <summary>
    /// Activates and deactivates the inactivityController
    /// </summary>
    /// <param name="state"></param>
    public void SetInactivityState(bool state, float newLimit)
    {
        inactivityController.SetActiveValue(state, newLimit);
    }
    public void SetFrogsReadyInactivity()
    {
        inactivityController.SetTimersForFrogs(frogsReady);
    }

    /// <summary>
    /// Destroys the currently active level
    /// </summary>
    /// <param name="keepTutorial"></param>
    public void DestroyCurrentLevel(bool keepTutorial)
    {
        generatorScript.DestroyLevel(keepTutorial);
    }

    /// <summary>
    /// This methods quits the whole game.
    /// </summary>
    public void ExitGame()
    {
        SavePlayerPrefs();
        Application.Quit();
    }

    public void SavePlayerPrefs()
    {
        //Save sound settings
        M_Sounds sounds = GameObject.FindObjectOfType<M_Sounds>();
        if (sounds != null)
        {
            PlayerPrefs.SetFloat("MasterVolume", sounds.masterSlider.value);
            PlayerPrefs.SetFloat("MusicVolume", sounds.musicSlider.value);
            PlayerPrefs.SetFloat("SFXVolume", sounds.sfxSlider.value);
            PlayerPrefs.SetFloat("EnvironmentVolume", sounds.environmentSlider.value);
        }

        PlayerPrefs.SetInt("VSYNC", QualitySettings.vSyncCount);
    }
    public void LoadPlayerPrefs()
    {
        //Load sound settings
        M_Sounds sounds = GameObject.FindObjectOfType<M_Sounds>();
        if (sounds != null)
        {
            float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            float environmentVolume = PlayerPrefs.GetFloat("EnvironmentVolume", 1f);

            sounds.masterSlider.SetValue(masterVolume);
            sounds.musicSlider.SetValue(musicVolume);
            sounds.sfxSlider.SetValue(sfxVolume);
            sounds.environmentSlider.SetValue(environmentVolume);
        }
        AllowSounds = true;

        int vsync = PlayerPrefs.GetInt("VSYNC", 0);
        QualitySettings.vSyncCount = (vsync > 0) ? 1 : 0;
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

    public static float GetClimbedHeight()
    {
        return Mathf.Clamp((Camera.main.transform.position.y / LevelHeight), 0, 1f);
    }

    public void LockParallaxes(bool value)
    {
        for (int i = 0; i < poffMountainScript.parallaxConnections.Length; i++)
        {
            ParallaxConnection c = poffMountainScript.parallaxConnections[i];
            if (c == null)
                continue;

            c.parallax.enabled = !value;

            if (value)
            {
                Vector3 localPos = c.parallax.transform.localPosition;
                localPos.y = c.yValue;
                c.parallax.transform.localPosition = localPos;
            }
            else
            {
                Vector3 localPos = c.parallax.transform.localPosition;
                localPos.y = c.initialYValue;
                c.parallax.transform.localPosition = localPos;
            }
        }
    }


    //Methods called from Update constantly
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

        if (completedTutorialCount != 0 && completedTutorialCount == GetFrogReadyCount())
        {
            readySetGo.StartSequence();
        }
        //Set tutorial complete with HACKS
        else if (GetCheatButton())
        {
            readySetGo.StartSequence();
        }
    }

    /// <summary>
    /// Checks to see if the camera has reached the final stretch marker, if so it activates the final stretch text.
    /// </summary>
    private void CheckForFinalStretch()
    {
        if (!gameActive)
            return;

        //Play finalstretch animation if we have climbed 80% of the mountain.
        bool reachedStretchMarker = (Camera.main.transform.position.y >= LevelHeight - 15f);
        cameraFollowScript.absoluteFinalStretchZoom = reachedStretchMarker;

        if(reachedStretchMarker)
        {
            if (!playedFinalStretchAnimation)
            { 
                playedFinalStretchAnimation = true;
                finalStretch.SetTrigger("Play");
                finalStretchMusic.SetStarted(true);
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
