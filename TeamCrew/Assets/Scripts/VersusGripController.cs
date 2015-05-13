using UnityEngine;
using System.Collections;

public class VersusGripController : MonoBehaviour 
{
    //Shake
    private bool shake = false;
    private int shakeDir = 1;
    private float shakeTimer = 0;
    private float shakeTurnDelay = 0.05f;
    private Vector3 originalStartPosition;

    //Blink
    private bool blink = false;
    public float blinkTime = 0;
    public float blinkTimer = 0;
    private float lastBlinkTimer;
    private SpriteRenderer renderer;

    //Sound Stuff
    private AudioSource sounder;
    private AudioClip boilSound;

    //Components
    public GripAnimation gripAnimation;

    void Start()
    {
        gameObject.AddComponent<AudioSource>();
        sounder  = GetComponent<AudioSource>();

        renderer = GetComponent<SpriteRenderer>();
        originalStartPosition = transform.localPosition;

        boilSound = Resources.Load("kettle") as AudioClip;
        if (boilSound == null) Debug.Log("boil sound is null");

        sounder.clip = boilSound;
        
    }

    void Update()
    {
        //Shake
        if (shake)
        {
            shakeTimer += Time.deltaTime;
            if (shakeTimer >= shakeTurnDelay)
            {
                shakeTimer -= shakeTurnDelay;
                shakeDir *= -1;
            }

            transform.localPosition = originalStartPosition;
            Vector3 dir = Random.insideUnitSphere;
            dir.z = 0;
            transform.localPosition += dir * shakeDir * Time.deltaTime * 4.5f;
        }

        if (blink)
        {
            if (!sounder.isPlaying)
            {
                sounder.time = blinkTimer;
                sounder.Play();
            }


            if (blinkTime < 0.15f && !shake)
            {
                ActivateShake();
            }
            else
            {
                DeActivateShake();
            }
            blinkTimer += Time.deltaTime;

            if (blinkTimer >= blinkTime / 2)
            {
                if (lastBlinkTimer < blinkTime / 2)
                    gripAnimation.Activate("red");

                if (blinkTime <= 0.6f)
                    renderer.color = Color.red;
            }
            else
            {
                if (blinkTime <= 0.6f)
                    renderer.color = Color.white;
            }

            if (blinkTime > 0.6f)
                renderer.color = Color.white;

            if (blinkTimer >= blinkTime)
            {
                blinkTimer = 0;
            }
        }
        else
        {
            if(sounder.isPlaying)
            {
                sounder.Stop();
            }
        }

        lastBlinkTimer = blinkTimer;
    }
    public void ActivateBlink()
    {
        blink = true;
        DeActivateShake();
    }
    public void DeActivateBlink()
    {
        blink = false;
        DeActivateShake();
    }
    private void ActivateShake()
    {
        shake = true;
    }
    private void DeActivateShake()
    {
        shake = false;
        transform.localPosition = originalStartPosition;
    }
}
