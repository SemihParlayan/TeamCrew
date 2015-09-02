using UnityEngine;
using System.Collections;

public class FinalMusic : MonoBehaviour
{
    private AudioSource finalsound;
    private GameManager gameManager;


    public float activationHeight = -40;

    private Transform cam;
    
    public FadeState fade = FadeState.NONE;
    private float timer = 0;
    public float fadeInDuration = 30;
    public float fadeOutDuration = 1;

	void Start () {
        finalsound = transform.GetComponent<AudioSource>();
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        cam = Camera.main.transform;
        //SetFadeState(FadeState.NONE);
	}

    void OnEnable()
    {
        //Copied from Start()
        finalsound = transform.GetComponent<AudioSource>();
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        cam = Camera.main.transform;
        //SetFadeState(FadeState.NONE);
        //===================================
        PlayFinalMusic();

        Debug.Log("Final Stretch Music enabled");
    }

    void OnDisable()
    {
        Debug.Log("Final Stretch Music Disabled");
    }

    void Update()
    {
        switch (fade)
        {
            case FadeState.NONE:
                NoFadeUpdate(); break;

            case FadeState.IN:
                FadeInUpdate(); break;

            case FadeState.OUT:
                FadeOutUpdate(); break;
        }
    }

    void NoFadeUpdate()
    {
        
        finalsound.volume = 1f - Mathf.Abs((cam.position.y) / (activationHeight));
    }

    void FadeInUpdate()
    {
        timer += Time.deltaTime;
        if (timer >= fadeInDuration)
        {
            finalsound.volume = 1;
            SetFadeState(FadeState.NONE);
        }
        else finalsound.volume = (timer / fadeInDuration);
    }

    void FadeOutUpdate()
    {
        timer += Time.deltaTime;
        if (timer >= fadeOutDuration)
        {
            Stop();
            finalsound.volume = 0;
            SetFadeState(FadeState.NONE);
            enabled = false;
        }
        else finalsound.volume = 1 - (timer / fadeOutDuration);
    }
	public void PlayFinalMusic()
    {
        if (!finalsound.isPlaying && gameManager.gameActive)
        {
            finalsound.Play();
            SetFadeState(FadeState.NONE);
        }
    }

    public void Stop()
    {
        finalsound.Stop();
    }

    public void SetFadeState(FadeState state)
    {
        if (finalsound == null)
        {
            finalsound = GetComponent<AudioSource>();
        }
        switch (state)
        {
            case FadeState.IN:
                PlayFinalMusic();
                timer = 0;
                finalsound.volume = 0;
                fade = FadeState.IN;
                break;
            case FadeState.OUT:
                timer = 0;
                fade = FadeState.OUT;

                break;
            case FadeState.NONE:
                fade = FadeState.NONE;
                break;
            default: break;

        }
    }

    
}
