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
        if (!sound.enabled)
            return;

        Vector2 thing = new Vector2(body.velocity.x, body.velocity.y); // do not want z

        if (thing.magnitude > .5f)
        {
            sound.volume = thing.magnitude / 70;
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
