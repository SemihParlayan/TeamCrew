﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
using UnityEngine.UI;
using System;
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
    public GameScreen gameScreen;
    public LeaderboardEntry clientEntry;
    public TextMesh timeleftForTodayObject;
    public SpriteRenderer mountainNotAvailableOverlay;
    public GameObject noEntriesToday;
    public GameObject noEntriesPreviousDay;
    public GameObject rightArrow;
    public GameObject leftArrow;
    public GameObject timerBackground;
    public M_Sounds soundManager;
    public StartScreen startScreen;
    public AudioSource mountainGenerationSound;

	//privates
    private GameManager gameManager;
    private SteamLeaderboardManager leaderboardManager;
    private List<LeaderboardEntry> entries;
    private int dailySeed = 0;
    private int entryMinIndex = 0;
    private bool canScroll = true;
    private float scrollDelay = 0.065f;
    private bool mountainGenerated;
    private bool canStart;
    public bool hasFoundClient;
    public float scrollHeight = 1.462f;
    private int dayOffset = 0;
    private bool showCloudOverlay = false;
    private bool playedGame = false;


	//Unity methods
    void Awake()
    {
        canStart = false;
        gameManager = GameObject.FindObjectOfType<GameManager>();
        leaderboardManager = GameObject.FindObjectOfType<SteamLeaderboardManager>();
        HideTimeTexts();
    }
    protected override void OnStart()
    {
        base.OnStart();
    }
    protected override void OnUpdate()
    {
        base.OnUpdate();

        DateTime today = DateManager.GetDateWithOffset(dayOffset);
        DateTime release = new DateTime(2016, 10, 6);

        int days = (int)(today - release).TotalDays;
        leftArrow.SetActive(days > 0);


        ScrollHighscores();

        //Show cloud overlay
        float targetAlpha = showCloudOverlay ? 0.5f : 0f;
        Color c = mountainNotAvailableOverlay.color;
        c.a = Mathf.MoveTowards(c.a, targetAlpha, Time.deltaTime);
        mountainNotAvailableOverlay.color = c;

        //Check for which frog to controll the mountain
        if (canStart)
        {
            for (int i = 0; i < 4; i++)
            {
                if (GameManager.GetPlayer(i).GetButtonDown("Button A"))
                {
                    SetPlayerToControlMountain(i);
                    OnPlayGame();
                    GameManager.DailyMountainPlayerID = i;
                }

                if (GameManager.GetPlayer(i).GetButtonDown("Button B"))
                {
                    ReturnToStartScreen();
                }
            }

        }
    }

	//public methods
    public void RefreshLeaderboards(int offsetInDaysFromToday)
    {
        if (leaderboardManager == null)
            return;

        //update date text to current date
        dateText.text = DateManager.GetDateString(offsetInDaysFromToday);

        //Enable load icon
        loadIcon.gameObject.SetActive(true);

        //Disable no entries text
        noEntriesToday.SetActive(false);
        noEntriesPreviousDay.SetActive(false);

        //Hide entries and fail texts
        entriesParent.gameObject.SetActive(false);
        couldNotConnectProperlyText.SetActive(false);

        //Aquire leaderboard for selected day
        AquireLeaderboard(offsetInDaysFromToday);
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
            if (dayOffset == 0 && !mountainGenerated)
                GenerateMountain();

            //Add fake entries
            //AddFakeEntries(57);

            //Enter client at fake rank
            //SetFakeRank(47);
        }

        //Find client entry
        clientEntry = GetClientEntry();

        if (playedGame)
        {
            if (hasFoundClient && clientEntry != null)
            {
                previousTimeObject.gameObject.SetActive(true);
                previousTimeObject.text = clientEntry.timer.GetTimeString(false, true, true, true);
            }
        }

        if (this.enabled)
        {
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

        if (entries.Count <= 0)
        {
            if (dayOffset == 0)
            {
                noEntriesToday.SetActive(true);
            }
            else
            {
                noEntriesPreviousDay.SetActive(true);
            }
        }
        else
        {
            noEntriesToday.SetActive(false);
            noEntriesPreviousDay.SetActive(false);
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

        playedGame = false;

        //Update seed
        dailySeed = DateManager.GetSeedFromUTC();

        //Enable menu music
        soundManager.StartMenuMusic();

        //Hide cloud
        HideCloudOverlay();

        //Can not start the game until leaderboards are ready
        canStart = false;

        //Hide right arrow
        rightArrow.SetActive(false);

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


        //Reset day offset
        dayOffset = 0;

        //Aquire leaderboard
        entryMinIndex = 0;
        RefreshLeaderboards(0);

        //Flag generated mountain
        mountainGenerated = false;


        //Disable scroll line
        scrollParent.SetActive(false);

        //Hide time texts
        HideTimeTexts();

        //Check if internetconnection was found
        DateManager.Initialize();
        if (DateManager.searchingForInternet)
        {
            Invoke("ReturnToStartScreen", 10f);
        }
    }
    public override void OnSwitchedFrom()
    {
        base.OnSwitchedFrom();

        //Unlock parallaxes
        gameManager.LockParallaxes(false);

        //Hide cloud
        HideCloudOverlay();

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
            gameManager.DestroyCurrentLevel(true);

            leaderboardManager.ResetGettingLeaderboards();
        }
        else
        {
            playedGame = true;
        }
    }

    public void HideTimeTexts()
    {
        currentTimeObject.gameObject.SetActive(false);
        previousTimeObject.gameObject.SetActive(false);
        timerBackground.SetActive(false);
    }
    public void OnPlayGame()
    {
        if (canStart)
        {
            //Hide cloud overlay
            HideCloudOverlay();
            gameManager.isInDailyMountain = true;
            currentTimeObject.SetActive(true);
            timerBackground.SetActive(true);
            dailyGamemode.OnPlayGame();

            AquireLeaderboard(0);

            //Disable menu music
            soundManager.StopMenuMusic();

            M_ScreenManager.SwitchScreen(gameScreen);

        }
    }
    public void RestartDaily()
    {
        poff.SetMenuMountainState(false, 0f);
        poff.PoffRepeating(true, dailySeed);

        gameManager.ResetGameVariables();
        gameManager.CreateNewFrogs();
        gameManager.isInDailyMountain = true;

        M_ScreenManager.SetActive(true);
        M_ScreenManager.SwitchScreen(gameScreen);
        dailyGamemode.OnPlayGame();
    }

	//private methods
    private void ScrollHighscores()
    {
        if (entries == null || entries.Count <= 0 || !mountainGenerated || !canScroll)
            return;

        float leftStickYValue = GameManager.defaultPlayer.GetAxis("LeftStick Vertical");

        if (leftStickYValue <= 0.5f && leftStickYValue >= -0.5f)
            return;

        int dir = (leftStickYValue > 0) ? -1 : 1;
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
        if (clientEntry != null && clientEntry.isClient)
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
            float height = scrollHeight * 2;
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

    private void ChangeDayOffset(int dir)
    {
        if (!canStart)
            return;

        if (loadIcon.activeInHierarchy)
            return;

        int previousOffset = dayOffset;
        //int maxDaysRecap = -7;
        dayOffset += dir;

        //dayOffset = Mathf.Clamp(dayOffset, maxDaysRecap, 0);

        if (previousOffset != dayOffset)
            RefreshLeaderboards(dayOffset);

        timeleftForTodayObject.gameObject.SetActive(dayOffset == 0);
        showCloudOverlay = dayOffset != 0;

        rightArrow.SetActive(dayOffset != 0);
        //leftArrow.SetActive(dayOffset != maxDaysRecap);
    }
    private void AquireLeaderboard(int offsetInDaysFromToday)
    {
        entries = new List<LeaderboardEntry>();
        LeaderboardEntries entriesObj = new LeaderboardEntries();
        entriesObj.entries = entries;


        //Aquire leaderboard name to find
        string leaderboardName = DateManager.GetSeedFromUTC(offsetInDaysFromToday).ToString();
        //Start searching for a leaderboard
        leaderboardManager.GetLeaderboardEntries(leaderboardName, entriesObj, OnEntriesCompleteMethod, OnEntriesFailedMethod);
    }
    
    private void GenerateMountain()
    {
        //Flag generated mountain
        mountainGenerated = true;

        //Generate a new mountain with the daily mode
        poff.SetMenuMountainState(false, 0f);
        poff.PoffRepeating(true, dailySeed);

        //Spawn mountain sound
        GameManager.PlayAudioSource(mountainGenerationSound, 0.7f, 1.4f);

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
        if (entries.Count <= 0)
            return;
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
        hasFoundClient = false;
        return null;
    }

    public void OnStickLeft()
    {
        ChangeDayOffset(-1);
    }
    public void OnStickRight()
    {
        ChangeDayOffset(1);
    }
    private void HideCloudOverlay()
    {
        showCloudOverlay = false;
        mountainNotAvailableOverlay.color = new Color(mountainNotAvailableOverlay.color.r, mountainNotAvailableOverlay.color.g, mountainNotAvailableOverlay.color.b, 0f);
    }
    private void ReturnToStartScreen()
    {
        SwitchScreen(startScreen);
    }
    private void SetPlayerToControlMountain(int playerID)
    {
        gameManager.CreateNewFrogs(playerID);
        
        for (int i = 0; i < 4; i++)
        {
            if (i != playerID)
            {
                gameManager.frogsReady[i] = false;
            }
            else
            {
                gameManager.frogsReady[i] = true;
            }
        }
    }
}
