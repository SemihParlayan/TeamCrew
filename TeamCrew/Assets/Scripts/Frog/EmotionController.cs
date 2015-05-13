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

        if (playerOneScript == null)
        {
            playerOneScript = GameManager.playerOne.GetComponent<FrogPrototype>();
        }
        if (playerTwoScript == null)
        {
            playerTwoScript = GameManager.playerTwo.GetComponent<FrogPrototype>();
        }

        float p1Y = GameManager.playerOne.position.y;
        float p2Y = GameManager.playerTwo.position.y;

        float distance = p1Y - p2Y;

        if (distance >= gladDistance)
        {
            playerOneScript.emotionsScript.SetGeneralEmotion("satisfied");
            playerTwoScript.emotionsScript.SetGeneralEmotion("irritated");
        }
        else if (distance <= -gladDistance)
        {
            playerOneScript.emotionsScript.SetGeneralEmotion("irritated");
            playerTwoScript.emotionsScript.SetGeneralEmotion("satisfied");
        }
        else
        {
            playerOneScript.emotionsScript.SetGeneralEmotion("neutral");
            playerTwoScript.emotionsScript.SetGeneralEmotion("neutral");
        }

        if (playerOneScript.leftGripScript.JustGripped || playerOneScript.rightGripScript.JustGripped)
        {
            playerOneScript.emotionsScript.SetSituationalEmotion("satisfied", 0.5f);
        }
        if (playerTwoScript.leftGripScript.JustGripped || playerTwoScript.rightGripScript.JustGripped)
        {
            playerTwoScript.emotionsScript.SetSituationalEmotion("satisfied", 0.5f);
        }
	}
}
