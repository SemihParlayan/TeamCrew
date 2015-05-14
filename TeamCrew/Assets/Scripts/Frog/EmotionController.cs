using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Respawn))]
public class EmotionController : MonoBehaviour 
{
	//Respawn script reference
    public float gladDistance = 15;

    private FrogPrototype playerOneScript, playerTwoScript;

	void Start () 
    {
	}

	void Update () 
    {
        if (GameManager.playerOne == null || GameManager.playerTwo == null)
            return;

        //Aquire playerScripts
        if (playerOneScript == null)
        {
            playerOneScript = GameManager.playerOne.GetComponent<FrogPrototype>();
        }
        if (playerTwoScript == null)
        {
            playerTwoScript = GameManager.playerTwo.GetComponent<FrogPrototype>();
        }


        //Change emotions
        if (playerOneScript.leftGripScript.JustGripped || playerOneScript.rightGripScript.JustGripped)
        {
            if (playerOneScript.leftGripScript.isVersusGripping || playerOneScript.rightGripScript.isVersusGripping) //Grab an oponent
            {
                playerOneScript.emotionsScript.SetGeneralEmotion("taunt");
                playerTwoScript.emotionsScript.SetGeneralEmotion("angry");
                playerTwoScript.emotionsScript.SetSituationalEmotion("surprise", 0.5f);
                playerTwoScript.emotionsScript.isAngry = true;
            }
            else
                playerOneScript.emotionsScript.SetSituationalEmotion("satisfied", 0.5f); // Grab a stone grip
        }
        if (playerTwoScript.leftGripScript.JustGripped || playerTwoScript.rightGripScript.JustGripped)
        {
            if (playerTwoScript.leftGripScript.isVersusGripping || playerTwoScript.rightGripScript.isVersusGripping)//Grab an oponent
            {
                playerTwoScript.emotionsScript.SetGeneralEmotion("taunt");
                playerOneScript.emotionsScript.SetGeneralEmotion("angry");
                playerOneScript.emotionsScript.SetSituationalEmotion("surprise", 0.5f);
                playerOneScript.emotionsScript.isAngry = true;
            }
            else
                playerTwoScript.emotionsScript.SetSituationalEmotion("satisfied", 0.5f);// Grab a stone grip
        }

        if (playerOneScript.emotionsScript.isAngry)
        {
            if (!playerTwoScript.leftGripScript.isVersusGripping && !playerTwoScript.rightGripScript.isVersusGripping)
            {
                playerOneScript.emotionsScript.isAngry = false;
                playerOneScript.emotionsScript.SetGeneralEmotion("neutral");
                playerTwoScript.emotionsScript.SetGeneralEmotion("neutral");
            }
        }
        if (playerTwoScript.emotionsScript.isAngry)
        {
            if (!playerOneScript.leftGripScript.isVersusGripping && !playerOneScript.rightGripScript.isVersusGripping)
            {
                playerTwoScript.emotionsScript.isAngry = false;
                playerTwoScript.emotionsScript.SetGeneralEmotion("neutral");
                playerOneScript.emotionsScript.SetGeneralEmotion("neutral");
            }
        }
	}
}
