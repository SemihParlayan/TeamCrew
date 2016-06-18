using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Steamworks;

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
    public GameObject entriesParent;

	//privates
    private GameManager gameManager;
    private SteamLeaderboardManager leaderboardManager;
    private List<LeaderboardEntry> entries;
    private int dailySeed = 50;
    private int entryMinIndex = 0;
    private bool canScroll = true;
    private float scrollDelay = 0.065f;
    private bool mountainGenerated;
    private LeaderboardEntry clientEntry;


	//Unity methods
    void Awake()
    {
        Random.seed = dailySeed;
        gameManager = GameObject.FindObjectOfType<GameManager>();
        leaderboardManager = GameObject.FindObjectOfType<SteamLeaderboardManager>();
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
    public IEnumerator RefreshLeaderboards(bool fetchNewEntries)
    {
        if (leaderboardManager == null)
            yield return null;

        if (fetchNewEntries || entries == null || entries.Count == 0)
        {
            loadIcon.gameObject.SetActive(true);
            entriesParent.gameObject.SetActive(false);

            entries = new List<LeaderboardEntry>();
            LeaderboardEntries entriesObj = new LeaderboardEntries();
            entriesObj.entries = entries;
            StartCoroutine(leaderboardManager.GetLeaderboardEntries(0f, entriesObj));

            yield return new WaitForSeconds(3f);
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

    public override void OnSwitchedTo()
    {
        base.OnSwitchedTo();
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
        StartCoroutine(RefreshLeaderboards(true));

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
            //Activate menu mountain
            poff.SetMenuMountainState(true, 0f);

            //Remove old frogs
            gameManager.DestroyFrogs();

            //Set which frogs are ready
            bool[] frogsReady = new bool[4] { false, false, false, false };
            gameManager.frogsReady = frogsReady;
            gameManager.DestroyTopFrog();

            //Destroy daily mountain
            gameManager.DestroyCurrentLevel(true);
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
        tmp.totalSeconds = entries[fakeRank].totalSeconds;


        entries[fakeRank].globalRank = fakeRank + 1;
        entries[fakeRank].isClient = entries[1].isClient;
        entries[fakeRank].name = entries[1].name;
        entries[fakeRank].totalSeconds = entries[1].totalSeconds;

        entries[1].globalRank = 2;
        entries[1].isClient = tmp.isClient;
        entries[1].name = tmp.name;
        entries[1].totalSeconds = tmp.totalSeconds;
    }
    private void AddFakeEntries(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            LeaderboardEntry prev = entries.Last();
            LeaderboardEntry e = new LeaderboardEntry();
            e.globalRank = prev.globalRank + 1;
            e.totalSeconds = 50 + 10 * i;
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
                return clientEntry = entries[i];
            }
        }
        return null;
    }
}
