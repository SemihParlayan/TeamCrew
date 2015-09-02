using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InactivityController : MonoBehaviour 
{
    public Text inactivityText;
    public float inactivitySecondLimit = 5;

    private float playerOneInactivityTimer;
    private float playerTwoInactivityTimer;
    private float inactivityTimer;

    private GameManager gameManager;
    private Respawn respawnScript;

	void Start () 
	{
        gameManager = GetComponent<GameManager>();
        respawnScript = GetComponent<Respawn>();

        inactivityTimer = inactivitySecondLimit;
	}

	void Update () 
	{
        if (gameManager.gameActive && !gameManager.designTestingEnabled)
        {
            //Increase inactivity timers
            playerOneInactivityTimer += Time.deltaTime;
            playerTwoInactivityTimer += Time.deltaTime;

            //Check if both players have been inactive for too long
            if (playerOneInactivityTimer >= inactivitySecondLimit && playerTwoInactivityTimer >= inactivitySecondLimit)
            {
                //Activate inactivity text
                inactivityText.transform.parent.gameObject.SetActive(true);
                inactivityTimer -= Time.deltaTime;

                //Update inactivity text
                inactivityText.text = "Inactivity! \n Returning to main menu in " + Mathf.RoundToInt(inactivityTimer) + "...";

                //Return to menu
                if (inactivityTimer <= 0)
                {
                    gameManager.GoBackToMenu();
                    playerOneInactivityTimer = 0;
                    playerTwoInactivityTimer = 0;
                }
            }
            else
            {
                //Disable inactivity text
                inactivityText.transform.parent.gameObject.SetActive(false);
                inactivityTimer = 5;
            }
        }


        //Deactivate inactivity with input from players
        DeactivateOnInput("P1");
        DeactivateOnInput("P2");

        //Set respawn script values
        respawnScript.playerOne.inactive = (playerOneInactivityTimer >= inactivitySecondLimit + 2.5f);
        respawnScript.playerTwo.inactive = (playerTwoInactivityTimer >= inactivitySecondLimit + 2.5f);


        //Set inactivity to max if in singleplayer mode
        if (!gameManager.IsInMultiplayerMode)
        {
            if (gameManager.singlePlayerStarted == "P1")
            {
                playerTwoInactivityTimer = 10;
            }
            else if (gameManager.singlePlayerStarted == "P2")
            {
                playerOneInactivityTimer = 10;
            }
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
            if (gameManager.singlePlayerStarted == string.Empty || gameManager.singlePlayerStarted == player)
            {
                if (player == "P1")
                    playerOneInactivityTimer = 0;
                else if (player == "P2")
                    playerTwoInactivityTimer = 0;
            }
            else if (gameManager.tutorialComplete)
            {
                if (player == "P1")
                    playerOneInactivityTimer = 0;
                else if (player == "P2")
                    playerTwoInactivityTimer = 0;

                gameManager.singlePlayerStarted = string.Empty;
            }
        }
    }
}
