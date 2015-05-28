using UnityEngine;
using System.Collections;

public class FinalMusic : MonoBehaviour
{
    public float activationHeight = 40;
    public float fadeInDuration = 30;
    public float fadeOutDuration = 1;
    public float fadeInMaxHeight = 20;


    private float originalActivationHeight;
    private AudioSource finalsound;
    private Camera cam;
    private GameManager gameManager;
    
    
    private Fade fade;
    private float timer = 0;
    private bool first = true;

    private float gameMaxHeight = 80;


	void Start () {
        originalActivationHeight = activationHeight;
        Debug.Log("original Activation height is "+ originalActivationHeight);
        finalsound = transform.GetComponent<AudioSource>();
        cam = Camera.main;
        activationHeight -= gameMaxHeight; //because the world goes downwards D:
        ChangeFadeState(Fade.nones);

        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
	}

    void OnEnable()
    {
        if (first)
        {
            first = false;
            return;
        }
        finalsound = transform.GetComponent<AudioSource>();
        cam = Camera.main;
        activationHeight = originalActivationHeight - 80; //because the world goes downwards D:
        ChangeFadeState(Fade.nones);
    }

    void Update()
    {
        if (cam.transform.position.y > activationHeight)
        {
            PlayFinalMusic();
            switch (fade)
            {
                case Fade.nones:
                {
                    float max = fadeInMaxHeight - activationHeight;
                    float camY = cam.transform.position.y - activationHeight;
                    finalsound.volume = camY / max;
                    break;
                }
                

                case Fade.ins:
                    {
                        timer += Time.deltaTime;
                        if (timer >= fadeInDuration)
                        {
                            finalsound.volume = 1;
                            ChangeFadeState(Fade.nones);
                        }
                        else finalsound.volume = (timer / fadeInDuration);

                        break;
                    }

                case Fade.outs:
                    {
                        timer += Time.deltaTime;
                        if (timer >= fadeOutDuration)
                        {
                            Stop();
                            finalsound.volume = 0;
                            ChangeFadeState(Fade.nones);
                            enabled = false;
                        }
                        else finalsound.volume = 1 - (timer / fadeOutDuration);

                        break;
                    }
            }
        }
    }
	public void PlayFinalMusic()
    {
        if (!finalsound.isPlaying && gameManager.gameActive) 
            finalsound.Play();
    }

    public void Stop()
    {
        finalsound.Stop();
    }

    public void ChangeFadeState(Fade state)
    {
        if (finalsound == null)
        {
            finalsound = GetComponent<AudioSource>();
        }
        switch (state)
        {
            case Fade.ins:
                timer = 0;
                finalsound.volume = 0;
                fade = Fade.ins;
                break;
            case Fade.outs:
                timer = 0;
                finalsound.volume = 1;
                fade = Fade.outs;

                break;
            case Fade.nones:
                fade = Fade.nones;

                break;
            default: break;

        }
    }

}
