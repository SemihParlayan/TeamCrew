using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Emotions : MonoBehaviour 
{
    public List<Transform> headTransforms = new List<Transform>();

    public bool isAngry;
    private float timerDelay;
    private float situationalTimer;
    private bool inSituational;
    private Transform generalEmotion;

    private Transform currentEmotion;
	
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            headTransforms.Add(transform.GetChild(i));
        }

        currentEmotion = FindEmotion("neutral");
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

    public void SetGeneralEmotion(string emotion)
    {
        Transform head = FindEmotion(emotion);
        if (head == null)
            return;

        if (currentEmotion.name != head.name)
        {
            currentEmotion.gameObject.SetActive(false);
            currentEmotion = head;
            currentEmotion.gameObject.SetActive(true);
        }
    }
    public void SetSituationalEmotion(string emotion, float time)
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
    Transform FindEmotion(string emotion)
    {
        for (int i = 0; i < headTransforms.Count; i++)
        {
            if (headTransforms[i].name.Contains(emotion))
            {
                return headTransforms[i];
            }
        }

        return null;
    }
}
