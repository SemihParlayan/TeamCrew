using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Steamworks;

public class DailyMountainScreen : M_Screen 
{
	//publics
    public List<SteamUI_LeaderboardEntry> uiEntries = new List<SteamUI_LeaderboardEntry>();
    public TextMesh dateText;
    public GameModes gameModes;
    public PoffMountain poff;
    public Transform scrollKnob;
    public GameObject loadIcon;
    public GameObject entriesParent;

	//privates
    private GameManager gameManager;
    private SteamLeaderboardManager leaderboardManager;
    private List<LeaderboardEntry> entries;
    private int dailySeed = 50;
    private int entryMinIndex = 0;
    private bool canScroll = true;
    private float scrollDelay = 0.1f;
    private bool mountainGenerated;


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

            for(int i = 0; i < 17; i++)
            {
                LeaderboardEntry e = entries[Random.Range(0, entries.Count)];
                e.globalRank = 10 + i;
                if (i == 16)
                    e.globalRank = 1337;
                entries.Add(e);
            }
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
            LeaderboardEntry steamEntry = entries[startIndex + i];

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

            if (uiEntry == null)
                continue;

            uiEntry.SetInfo(entries[startIndex + i]);
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

        //Set scroll knob
        float height = 1.426f * 2;
        float topPos = height / 2;

        float percent = (maxIndex == 0) ? 0 : (float)entryMinIndex / (float)maxIndex;
        float newPos = topPos - (height * percent);

        Vector3 pos = scrollKnob.transform.localPosition;
        pos.x = newPos;
        scrollKnob.transform.localPosition = pos;
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
}
