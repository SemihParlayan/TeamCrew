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

	void Start () 
    {
        finalsound = transform.GetComponent<AudioSource>();

        cam = Camera.main.transform;
	}

    void Update()
    {
        switch(fade)
        {
            case FadeState.IN:
                //finalsound.volume = Mathf.MoveTowards(finalsound.volume, 1f, Time.deltaTime * fadeInSpeed);
                float n = (cam.transform.position.y - activationHeight) / (GameManager.LevelHeight - activationHeight);
                n -= 0.5f;
                finalsound.volume = Mathf.MoveTowards(finalsound.volume, n, Time.deltaTime);
                break;
            case FadeState.OUT:
                finalsound.volume = Mathf.MoveTowards(finalsound.volume, 0f, Time.deltaTime * fadeOutSpeed);
                break;
        }
        //switch (fade)
        //{
        //    case FadeState.NONE:
        //        NoFadeUpdate(); break;

        //    case FadeState.IN:
        //        FadeInUpdate(); break;

        //    case FadeState.OUT:
        //        FadeOutUpdate(); break;
        //}
    }

    public void SetFadeState(FadeState state)
    {
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
                finalsound.volume = 0;
                activationHeight = cam.position.y;

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
