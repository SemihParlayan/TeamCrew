﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour 
{
    //Components
    public GameObject UIParent;
    public PlayerReadyInput playerOneReadyInput;
    public PlayerReadyInput playerTwoReadyInput;
    public Image playerOneReady;
    public Image playerTwoReady;

    private Animator anim;
    private GameManager gameManager;

    //Countdown 3-2-1-GO! variables
    [HideInInspector]public bool goReady;
    public Sprite[] goSprites;
    public SpriteRenderer[] goRenderers;
    private int goIndex;
    private float goTimer;
    private bool startGo;

    void Start()
    {
        anim = GetComponent<Animator>();
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        EnableUI();
    }
	void Update () 
    {
        ///Countdown 3-2-1-GO!
        if (startGo)
        {
            goTimer += Time.deltaTime;

            if (goTimer >= 1)
            {
                goTimer = 0;
                goIndex++;

                goRenderers[goIndex].sprite = goSprites[goIndex];

                if (goIndex >= 2)
                {
                    goReady = true;
                    startGo = false;
                    playerOneReady.gameObject.SetActive(false);
                    playerTwoReady.gameObject.SetActive(false);
                }
            }
        }
	}

    public void StartGoImage(SpriteRenderer[] renderers)
    {
        if (startGo || goReady)
            return;

        goRenderers = renderers;
        startGo = true;
        goIndex = -1;
    }

    public void DisableUI()
    {
        anim.SetTrigger("DisableUI");
    }
    public void DisableUIParent()
    {
        playerOneReadyInput.ready = false;
        playerOneReadyInput.singlePlayerReady = false;

        playerTwoReadyInput.ready = false;
        playerTwoReadyInput.singlePlayerReady = false;
        UIParent.SetActive(false);
    }
    public void EnableUI()
    {
        Invoke("EnableUIParent", 0.1f);
        anim.SetTrigger("EnableUI");
    }
    private void EnableUIParent()
    {
        UIParent.SetActive(true);
    }

    public Animator p1Win;
    public Animator p2Win;
    public Animator p1DeathCounter;
    public Animator p2DeathCounter;
    private int winFrog;

    public void StartMenuCycle(int frogNumber)
    {
        winFrog = frogNumber;
        Invoke("EnableWinSign", 1.5f);
    }

    void EnableWinSign()
    {
        if (winFrog == 1)
        {
            p1Win.SetTrigger("Activate");
        }
        else
        {
            p2Win.SetTrigger("Activate");
        }
    }
    public void WinSignComplete()
    {
        EnableDeathCounter();
    }

    public Animator victoryTextSign;
    void EnableDeathCounter()
    {
        p1DeathCounter.SetTrigger("Activate");
        p2DeathCounter.SetTrigger("Activate");

        Vector2 deathCount = gameManager.GetFrogDeathCount();
        p1DeathCounter.transform.GetChild(0).GetComponent<Text>().text = deathCount.x.ToString();
        p2DeathCounter.transform.GetChild(0).GetComponent<Text>().text = deathCount.y.ToString();

        int v = 0;

        if (winFrog == 1)
        {
            v = (int)deathCount.y - (int)deathCount.x;
        }
        else
        {
            v = (int)deathCount.x - (int)deathCount.y;
        }

        string t = "";

        if (v <= -3)
            t = "Disgusting victory!";
        else if (v == -2)
            t = "Douchefrog victory";
        else if (v == -1)
            t = "Bandaged victory";
        else if (v == 0)
            t = "Fair victory";
        else if (v == 1)
            t = "Beautiful victory";
        else if (v == 2)
            t = "Crushingly dominant victory";
        else if (v >= 3)
            t = "You-won-even-though-your-friend-is-a-butt victory";
        else
            t = "Victory";

        victoryTextSign.SetTrigger("Activate");
        victoryTextSign.transform.GetChild(0).GetComponent<Text>().text = t;
    }
    public void DeathCounterComplete()
    {
        EnableUI();
    }
}
