using UnityEngine;
using System.Collections;

public class FallSoundCam : MonoBehaviour
{
    Camera body;
    AudioSource sound;

	void Start ()
    {
        body = GetComponent<Camera>();

        sound = GetComponent<AudioSource>();
	}
	
	
	void Update ()
    {
        if (body.velocity.magnitude > 0)
        {
            sound.volume = body.velocity.magnitude / 70;
            if (!sound.isPlaying)
            {
                sound.time = 1;
                sound.Play();
            }
        }
        else
        {
            sound.Stop();
        }
    }
}
