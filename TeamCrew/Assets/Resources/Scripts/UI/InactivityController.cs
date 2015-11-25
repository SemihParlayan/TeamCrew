using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class InactivityController : MonoBehaviour 
{
    //Data
    public float inactivitySecondLimit = 6;
    private float inactivityTimer;
    private bool started;

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

        inactivityTimer = inactivitySecondLimit * 1.5f;

        ResetVariables();
	}
    private void ResetVariables()
    {
        for (int i = 0; i < inactivityScripts.Length; i++)
        {
            inactivityScripts[i].frog = "P" + (i + 1).ToString();
            inactivityScripts[i].inactivityLimit = inactivitySecondLimit;
            inactivityScripts[i].inactivityTimer = inactivitySecondLimit;
        }
    }

    public void OnGameStart()
    {
        for (int i = 0; i < GameManager.players.Length; i++)
        {
            if (GameManager.players[i] != null)
            {
                inactivityScripts[i].inactivityTimer = 0;
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
            //gameObject.SetActive(false);
            return;
        }

        if (!gameManager.gameActive || gameManager.designTestingEnabled)
            return;

        //Search for how many frogs are inactive
        int inactivityFrogCounter = 0;
        for (int i = 0; i < inactivityScripts.Length; i++)
        {
            if (inactivityScripts[i] != null)
            {
                inactivityScripts[i].inactivityTimer += Time.deltaTime;

                if (inactivityScripts[i].IsInactive)
                {
                    inactivityFrogCounter++;
                }
            }
        }

        if (inactivityFrogCounter >= inactivityScripts.Length)
        {
            //Activate inactivity text
            inactivityText.transform.parent.gameObject.SetActive(true);
            inactivityTimer -= Time.deltaTime;

            //Update inactivity text
            inactivityText.text = "Inactivity! \n Returning to main menu in " + Mathf.RoundToInt(inactivityTimer) + "...";

            //Return to menu
            if (inactivityTimer <= 0)
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
            inactivityTimer = 5;
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
        if (input != Vector3.zero || input2 != Vector3.zero || button || button2 || (GameManager.Hacks && Input.GetMouseButton(0)))
        {
            string sub = player.Split('P').Last();
            int frog = int.Parse(sub) - 1;

            if (!gameManager.tutorialComplete)
            {
                if (GameManager.players[frog] != null)
                {
                    inactivityScripts[frog].inactivityTimer = 0;
                }
            }
            else
            {
                inactivityScripts[frog].inactivityTimer = 0;
            }
        }
    }
}
