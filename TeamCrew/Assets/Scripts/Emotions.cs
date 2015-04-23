using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Emotions : MonoBehaviour 
{
    public List<Sprite> head_emotions = new List<Sprite>();

    new private SpriteRenderer renderer;

    private float timerDelay;
    private float situationalTimer;
    private bool inSituational;
    private Sprite generalEmotion;

    private string currentEmotion;

	void Start () 
    {
        renderer = GetComponent<SpriteRenderer>();
	}
	
	void Update () 
    {
        if (inSituational)
        {
            situationalTimer += Time.deltaTime;

            if (situationalTimer >= timerDelay)
            {
                timerDelay = 0;
                situationalTimer = 0;
                inSituational = false;

                renderer.sprite = generalEmotion;
                generalEmotion = null;
            }
        }
	}

    public void SetGeneralEmotion(string emotion)
    {
        if (currentEmotion != emotion)
        {
            currentEmotion = emotion;
            renderer.sprite = FindEmotion(emotion);
        }
    }
    public void SetSituationalEmotion(string emotion, float time)
    {
        if (inSituational)
            return;

        inSituational = true;
        timerDelay = time;
        generalEmotion = renderer.sprite;
        renderer.sprite = FindEmotion(emotion);
    }
    Sprite FindEmotion(string emotion)
    {
        for (int i = 0; i < head_emotions.Count; i++)
        {
            if (head_emotions[i].name.Contains(emotion))
            {
                return head_emotions[i];
            }
        }

        return null;
    }
}
