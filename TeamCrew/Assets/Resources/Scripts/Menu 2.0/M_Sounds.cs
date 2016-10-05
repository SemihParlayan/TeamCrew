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
    public AudioMixerGroup vineSnapGroup;
    public static AudioMixerGroup vineSnapGroupStatic;
    public M_SliderButton masterSlider;
    public M_SliderButton sfxSlider;
    public M_SliderButton musicSlider;
    public M_SliderButton environmentSlider;

    void Awake()
    {
        vineSnapGroupStatic = vineSnapGroup;
    }
    public void Update()
    {
        if (!GameManager.AllowSounds)
        {
            mixer.SetFloat("MasterVolume", -80f);
            return;
        }

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
        if (playMenuMusic)
        {
            SetMixerVolume("MasterVolume", masterSlider);
            SetMixerVolume("SFXVolume", sfxSlider);
            SetMixerVolume("MusicVolume", musicSlider);
            SetMixerVolume("EnvironmentVolume", environmentSlider);
        }

        if (M_ScreenManager.GetCurrentScreen() is OptionScreen)
        {
            if (musicSlider.pressed || masterSlider.pressed)
                SetMixerVolume("MusicVolume", musicSlider);
            else
                mixer.SetFloat("MusicVolume", -80f);
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

    public void SetMixerVolume(string parameter, M_SliderButton slider)
    {
        float muteValue = 35f;

        float oldRange = 1;
        float newRange = muteValue;
        float sliderValue = slider.value;
        float newValue = ((sliderValue * newRange) / oldRange) - muteValue;
        if (newValue <= -muteValue)
            newValue = -80;

        bool worked = mixer.SetFloat(parameter, newValue);
    }
}
