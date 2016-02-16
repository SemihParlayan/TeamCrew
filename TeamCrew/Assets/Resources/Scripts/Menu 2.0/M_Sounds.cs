using UnityEngine;
using System.Collections;

public class M_Sounds : MonoBehaviour 
{
    public AudioClip buttonClick;
    public AudioClip buttonSwitch;

    public AudioSource menuMusic;
    public bool playMenuMusic = true;

    public void Update()
    {
        //Menu music
        if (playMenuMusic)
        {
            menuMusic.volume = Mathf.MoveTowards(menuMusic.volume, 1f, Time.deltaTime);
        }
        else
        {
            if (menuMusic.volume >= 0.1f)
            {
                menuMusic.volume = Mathf.MoveTowards(menuMusic.volume, 0f, Time.deltaTime);
            }
            else
            {
                if (menuMusic.isPlaying)
                    menuMusic.Stop();
            }
        }
    }

    public void StartMenuMusic()
    {
        if (playMenuMusic)
            return;

        playMenuMusic = true;
        menuMusic.Play();
    }
    public void StopMenuMusic()
    {
        playMenuMusic = false;
    }
}
