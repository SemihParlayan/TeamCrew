using UnityEngine;
using System.Collections;

public class M_Sounds : MonoBehaviour 
{
    public AudioClip buttonClick;
    public AudioClip buttonSwitch;

    public AudioSource menuMusic;
    private bool playMenuMusic = true;

    public void Update()
    {
        //Menu music
        if (playMenuMusic)
        {
            menuMusic.volume = Mathf.MoveTowards(menuMusic.volume, 1f, Time.deltaTime);
        }
        else
        {
            menuMusic.volume = Mathf.MoveTowards(menuMusic.volume, 0f, Time.deltaTime);
        }
    }

    public void StartMenuMusic()
    {
        playMenuMusic = true;
        menuMusic.Stop();
    }
    public void StopMenuMusic()
    {
        playMenuMusic = false;
        menuMusic.Play();
    }
}
