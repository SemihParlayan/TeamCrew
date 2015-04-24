using UnityEngine;
using System.Collections;

public class RandomSoundFromList : MonoBehaviour
{
    public AudioClip[] SoundList;
    private AudioSource speaker;

	void Start ()
    {
        speaker = GetComponent<AudioSource>();
	}

    public void GenerateGrip()
    {
        speaker.pitch = 1;
        speaker.volume = 1;
        speaker.clip = SoundList[Random.Range(0, 8)];
    }

    public void GenerateRelease()
    {
        speaker.volume = Random.Range(0.5f, 1.0f);
        speaker.pitch = Random.Range(1.0f, 2.0f);
        speaker.clip = SoundList[9];
    }

}
