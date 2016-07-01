using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
using UnityEngine.UI;

public class DailyMountainScreen : M_Screen 
{
	//publics
    public List<SteamUI_LeaderboardEntry> uiEntries = new List<SteamUI_LeaderboardEntry>();
    public TextMesh dateText;
    public GameModes gameModes;
    public PoffMountain poff;
    public Transform scrollKnob;
    public GameObject scrollParent;
    public GameObject loadIcon;
    public GameObject couldNotConnectProperlyText;
    public GameObject entriesParent;
    public GameObject currentTimeObject;
    public Text previousTimeObject;
    public DailyMountainGameMode dailyGamemode;
    public DailyEndgameScreen dailyEndgameScreen;
    public GameScreen gameScreen;

	//privates
    private GameManager gameManager;
    private SteamLeaderboardManager leaderboardManager;
    private List<LeaderboardEntry> entries;
    private int dailySeed = 50;
    private int entryMinIndex = 0;
    private bool canScroll = true;
    private float scrollDelay = 0.065f;
    private bool mountainGenerated;
    public LeaderboardEntry clientEntry;
    private bool canStart;
    private bool hasFoundClient;


	//Unity methods
    void Awake()
    {
        canStart = false;
        Random.seed = dailySeed;
        gameManager = GameObject.FindObjectOfType<GameManager>();
        leaderboardManager = GameObject.FindObjectOfType<SteamLeaderboardManager>();
        previousTimeObject.gameObject.SetActive(false);
    }
    protected override void OnStart()
    {
        base.OnStart();
        System.DateTime time = CompleteLevel.currentDateTime;
        string text = string.Empty;
        text += time.DayOfWeek + " - ";
        text += time.Date.Day + "/";
        text += time.Date.Month + "/";
        text += time.Date.Year;
        dateText.text = text;
        Debug.Log(dateText);
    }
    protected override void OnUpdate()
    {
        base.OnUpdate();

        ScrollHighscores();
    }

	//public methods
    public void RefreshLeaderboards()
    {
        if (leaderboardManager == null)
            return;

        loadIcon.gameObject.SetActive(true);
        entriesParent.gameObject.SetActive(false);
        couldNotConnectProperlyText.SetActive(false);

        entries = new List<LeaderboardEntry>();
        LeaderboardEntries entriesObj = new LeaderboardEntries();
        entriesObj.entries = entries;

        leaderboardManager.GetLeaderboardEntries(entriesObj, OnEntriesCompleteMethod, OnEntriesFailedMethod);
    }
    private void SimpleRefresh()
    {
        if (leaderboardManager == null || entries == null || entries.Count <= 0)
            return;

        int startIndex = entryMinIndex;
        int endIndex = entryMinIndex + uiEntries.Count;

        if (startIndex < 0)
        {
            startIndex = 0;
        }
        if (endIndex > entries.Count)
        {
            endIndex = entries.Count;
        }

        for (int i = 0; i < uiEntries.Count; i++)
        {
            SteamUI_LeaderboardEntry uiEntry = uiEntries[i];
            int index = startIndex + i;
            LeaderboardEntry steamEntry = null;
            if (index < entries.Count)
                steamEntry = entries[startIndex + i];

            if (uiEntry == null)
                continue;

            uiEntry.SetInfo(steamEntry);
        }
    }
    private void OnEntriesCompleteMethod()
    {
        if (this.enabled)
        {
            loadIcon.gameObject.SetActive(false);
            entriesParent.gameObject.SetActive(true);
            GenerateMountain();

            //Add fake entries
            AddFakeEntries(57);

            //Enter client at fake rank
            //SetFakeRank(47);

            //Find client entry
            clientEntry = GetClientEntry();

            //Set scroll line options
            if (entries.Count > uiEntries.Count)
            {
                scrollParent.SetActive(true);
            }

            //Scroll to client entry
            ScrollToClient();

            //Ready to start the game
            canStart = true;
        }

        int startIndex = entryMinIndex;
        int endIndex = entryMinIndex + uiEntries.Count;

        if (startIndex < 0)
        {
            startIndex = 0;
        }
        if (endIndex > entries.Count)
        {
            endIndex = entries.Count;
        }

        for (int i = 0; i < uiEntries.Count; i++)
        {
            SteamUI_LeaderboardEntry uiEntry = uiEntries[i];
            int index = startIndex + i;
            LeaderboardEntry steamEntry = null;
            if (index < entries.Count)
                steamEntry = entries[startIndex + i];

            if (uiEntry == null)
                continue;

            uiEntry.SetInfo(steamEntry);
        }
    }
    private void OnEntriesFailedMethod()
    {
        Debug.Log("Någonting sket sig");
        loadIcon.gameObject.SetActive(false);
        couldNotConnectProperlyText.SetActive(true);
    }
    public bool UploadHighscoreToLeaderboard(Timer newEntry, SteamLeaderboardManager.OnUploadComplete completeMethod, SteamLeaderboardManager.OnUploadFailed failedMethod)
    {
        if (leaderboardManager == null || newEntry == null)
            return false;

        if (!hasFoundClient)
        {
            leaderboardManager.UploadTimeToLeaderboard(newEntry.milliSeconds, completeMethod, failedMethod);
            return true;
        }
        else
        {
            if (clientEntry != null && newEntry.HasBetterTimeThan(clientEntry.timer))
            {
                leaderboardManager.UploadTimeToLeaderboard(newEntry.milliSeconds, completeMethod, failedMethod);
                return true;
            }
        }

        return false;
    }

    public override void OnSwitchedTo()
    {
        base.OnSwitchedTo();
        //Can not start the game until leaderboards are ready
        canStart = false;

        //Has not found a client entry
        hasFoundClient = false;

        //Set gamemode to random daily gamemode
        GameMode dailyMode = gameModes.GetRandomDailyGameMode();
        if (dailyMode == null)
            return;

        GameManager.CurrentGameMode = dailyMode;

        //Lock parallaxes
        gameManager.LockParallaxes(true);

        //Set which frogs are ready
        bool[] frogsReady = new bool[4] { true, false, false, false };
        gameManager.frogsReady = frogsReady;

        //Aquire leaderboard
        entryMinIndex = 0;
        RefreshLeaderboards();

        //Flag generated mountain
        mountainGenerated = false;


        //Disable scroll line
        scrollParent.SetActive(false);
    }
    public override void OnSwitchedFrom()
    {
        base.OnSwitchedFrom();

        //Unlock parallaxes
        gameManager.LockParallaxes(false);

        if (M_ScreenManager.GetCurrentScreen() is StartScreen)
        {
            //Stop refresh leaderboard coroutine
            StopAllCoroutines();

            //Activate menu mountain
            poff.SetMenuMountainState(true, 0f);

            //Remove old frogs
            gameManager.DestroyFrogs();

            //Set which frogs are ready
            bool[] frogsReady = new bool[4] { false, false, false, false };
            gameManager.frogsReady = frogsReady;
            gameManager.DestroyTopFrog();

            //Destroy daily mountain
            if (mountainGenerated)
                gameManager.DestroyCurrentLevel(true);

            leaderboardManager.ResetGettingLeaderboards();
        }
        else
        {
            if (hasFoundClient && clientEntry != null)
            {
                previousTimeObject.gameObject.SetActive(true);
                previousTimeObject.text = clientEntry.timer.GetTimeString();
            }
        }
    }

    public void OnPlayGame()
    {
        if (canStart)
        {
            gameManager.isInDailyMountain = true;
            currentTimeObject.SetActive(true);
            dailyGamemode.OnPlayGame();

            M_ScreenManager.SwitchScreen(gameScreen);
        }
    }

	//private methods
    private void ScrollHighscores()
    {
        if (entries == null || entries.Count <= 0 || !mountainGenerated || !canScroll)
            return;

        Vector2 stick = GameManager.GetThumbStick(XboxThumbStick.Left, -1);
        if (stick.y == 0)
        {
            stick = GameManager.GetDPad(-1);
            if (stick.y == 0)
            {
                stick = new Vector2(0, Input.GetAxis("Vertical"));
            }
        }

        if (stick.y <= 0.5f && stick.y >= -0.5f)
            return;

        int dir = (stick.y > 0) ? -1 : 1;
        Scroll(dir);
    }
    private void Scroll(int dir)
    {
        canScroll = false;
        Invoke("ResetScroll", scrollDelay);

        entryMinIndex += dir;

        int maxIndex = Mathf.Clamp(entries.Count - uiEntries.Count, 0 , int.MaxValue);
        if (entryMinIndex < 0)
        {
            entryMinIndex = 0;
        }
        else if (entries.Count > 0)
        {

            if (entryMinIndex > maxIndex)
            {
                entryMinIndex = maxIndex;
            }
        }
        SimpleRefresh();

        //Show client entry at all times
        if (clientEntry != null)
        {
            int clientRank = clientEntry.globalRank;
            LeaderboardEntry topEntry = uiEntries.First().entry;
            LeaderboardEntry botEntry = uiEntries.Last().entry;

            if (topEntry != null && botEntry != null)
            {
                int topRank = topEntry.globalRank;
                int botRank = botEntry.globalRank;

                if (clientRank < topRank)
                {
                    uiEntries.First().SetInfo(clientEntry);
                }
                else if (clientRank > botRank)
                {
                    uiEntries.Last().SetInfo(clientEntry);
                }
            }
        }

        //Set scroll knob
        if (scrollParent.activeInHierarchy)
        {
            float height = 1.426f * 2;
            float topPos = height / 2;

            float percent = (maxIndex == 0) ? 0 : (float)entryMinIndex / (float)maxIndex;
            float newPos = topPos - (height * percent);

            Vector3 pos = scrollKnob.transform.localPosition;
            pos.x = newPos;
            scrollKnob.transform.localPosition = pos;
        }
    }
    private void ScrollToClient()
    {
        if (clientEntry == null)
            return;

        int targetRank = clientEntry.globalRank - 5;
        int safety = 0;
        while(entryMinIndex < targetRank)
        {
            safety++;
            if (safety > 1000)
                return;
            Scroll(1);
        }
    }
    private void ResetScroll()
    {
        canScroll = true;
    }
    
    private void GenerateMountain()
    {
        //Flag generated mountain
        mountainGenerated = true;

        //Generate a new mountain with the daily mode
        poff.SetMenuMountainState(false, 0f);
        poff.PoffRepeating(dailySeed);

        //Create new frogs
        gameManager.CreateNewFrogs(0);
        gameManager.DestroyTopFrog();
    }
    private void SetFakeRank(int fakeRank)
    {
        LeaderboardEntry tmp = new LeaderboardEntry();
        tmp.globalRank = entries[fakeRank].globalRank;
        tmp.isClient = entries[fakeRank].isClient;
        tmp.name = entries[fakeRank].name;
        tmp.timer = entries[fakeRank].timer;


        entries[fakeRank].globalRank = fakeRank + 1;
        entries[fakeRank].isClient = entries[1].isClient;
        entries[fakeRank].name = entries[1].name;
        entries[fakeRank].timer = entries[1].timer;

        entries[1].globalRank = 2;
        entries[1].isClient = tmp.isClient;
        entries[1].name = tmp.name;
        entries[1].timer = tmp.timer;
    }
    private void AddFakeEntries(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            LeaderboardEntry prev = entries.Last();
            LeaderboardEntry e = new LeaderboardEntry();
            e.globalRank = prev.globalRank + 1;
            e.timer = new Timer(5000 + (1000 * i));
            e.name = "test";
            if (i == amount - 1)
                e.globalRank = 1337;
            entries.Add(e);
        }
    }
    private LeaderboardEntry GetClientEntry()
    {
        for (int i = 0; i < entries.Count; i++)
        {
            if (entries[i].isClient)
            {
                hasFoundClient = true;
                return clientEntry = entries[i];
            }
        }
        return null;
    }
}
