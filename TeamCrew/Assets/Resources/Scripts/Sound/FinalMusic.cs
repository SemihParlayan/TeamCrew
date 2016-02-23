using UnityEngine;
using System.Collections;

public class FinalMusic : MonoBehaviour
{
    private AudioSource finalsound;

    public float activationHeight = 0;

    private Transform cam;
    
    public FadeState fade = FadeState.NONE;
    private float timer = 0;
    public float fadeInSpeed = 1f;
    public float fadeOutSpeed = 1f;
    private bool started = false;

	void Start () 
    {
        finalsound = transform.GetComponent<AudioSource>();

        cam = Camera.main.transform;
	}

    void Update()
    {
        if (!enabled)
            return;
        switch (fade)
        {
            case FadeState.IN:
                //finalsound.volume = Mathf.MoveTowards(finalsound.volume, 1f, Time.deltaTime * fadeInSpeed);
                //float n = (cam.transform.position.y - activationHeight) / (GameManager.LevelHeight - activationHeight);
                //n -= 0.5f;
                finalsound.volume = Mathf.MoveTowards(finalsound.volume, 1f, Time.deltaTime * fadeInSpeed);
                break;
            case FadeState.OUT:

                if (Camera.main.transform.position.y < activationHeight - 5f)
                    started = false;

                if (started)
                    finalsound.volume = Mathf.MoveTowards(finalsound.volume, 0.1f, Time.deltaTime * fadeOutSpeed);
                else
                    finalsound.volume = Mathf.MoveTowards(finalsound.volume, 0.0f, Time.deltaTime * fadeOutSpeed);
                break;
        }
    }

    public void SetStarted(bool state)
    {
        started = state;
    }
    public void SetFadeState(FadeState state)
    {
        if (!enabled)
            return;
        if (finalsound == null)
        {
            finalsound = GetComponent<AudioSource>();
        }

        if (fade == state)
            return;

        fade = state;
        switch (state)
        {
            case FadeState.IN:
                if (started)
                    finalsound.volume = 0.1f;
                else
                    finalsound.volume = 0;

                activationHeight = cam.position.y;
                started = true;
                if (!finalsound.isPlaying)
                    finalsound.Play();
                break;
            case FadeState.OUT:
                break;
            case FadeState.NONE:
                break;
            default: break;

        }
    }
}
