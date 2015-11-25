using UnityEngine;
using System.Collections;

public class M_FadeToColor : MonoBehaviour
{
    public SpriteRenderer fadeSprite;
    public float inSpeed;
    public float outSpeed;
    public float pauseDuration;

    public bool OnComplete
    {
        get
        {
            return (!lastComplete && complete);
        }
    }
    private bool complete, lastComplete;


    public bool Halfway
    {
        get
        {
            return (!lastHalfway && halfway);
        }
    }
    private bool halfway, lastHalfway;

    private bool fading;
    private bool started;
    private bool pause;


    void Update()
    {
        if (pause)
            return;

        if (!started)
            return;

        if (OnComplete)
        {
            started = false;
            //gameObject.SetActive(false);
        }

        lastComplete = complete;
        lastHalfway = halfway;

        if (fading)
        {
            Color c = fadeSprite.color;
            c.a = Mathf.MoveTowards(c.a, 1f, Time.deltaTime * inSpeed);
            fadeSprite.color = c;

            if (fadeSprite.color.a >= 0.99f)
            {
                fading = false;
                halfway = true;
                pause = true;
                Invoke("Unpause", pauseDuration);
            }
        }
        else
        {
            Color c = fadeSprite.color;
            c.a = Mathf.MoveTowards(c.a, 0f, Time.deltaTime * outSpeed);
            fadeSprite.color = c;

            if (fadeSprite.color.a <= 0.01f)
            {
                complete = true;
            }
        }
    }
    public void StartFade()
    {
        if (started)
            return;

        //gameObject.SetActive(true);
        this.started = true;
        this.fading = true;
        this.complete = false;
        this.lastComplete = false;
        this.halfway = false;
        this.lastHalfway = false;
    }

    private void Unpause()
    {
        pause = false;
    }
}

