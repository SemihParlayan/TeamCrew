using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using XInputDotNetPure;

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
    //Xinput
    public static bool controllersAreSet;
    public static Controller[] controllers = new Controller[4];
    public static Vector2 GetThumbStick(XboxThumbStick stick, int playerNumber = -1)
    {
        if (playerNumber < -1 || playerNumber > controllers.Length - 1)
        {
            Debug.Log("Tried to access controller: " + playerNumber + ", out of range");
            return Vector2.zero;
        }

        if (playerNumber == -1)
        {
            for (int i = 0; i < controllers.Length; i++)
            {
                if (controllers[i] == null)
                    continue;

                if (stick == XboxThumbStick.Left)
                {
                    Vector2 dir = new Vector2(controllers[i].currentState.ThumbSticks.Left.X, controllers[i].currentState.ThumbSticks.Left.Y);
                    if (dir != Vector2.zero)
                        return dir;
                }
                else
                {
                    Vector2 dir = new Vector2(controllers[i].currentState.ThumbSticks.Right.X, controllers[i].currentState.ThumbSticks.Right.Y);
                    if (dir != Vector2.zero)
                        return dir;
                }
            }
        }
        else
        {
            if (controllers[playerNumber] != null)
            {
                if (stick == XboxThumbStick.Left)
                {
                    return new Vector2(controllers[playerNumber].currentState.ThumbSticks.Left.X, controllers[playerNumber].currentState.ThumbSticks.Left.Y);
                }
                else
                {
                    return new Vector2(controllers[playerNumber].currentState.ThumbSticks.Right.X, controllers[playerNumber].currentState.ThumbSticks.Right.Y);
                }
            }
        }

        return Vector2.zero;
    }
    public static bool GetButtonPress(XboxButton button, int playerNumber = -1)
    {
        if (playerNumber < -1 || playerNumber > controllers.Length - 1)
        {
            Debug.Log("Tried to access controller: " + playerNumber + ", out of range");
            return false;
        }

        if (playerNumber == -1)
        {
            for (int i = 0; i < controllers.Length; i++)
            {
                if (controllers[i] != null)
                {
                    if (controllers[i].GetButtonPress(button))
                    {
                        return true;
                    }
                }
            }
        }
        else
        {
            if (controllers[playerNumber] != null)
            {
                return controllers[playerNumber].GetButtonPress(button);
            }
        }

        return false;
    }
    public static bool GetButtonRelease(XboxButton button, int playerNumber = -1)
    {
        if (playerNumber < -1 || playerNumber > controllers.Length - 1)
        {
            Debug.Log("Tried to access controller: " + playerNumber + ", out of range");
            return false;
        }

        if (playerNumber == -1)
        {
            for (int i = 0; i < controllers.Length; i++)
            {
                if (controllers[i] != null)
                {
                    if (controllers[i].GetButtonRelease(button))
                    {
                        return true;
                    }
                }
            }
        }
        else
        {
            if (controllers[playerNumber] != null)
            {
                return controllers[playerNumber].GetButtonRelease(button);
            }
        }

        return false;
    }
    public static bool GetGrip(int player, GripSide button = GripSide.Both)
    {
        if (controllers[player] == null)
            return false;
        Controller c = controllers[player];

        switch (button)
        {
            case GripSide.Left:
                return (c.currentState.Buttons.LeftShoulder == ButtonState.Pressed || c.currentState.Triggers.Left > 0);

            case GripSide.Right:
                return (c.currentState.Buttons.RightShoulder == ButtonState.Pressed || c.currentState.Triggers.Right > 0);

            case GripSide.Both:
                {
                    bool gripped = false;
                    if (c.currentState.Buttons.LeftShoulder == ButtonState.Pressed || c.currentState.Buttons.RightShoulder == ButtonState.Pressed)
                        gripped = true;
                    if (c.currentState.Triggers.Left > 0 || c.currentState.Triggers.Right > 0)
                        gripped = true;

                    return gripped;
                }

            default:
                return false;
        }
    }

    private void TurnOffVibration()
    {
        GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
        GamePad.SetVibration(PlayerIndex.Two, 0f, 0f);
        GamePad.SetVibration(PlayerIndex.Three, 0f, 0f);
        GamePad.SetVibration(PlayerIndex.Four, 0f, 0f);
    }
    
    
    //Static variables and properties
    public static bool GetCheatButton()
    {
        if (!Hacks)
            return false;

        return (GetButtonPress(XboxButton.Back));
    }
    public static bool Hacks;
    public static bool Xbox;
    public static bool PS4;
    public static bool DigitalInput;

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
    public bool ps4 = false;
    public bool digitalInput = false;

    private GameObject fireWorks;

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
    [HideInInspector]
    public bool[] frogsReady = new bool[4];
    public EndgameScreen endGameScreen;
    public ReadySetGo readySetGo;
    private ConnectFrogs connectFrogs;

    private bool playerFinalStretchAnimation = true;
    private Transform topFrogPrefab;
    private int victoryFrogNumber;

    [HideInInspector]
    public string singlePlayerStarted;
    public bool IsInMultiplayerMode { get { return (singlePlayerStarted == string.Empty); } }
    private bool hangingFrogsSpawned;

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
		
        //Aquire tutorialBubbles script
        tutorialBubbles = GetComponent<TutorialBubbles>();
        if (tutorialBubbles == null)
        {
            Debug.LogError("Can't find a TutorialBubbles script on GameManager object");
        }

        bandageManager = GetComponent<BandageManager>();
        connectFrogs = GetComponent<ConnectFrogs>();

        readySetGo = GetComponent<ReadySetGo>();

        //Enable menu music
        menuMusicController.Play();
        menuMusicController.ChangeFadeState(FadeState.IN);
	}

    //Update method
    void Update()
    {
        if (GetButtonPress(XboxButton.Guide))
        {
            TurnOffVibration();
        }

        UpdateControllers();
        RestartGame();
        CheckForTutorialComplete();
        //CheckForCameraPanComplete();
        CheckForFinalStretch();
        //CheckForPlayersReadyInMenu();
        SetPlayerScripts();
    }


    //Single use methods
    public void SpawnHangingFrogs()
    {
        if (hangingFrogsSpawned)
            return;
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
        Vector3 topPosition = generatorScript.GetTopPosition() - new Vector3(0, 2.7f, 0);
        Transform previousFrog = null;
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

            hangingFrogs.Add(previousFrog);
            spawnedFrogs++;
        }

        DestroyFrogs();
        SpawnTopFrog();
    }

    /// <summary>
    /// Deletes old frogs and creates either one or two new frogs depending on which game mode the game is currently in.
    /// </summary>
    public void CreateNewFrogs()
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
                players[i] = (Instantiate(respawnScript.respawnScripts[i].prefab, spawnPosition, Quaternion.identity) as Transform).FindChild("body");
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
        topfrogSpawnerScript.RemoveFrog();
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
        finalStretchMusic.SetStarted(false);
        finalStretchMusic.SetFadeState(FadeState.OUT);

        ResetGameVariables();

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
        hangingFrogsSpawned = false;

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
        tutorialBubbles.Enable(playerScripts);

        readySetGo.lights = generatorScript.GetReadySetGoSpriteRenderes();
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
    public void UpdateControllers()
    {
        //SET CONTROLLERS
        if (!controllersAreSet)
        {
            for (int i = 0; i < 4; ++i)
            {
                PlayerIndex player = (PlayerIndex)(i);
                GamePadState state = GamePad.GetState(player);
                if (state.IsConnected)
                {
                    controllers[i] = new Controller(state, player);
                    Debug.Log("GamePad '" + player.ToString() + "' found & registered!");
                }
            }
            controllersAreSet = true;
        }



        //UPDATE CONTROLLERS
        for (int i = 0; i < controllers.Length; i++)
        {
            if (controllers[i] == null)
                continue;
            Controller c = controllers[i];

            //Set previous state
            c.previousState = c.currentState;

            //Get new state
            c.currentState = GamePad.GetState(c.player);
        }

    }

    /// <summary>
    /// Reloads the current scene loaded.
    /// </summary>
    private void RestartGame()
    {
        if (!Hacks)
            return;

        //Restart game with Xbox 360 Controller
        if (GameManager.Xbox)
        {
            if (GetButtonPress(XboxButton.Y))
                Application.LoadLevel(Application.loadedLevel);
        }
        ////Restart game with PS4 controller
        //else if (GameManager.PS4)
        //{
        //    if (Input.GetButtonDown("StartPS"))
        //        Application.LoadLevel(Application.loadedLevel);
        //}
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
            if (!playerFinalStretchAnimation)
            { 
                playerFinalStretchAnimation = true;
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

    /// <summary>
    /// This methods quits the whole game.
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }
}
