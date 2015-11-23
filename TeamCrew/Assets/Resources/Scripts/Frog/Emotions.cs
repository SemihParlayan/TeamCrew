using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Emotions : MonoBehaviour 
{
    public List<Transform> headTransforms = new List<Transform>();

    private float timerDelay;
    private float situationalTimer;
    public bool inSituational;
    private Transform generalEmotion;

    private Transform currentEmotion;
	
    void Start()
    {
        headTransforms.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            headTransforms.Add(transform.GetChild(i));
        }

        currentEmotion = FindEmotion(Emotion.neutral);
        currentEmotion.gameObject.SetActive(true);
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

                currentEmotion.gameObject.SetActive(false);

                currentEmotion = generalEmotion;
                currentEmotion.gameObject.SetActive(true);
                generalEmotion = null;
            }
        }
	}

    public void SetGeneralEmotion(Emotion emotion)
    {
        Transform head = FindEmotion(emotion);
        if (head == null)
            return;

        if (inSituational)
        {
            generalEmotion = head;
        }
        else if (currentEmotion == null)
        {
            currentEmotion = head;
            currentEmotion.gameObject.SetActive(true);
        }
        else if (currentEmotion.name != head.name)
        {
            currentEmotion.gameObject.SetActive(false);

            currentEmotion = head;
            currentEmotion.gameObject.SetActive(true);
        }
    }
    public void SetSituationalEmotion(Emotion emotion, float time)
    {
        if (inSituational)
            return;

        Transform head = FindEmotion(emotion);

        if (head)
        {
            inSituational = true;
            timerDelay = time;
            generalEmotion = currentEmotion;
            generalEmotion.gameObject.SetActive(false);

            currentEmotion = head;
            currentEmotion.gameObject.SetActive(true);
        }
    }
    Transform FindEmotion(Emotion emotion)
    {
        string e = emotion.ToString();

        for (int i = 0; i < headTransforms.Count; i++)
        {
            if (headTransforms[i].name.Contains(e))
            {
                return headTransforms[i];
            }
        }

        return null;
    }
    public Transform GetCurrentEmotion()
    {
        return currentEmotion;
    }
}
