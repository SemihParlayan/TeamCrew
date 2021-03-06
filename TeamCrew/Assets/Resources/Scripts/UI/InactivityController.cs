﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class InactivityController : MonoBehaviour
{
    //Data
    public float signLimit;
    public float frogInactivityLimit = 6;
    public float signTimer;
    public bool active;
    public bool gameHasStartedDelay = false;

    //Components

    //References
    public Text inactivityText;
    private GameManager gameManager;
    private Respawn respawn;
    public InactivityFrog[] inactivityScripts = new InactivityFrog[4];
    public M_Screen mainmenuScreen;
    public EndgameMenuScreen endgameScreen;
    private M_FadeToColor fade;

    void Awake()
    {
        fade = GetComponent<M_FadeToColor>();
    }
    void Start()
    {
        gameManager = transform.parent.GetComponent<GameManager>();
        respawn = transform.parent.GetComponent<Respawn>();

        ResetVariables();
    }

    private void ResetVariables()
    {
        signTimer = signLimit;

        for (int i = 0; i < inactivityScripts.Length; i++)
        {
            inactivityScripts[i].frog = "P" + (i + 1).ToString();
            inactivityScripts[i].limit = frogInactivityLimit;
        }
    }
    public void OnGameStart()
    {
        for (int i = 0; i < GameManager.players.Length; i++)
        {
            if (GameManager.players[i] != null)
            {
                inactivityScripts[i].timer = 0;
            }
        }
    }
    private void GameHasStarted()
    {
        gameHasStartedDelay = true;
    }

    public void ResetGameStartDelay()
    {
        gameHasStartedDelay = false;
    }
    public void OnCameraReachedBottom()
    {
        Invoke("GameHasStarted", 0.5f);
    }

    void Update()
    {
        if (fade.Halfway)
        {
            ResetVariables();
            endgameScreen.ActivateMenuMountain();
            M_ScreenManager.SetActive(true);
            M_ScreenManager.SwitchScreen(mainmenuScreen);
            M_ScreenManager.TeleportToCurrentScreen();
            gameManager.DestroyFrogs();
            gameManager.DestroyTopFrog();
            gameManager.ResetGameVariables();
            gameManager.DestroyCurrentLevel(false);
            return;
        }

        if (!active)
            return;

        //Search for how many frogs are inactive
        int inactivityFrogCounter = 0;
        for (int i = 0; i < inactivityScripts.Length; i++)
        {
            if (inactivityScripts[i] == null)
                continue;

            InactivityFrog frog = inactivityScripts[i];
            if (!gameManager.isInDailyMountain)
            {
                if (gameManager.gameActive)
                {
                    if (gameManager.tutorialComplete)
                        frog.timer += Time.deltaTime;
                }
                else
                {
                    frog.timer += Time.deltaTime;
                }


                if (frog.IsInactive)
                {
                    inactivityFrogCounter++;
                }
            }
            else
            {
                if (i != GameManager.DailyMountainPlayerID)
                    frog.timer = float.MaxValue;
            }
        }


        if (GameManager.ReturnToMenuWhenInactive)
        {
            //If all frogs are inactive
            if (inactivityFrogCounter >= inactivityScripts.Length)
            {
                //Activate inactivity text
                inactivityText.transform.parent.gameObject.SetActive(true);
                signTimer -= Time.deltaTime;

                //Update inactivity text
                inactivityText.text = "Inactivity! \n Returning to main menu in " + Mathf.RoundToInt(signTimer) + "...";

                //Return to menu
                if (signTimer <= 1)
                {
                    for (int i = 0; i < inactivityScripts.Length; i++)
                    {
                        fade.StartFade();
                    }
                }
            }
            else
            {
                //Disable inactivity text
                inactivityText.transform.parent.gameObject.SetActive(false);
                signTimer = signLimit;
            }
        }

        if (!gameManager.isInDailyMountain)
        {
            //Deactivate inactivity with input from players
            for (int i = 0; i < inactivityScripts.Length; i++)
            {
                if (gameHasStartedDelay)
                {
                    if (gameManager.gameActive)
                    {
                        if (gameManager.tutorialComplete)
                        {
                            DeactivateOnInput(i);
                        }
                        else
                        {
                            if (!inactivityScripts[i].IsInactive)
                            {
                                DeactivateOnInput(i);
                            }
                        }
                    }
                    else
                    {
                        DeactivateOnInput(i);
                    }
                }
            }
        }

        //Set respawn script values
        for (int i = 0; i < respawn.respawnScripts.Count; i++)
        {
            respawn.respawnScripts[i].inactive = inactivityScripts[i].IsInactive;
        }

        if (gameManager.gameActive)
        {
            for (int i = 0; i < gameManager.frogsReady.Length; i++)
            {
                gameManager.frogsReady[i] = !inactivityScripts[i].IsInactive;
            }
        }
    }
    private void DeactivateOnInput(int player)
    {
        bool actionUsed =
            GameManager.GetPlayer(player).GetButton("Button A") ||
            GameManager.GetPlayer(player).GetButton("Button B") ||
            GameManager.GetPlayer(player).GetButton("Button X") ||
            GameManager.GetPlayer(player).GetButton("Button Y") ||
            GameManager.GetPlayer(player).GetButton("LeftStick Horizontal") ||
            GameManager.GetPlayer(player).GetButton("LeftStick Vertical") ||
            GameManager.GetPlayer(player).GetButton("RightStick Horizontal") ||
            GameManager.GetPlayer(player).GetButton("RightStick Vertical") ||
            GameManager.GetPlayer(player).GetButton("LeftShoulder") ||
            GameManager.GetPlayer(player).GetButton("RightShoulder") ||
            GameManager.GetPlayer(player).GetButton("LeftTrigger") ||
            GameManager.GetPlayer(player).GetButton("RightTrigger") ||
            GameManager.GetPlayer(player).GetButton("Select");
        if (actionUsed)
        {
            inactivityScripts[player].timer = 0;
        }
    }
    public void SetActiveValue(bool value, float newLimit)
    {
        frogInactivityLimit = newLimit;
        active = value;

        if (!active)
        {
            ResetVariables();
            OnGameStart();
            inactivityText.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            for (int i = 0; i < inactivityScripts.Length; i++)
            {
                inactivityScripts[i].timer = 0;
            }
        }
    }

    public void SetTimersForFrogs(bool[] frogsReady)
    {
        for (int i = 0; i < frogsReady.Length; i++)
        {
            if (frogsReady[i])
                inactivityScripts[i].timer = 0;
            else
                inactivityScripts[i].timer = int.MaxValue;
        }
    }
}
