using UnityEngine;
using System.Collections;

public class RandomSoundFromList : MonoBehaviour
{
    public AudioClip[] SoundList;
    private AudioSource speaker;

	void Start ()
    {
	
	}

    public void GenerateGrip()
    {
        speaker.clip = SoundList[Random.Range(0, 8)];
    }

    public void GenerateRelease()
    {
        speaker.clip = SoundList[9];
    }

}
