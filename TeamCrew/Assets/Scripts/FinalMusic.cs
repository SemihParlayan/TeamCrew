using UnityEngine;
using System.Collections;

public class FinalMusic : MonoBehaviour
{
    private AudioSource finalsound;
    private Camera cam;
    private float activationHeight = 40;
	void Start () {
        finalsound = transform.GetComponent<AudioSource>();
        cam = Camera.main;
        activationHeight -= 80; //because the world goes downwards D:
	}

    void Update()
    {
        if (cam.transform.position.y > activationHeight)
        {
            PlayFinalMusic();
        }
    }
	public void PlayFinalMusic()
    {
        if(!finalsound.isPlaying) finalsound.Play();
    }

    public void Stop()
    {
        finalsound.Stop();
    }
}
