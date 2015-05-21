using UnityEngine;
using System.Collections;

public enum Fade
{
    ins,
    outs,
    nones
}

public class MenuMusicController : MonoBehaviour
{
    public AudioClip[] SoundList;

    private Fade fade = Fade.ins;

    private float fadeInDuration = 10;
    private float fadeOutDuration = 1;

    private float timer;

    private AudioSource speaker;

    void Start()
    {
        speaker = transform.GetComponent<AudioSource>();

        speaker.volume = 0;

        Gen();
        ChangeFadeState(fade);
        Play();
    }

    public void Gen()
    {
        speaker.clip = SoundList[Random.Range(0, SoundList.Length)];
    }

    public void Play()
    {
        speaker.Play();
    }

    public void Stop()
    {
        speaker.Stop();
    }

    public void Update()
    {
        switch(fade)
        {
            case Fade.ins:
            {
                timer += Time.deltaTime;
                if (timer >= fadeInDuration)
                {
                    speaker.volume = 1;
                    ChangeFadeState(Fade.nones);
                }
                else speaker.volume = (timer / fadeInDuration);

                break;
            }

            case Fade.outs:
            {
                timer += Time.deltaTime;
                if (timer >= fadeOutDuration)
                {
                    Stop();
                    speaker.volume = 0;
                    ChangeFadeState(Fade.nones);
                }
                else speaker.volume = 1 - (timer / fadeOutDuration);

                break;
            }
        }
    }

    public void ChangeFadeState(Fade state)
    {
        if(speaker == null)
        {
            speaker = GetComponent<AudioSource>();
        }
        switch(state)
        {
            case Fade.ins:
                timer = 0;
                speaker.volume = 0;
                fade = Fade.ins;
                break;
            case Fade.outs:
                timer = 0;
                speaker.volume = 1;
                fade = Fade.outs;

                break;
            case Fade.nones:
                fade = Fade.nones;

                break;
            default: break;

        }
    }

}
