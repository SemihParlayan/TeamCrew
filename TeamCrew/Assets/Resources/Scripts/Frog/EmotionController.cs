using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XInputDotNetPure;


public enum Emotion { neutral, angry, panic, satisfied, taunt }

[RequireComponent(typeof(Respawn))]
public class EmotionController : MonoBehaviour 
{
	void Update () 
    {
        //Aquire Frog scripts
        List<FrogPrototype> scripts = new List<FrogPrototype>();
        for (int i = 0; i < GameManager.players.Length; i++)
        {
            if (GameManager.players[i] == null)
                continue;

            scripts.Add(GameManager.players[i].GetComponent<FrogPrototype>());
        }




        ///////////////////////////////////////////////////////////////////////////
        for (int i = 0; i < scripts.Count; i++)
        {
            FrogPrototype s = scripts[i];



            Emotion currentEmotion = Emotion.neutral;
            bool satisfied = false;

            //Panic
            if (s.body)
            {
                if (s.body.velocity.y < -2f)
                {
                    currentEmotion = Emotion.panic;
                }
            }

            //Angry
            if (s.versusHands > 0)
            {
                currentEmotion = Emotion.angry;
            }


            //Taunt
            if (s.leftGripScript.isVersusGripping || s.rightGripScript.isVersusGripping)
            {
                currentEmotion = Emotion.taunt;
            }
            
            
            //Satisfied
            if (s.leftGripScript.JustGripped || s.rightGripScript.JustGripped)
            {
                satisfied = true;
            }

            s.emotionsScript.SetGeneralEmotion(currentEmotion);
            if (satisfied && currentEmotion != Emotion.angry && currentEmotion != Emotion.taunt)
            {
                s.emotionsScript.SetSituationalEmotion(Emotion.satisfied, 0.5f);
            }
        }
	}
}
