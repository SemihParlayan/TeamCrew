using UnityEngine;
using System.Collections;

public class OptionsTestSound : MonoBehaviour 
{
    private M_Sounds sounds;
    public AudioSource sfxTest;
    public AudioSource environmentTest;

    void Start()
    {
        sounds = GetComponent<M_Sounds>();
    }

    void Update()
    {
        if (sounds.environmentSlider.pressed || sounds.masterSlider.pressed)
        {
            if (!environmentTest.isPlaying)
                environmentTest.Play();
        }
        else
        {
            environmentTest.Stop();
        }

        if (sounds.sfxSlider.pressed && !sfxTest.isPlaying)
        {
            sfxTest.Play();
        }
    }
}
