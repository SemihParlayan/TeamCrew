using UnityEngine;
using System.Collections;

public class SoundPlay : MonoBehaviour
{
    public enum Fade
    {
        ins,
        outs,
        nones
    }

    private Fade fade = Fade.ins;


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

    public void Update()
    {
        //Time.deltaTime;
    }

    public void ChangeFadeState(Fade state)
    {
    }
}
