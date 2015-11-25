using UnityEngine;
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
	void Start () 
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

	void Update () 
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
            gameManager.DestroyCurrentLevel(true);
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
            frog.timer += Time.deltaTime;

            if (frog.IsInactive)
            {
                inactivityFrogCounter++;
            }
        }

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

            
        //Deactivate inactivity with input from players
        for (int i = 0; i < inactivityScripts.Length; i++)
        {
            DeactivateOnInput("P" + (i + 1));
        }

        //Set respawn script values
        for (int i = 0; i < respawn.respawnScripts.Count; i++)
        {
            respawn.respawnScripts[i].inactive = inactivityScripts[i].IsInactive;
        }
	}
    private void DeactivateOnInput(string player)
    {
        Vector3 input = GameManager.GetInput(player + "HL", player + "VL");
        Vector3 input2 = GameManager.GetInput(player + "HR", player + "VR");
        bool button = GameManager.GetGrip(player + "GL");
        bool button2 = GameManager.GetGrip(player + "GR");
        bool button3 = Input.GetButtonDown(player + "MenuSelectX");
        bool button4 = Input.GetButtonDown(player + "MenuReturnX");

        if (input != Vector3.zero || input2 != Vector3.zero || button || button2 || button3 || button4 || (GameManager.Hacks && Input.GetMouseButton(0)))
        {
            string sub = player.Split('P').Last();
            int frog = int.Parse(sub) - 1;

            inactivityScripts[frog].timer = 0;
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
        for (int i = 0 ; i < frogsReady.Length; i++)
        {
            if (frogsReady[i])
                inactivityScripts[i].timer = 0;
            else
                inactivityScripts[i].timer = int.MaxValue;
        }
    }
}
