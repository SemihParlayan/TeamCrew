using UnityEngine;
using System.Collections;
using System.Collections.Generic;


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
            if (satisfied)
            {
                s.emotionsScript.SetSituationalEmotion(Emotion.satisfied, 0.5f);
            }








            ////Panic
            //if (!s.leftGripScript.isOnGrip && !s.rightGripScript.isOnGrip)
            //{
            //    if (s.body)
            //    {
            //        if (s.body.velocity.y < -2f)
            //        {
            //            s.emotionsScript.SetGeneralEmotion(Emotion.panic);
            //        }
            //    }
            //}

            ////Versus and stone grip
            //if (s.leftGripScript.JustGripped || s.rightGripScript.JustGripped)
            //{
            //    if (s.leftGripScript.isVersusGripping || s.rightGripScript.isVersusGripping)
            //    {
            //        s.emotionsScript.SetGeneralEmotion(Emotion.taunt);
            //    }
            //    else
            //    {
            //        s.emotionsScript.SetGeneralEmotion(Emotion.neutral);
            //        s.emotionsScript.SetSituationalEmotion(Emotion.satisfied, 0.5f);
            //    }
            //}

            ////Angry
            //if (s.versusHands > 0)
            //{
            //    s.emotionsScript.SetGeneralEmotion(Emotion.angry);
            //}
            //else
            //{
            //    if (s.emotionsScript.GetCurrentEmotion().name.Contains(Emotion.angry.ToString()))
            //    {
            //        s.emotionsScript.SetGeneralEmotion(Emotion.neutral);
            //    }
            //}
        }







        /////////////////////////////////////////////////////////////////////////////
        ////          Reset angry when other player releases versus grip
        /////////////////////////////////////////////////////////////////////////////
        //if (playerOneScript != null && playerTwoScript != null)
        //{
        //    if (playerOneScript.emotionsScript.isAngry)
        //    {
        //        if (!playerTwoScript.leftGripScript.isVersusGripping && !playerTwoScript.rightGripScript.isVersusGripping)
        //        {
        //            playerOneScript.emotionsScript.isAngry = false;
        //            playerOneScript.emotionsScript.SetGeneralEmotion("neutral");
        //            playerTwoScript.emotionsScript.SetGeneralEmotion("neutral");
        //        }
        //    }

        //    if (playerTwoScript.emotionsScript.isAngry)
        //    {
        //        if (!playerOneScript.leftGripScript.isVersusGripping && !playerOneScript.rightGripScript.isVersusGripping)
        //        {
        //            playerTwoScript.emotionsScript.isAngry = false;
        //            playerTwoScript.emotionsScript.SetGeneralEmotion("neutral");
        //            playerOneScript.emotionsScript.SetGeneralEmotion("neutral");
        //        }
        //    }
        //}

        
        
	}
}
