using UnityEngine;
using System.Collections;

public enum FadeState
{
    IN,
    OUT,
    NONE
}

public class MenuMusicController : MonoBehaviour
{
    public AudioClip[] SoundList;
    public int songNum = 4;

    private FadeState fade = FadeState.NONE;

    private float fadeInDuration = 10;
    private float fadeOutDuration = 1;

    private float timer;

    private AudioSource speaker;

    void Awake()
    {
        speaker = transform.GetComponent<AudioSource>();
    }
    void Start()
    {
       

        if (speaker != null)
            Debug.Log("yay speaker is here!");

        speaker.volume = 0;

        //Gen();
        SetA(songNum);
    }

    public void Gen()
    {
        speaker.clip = SoundList[Random.Range(0, SoundList.Length)];
    }
    public void SetA(int songNum)
    {
        speaker.clip = SoundList[songNum];
    }

    public void Play()
    {
        if(speaker != null)
            speaker.Play();
        else
        {
            Debug.Log("no fucking speaker");
        }
    }

    public void Stop()
    {
        speaker.Stop();
    }

    public void Update()
    {
        switch(fade)
        {
            case FadeState.IN:
            {
                timer += Time.deltaTime;
                if (timer >= fadeInDuration)
                {
                    speaker.volume = 1;
                    ChangeFadeState(FadeState.NONE);
                }
                else speaker.volume = (timer / fadeInDuration);

                break;
            }

            case FadeState.OUT:
            {
                timer += Time.deltaTime;
                if (timer >= fadeOutDuration)
                {
                    Stop();
                    speaker.volume = 0;
                    ChangeFadeState(FadeState.NONE);
                }
                else speaker.volume = 1 - (timer / fadeOutDuration);

                break;
            }
        }
    }

    public void ChangeFadeState(FadeState state)
    {
        if(speaker == null)
        {
            speaker = GetComponent<AudioSource>();
        }
        switch(state)
        {
            case FadeState.IN:
                timer = 0;
                speaker.volume = 0;
                fade = FadeState.IN;
                break;
            case FadeState.OUT:
                timer = 0;
                speaker.volume = 1;
                fade = FadeState.OUT;

                break;
            case FadeState.NONE:
                fade = FadeState.NONE;

                break;
            default: break;

        }
    }

}
