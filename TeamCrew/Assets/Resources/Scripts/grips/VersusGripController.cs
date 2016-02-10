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
    private float blinkTime = 0;
    private float blinkTimer = 0;
    private float lastBlinkTimer;
    private SpriteRenderer spriteRenderer;

    //Sound
    public AudioSource boilerSound;
    public AudioSource releaseSound;

    

    //Components
    public GripAnimation gripAnimation;
    public float timer;
    public float maxTimer;
    public bool active;
    private bool complete;
    private FrogPrototype frog;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalStartPosition = transform.localPosition;


        boilerSound = gameObject.AddComponent<AudioSource>();
        releaseSound = gameObject.AddComponent<AudioSource>();

        boilerSound.clip = Resources.Load("Audio/Sound/Frog/kettle") as AudioClip;
        releaseSound.clip = Resources.Load("Audio/Sound/Frog/shh") as AudioClip;

        if (boilerSound != null && releaseSound != null)
        {
            boilerSound.playOnAwake = false;
            releaseSound.playOnAwake = false;

            boilerSound.volume = 1f;
            releaseSound.volume = 0.1f;
        }
    }

    void Update()
    {
        if (!active)
            return;        
        if (!frog)
        {
            timer -= Time.deltaTime;
            blinkTime = timer / maxTimer;
        }
        else
        {
            timer -= Time.deltaTime;
            frog.leftGripScript.versusGripController.SetTime(timer, maxTimer);
        }

        if (timer <= 0)
        {
            DeActivate();

            if (releaseSound != null && !releaseSound.isPlaying)
            {
                releaseSound.Play();
            }
        }

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
            if (blinkTime < 0.15f && !shake)
            {
                ActivateShake();
            }
            else
            {
                DeActivateShake();
            }


            blinkTimer += Time.deltaTime;

            //Blink red and white
            if (blinkTimer >= blinkTime / 2)
            {
                if (lastBlinkTimer < blinkTime / 2)
                    gripAnimation.Activate("red");

                if (blinkTime <= 0.6f)
                    spriteRenderer.color = Color.red;
            }
            else
            {
                if (blinkTime <= 0.6f)
                    spriteRenderer.color = Color.white;
            }

            //Stay white
            if (blinkTime > 0.6f)
                spriteRenderer.color = Color.white;

            if (blinkTimer >= blinkTime)
            {
                blinkTimer = 0;
            }
        }

        lastBlinkTimer = blinkTimer;
    }

    public void Activate(float time, FrogPrototype frog = null)
    {
        if (active)
            return;

        this.frog = frog;
        ActivateBlink();
        active = true;
        complete = false;
        maxTimer = time;
        timer = maxTimer;
    }
    public void DeActivate()
    {
        if (!active)
            return;

        if (frog)
        {
            if (frog.leftGripScript.versusGripController.frog == null)
            {
                frog.leftGripScript.versusGripController.frog = frog;
            }
            if (frog.rightGripScript.versusGripController.frog == null)
            {
                frog.rightGripScript.versusGripController.frog = frog;
            }
            frog = null;
        }
        spriteRenderer.color = Color.white;
        active = false;
        complete = true;
        timer = maxTimer = 0;

        if (boilerSound != null && boilerSound.isPlaying)
        {
            boilerSound.Stop();
        }
    }
    public void SetTime(float time, float maxTime)
    {
        timer = time;
        maxTimer = maxTime;
    }
    public bool Complete()
    {
        bool result = complete;

        if (result)
            complete = false;

        return result;
    }
    private void ActivateBlink()
    {
        if (!boilerSound.isPlaying)
        {
            boilerSound.time = 3.0f;
            boilerSound.Play();
        }
        blink = true;
        DeActivateShake();
    }
    private void DeActivateBlink()
    {
        if (boilerSound.isPlaying)
        {
            boilerSound.Stop();
        }
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
