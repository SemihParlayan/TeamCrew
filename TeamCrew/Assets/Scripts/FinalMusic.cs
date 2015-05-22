using UnityEngine;
using System.Collections;

public class FinalMusic : MonoBehaviour
{
    private AudioSource finalsound;
    private Camera cam;
	void Start () {
        finalsound = transform.GetComponent<AudioSource>();
        cam = Camera.main;
	}

    void Update()
    {
        if (cam.transform.position.y > -40)
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
