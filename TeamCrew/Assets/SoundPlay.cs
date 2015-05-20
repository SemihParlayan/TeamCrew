using UnityEngine;
using System.Collections;
public class SoundPlay : MonoBehaviour
{
    private AudioSource speaker;
    void Start()
    {
        speaker = transform.GetComponent<AudioSource>();

        Debug.Log("didplay");
        Invoke("Play", .1f);
    }

    public void Play()
    {
        speaker.Play();
    }
}
