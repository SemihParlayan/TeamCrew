using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class M_Sounds : MonoBehaviour 
{
    public AudioSource buttonClick;
    public AudioSource buttonSwitch;

    public AudioSource menuMusic;
    public bool playMenuMusic = true;

    public AudioMixer mixer;
    public M_SliderButton masterSlider;
    public M_SliderButton sfxSlider;
    public M_SliderButton musicSlider;
    public M_SliderButton environmentSlider;

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

        //Mixer
        SetMixerVolume("MasterVolume", masterSlider);
        SetMixerVolume("SFXVolume", sfxSlider);
        SetMixerVolume("MusicVolume", musicSlider);
        SetMixerVolume("EnvironmentVolume", environmentSlider);
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

    private void SetMixerVolume(string parameter, M_SliderButton slider)
    {
        float oldRange = 1;
        float newRange = 80;
        float newValue = ((slider.value * newRange) / oldRange) - 80;
        bool worked = mixer.SetFloat(parameter, newValue);
    }
}
