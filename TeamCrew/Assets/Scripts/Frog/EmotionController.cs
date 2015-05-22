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
        ///////////////////////////////////////////////////////////////////////////
        //       Aquire frog scripts
        ///////////////////////////////////////////////////////////////////////////
        if (playerOneScript == null)
        {
            if (GameManager.playerOne != null)
                playerOneScript = GameManager.playerOne.GetComponent<FrogPrototype>();
        }
        if (playerTwoScript == null)
        {
            if (GameManager.playerTwo != null)
                playerTwoScript = GameManager.playerTwo.GetComponent<FrogPrototype>();
        }


        ///////////////////////////////////////////////////////////////////////////
        //       Player versus grip (taunt) & satisfied(stone grip)
        ///////////////////////////////////////////////////////////////////////////
        if (playerOneScript != null)
        {
            if (playerOneScript.leftGripScript.JustGripped || playerOneScript.rightGripScript.JustGripped)
            {
                playerOneScript.emotionsScript.SetGeneralEmotion("neutral");

                if (playerOneScript.leftGripScript.isVersusGripping || playerOneScript.rightGripScript.isVersusGripping && playerTwoScript != null) //Grab an oponent
                {
                    playerOneScript.emotionsScript.SetGeneralEmotion("taunt");
                    playerTwoScript.emotionsScript.SetGeneralEmotion("angry");
                    playerTwoScript.emotionsScript.SetSituationalEmotion("panic", 0.5f);
                    playerTwoScript.emotionsScript.isAngry = true;
                }
                else
                    playerOneScript.emotionsScript.SetSituationalEmotion("satisfied", 0.5f); // Grab a stone grip
            }
        }
        if (playerTwoScript != null)
        {
            if (playerTwoScript.leftGripScript.JustGripped || playerTwoScript.rightGripScript.JustGripped)
            {
                playerTwoScript.emotionsScript.SetGeneralEmotion("neutral");

                if (playerTwoScript.leftGripScript.isVersusGripping || playerTwoScript.rightGripScript.isVersusGripping && playerOneScript != null)//Grab an oponent
                {
                    playerTwoScript.emotionsScript.SetGeneralEmotion("taunt");
                    playerOneScript.emotionsScript.SetGeneralEmotion("angry");
                    playerOneScript.emotionsScript.SetSituationalEmotion("panic", 0.5f);
                    playerOneScript.emotionsScript.isAngry = true;
                }
                else
                    playerTwoScript.emotionsScript.SetSituationalEmotion("satisfied", 0.5f);// Grab a stone grip
            }
        }
        ///////////////////////////////////////////////////////////////////////////
        //          Reset angry when other player releases versus grip
        ///////////////////////////////////////////////////////////////////////////
        if (playerOneScript != null && playerTwoScript != null)
        {
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
        ///////////////////////////////////////////////////////////////////////////
        //                      Panic in air with no grips
        ///////////////////////////////////////////////////////////////////////////
        if (playerOneScript != null)
        {
            if (!playerOneScript.leftGripScript.isOnGrip && !playerOneScript.rightGripScript.isOnGrip)
            {
                if (playerOneScript.body.velocity.y < -2)
                {
                    playerOneScript.emotionsScript.SetGeneralEmotion("panic");
                }
            }
        }
        if (playerTwoScript != null)
        {
            if (!playerTwoScript.leftGripScript.isOnGrip && !playerTwoScript.rightGripScript.isOnGrip)
            {
                if (playerTwoScript.body.velocity.y < -2)
                {
                    playerTwoScript.emotionsScript.SetGeneralEmotion("panic");
                }
            }
        }
        
	}
}
