using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class VersusGripController : MonoBehaviour 
{
    //Shake
    private bool shake = false;
    private int shakeDir = 1;
    private float shakeTimer = 0;
    private float shakeTurnDelay = 0.05f;
    private Vector3 originalStartPosition;

    private SpriteRenderer spriteRenderer;

    //Sound
    public AudioSource boilerSound;
    public AudioSource releaseSound;

    //Components
    public GripAnimation gripAnimation;


    public float blinkTimer;
    public float blinkDelay;

    public float timer;
    public float maxTimer;
    private float normal;
    private bool isVersusGripping;
    private bool complete;
    public bool active;

    void Awake()
    {
        AudioSource originalSource = GetComponent<AudioSource>();
        AudioMixerGroup mixerGroup = null;
        if (originalSource)
            mixerGroup = originalSource.outputAudioMixerGroup;

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalStartPosition = transform.localPosition;

        if (boilerSound != null && releaseSound != null)
        {
            boilerSound.playOnAwake = false;
            boilerSound.spatialBlend = 0.0f;

            releaseSound.playOnAwake = false;
            releaseSound.spatialBlend = 0.0f;

            boilerSound.volume = 1f;
            releaseSound.volume = 0.2f;
        }

        SetState(false, 0f, 0f);
    }

    void Update()
    {
        if (!active)
            return;

        normal = timer / maxTimer;

        if (normal <= 0)
        {
            complete = true;
            active = false;
            if (!releaseSound.isPlaying)
            {
                releaseSound.Play();
                boilerSound.Stop();
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


        //Blink red and white
        blinkTimer += Time.deltaTime;
        if (blinkTimer >= blinkDelay)
        {
            blinkTimer = 0;
            blinkDelay = (maxTimer / 4) * normal;
            spriteRenderer.color = (spriteRenderer.color == Color.red) ? Color.white : Color.red;

            gripAnimation.Activate("red");
        }
    }


    public void SetState(bool state, float time, float maxTime)
    {
        active = state;

        blinkDelay = 0f;
        blinkTimer = 0f;
        timer = 0f;
        maxTimer = 0f;

        if (active)
        {
            this.timer = time;
            this.maxTimer = maxTime;

            blinkTimer = 0;
            blinkDelay = (maxTimer / 4);
        }
        else
        {
            if (boilerSound != null)
                boilerSound.Stop();
        }
    }
    public void ActivateBoiler(float startTime)
    {
        boilerSound.Stop();

        boilerSound.Play();
        boilerSound.time = Mathf.Abs(startTime);
    }
    public void SetTime(float time, float maxTime, bool isVersusGripping = false)
    {
        if (!active)
            return;

        this.timer = time;
        this.maxTimer = maxTime;
        this.isVersusGripping = isVersusGripping;
    }
    public bool Complete()
    {
        if (complete)
        {
            complete = false;
            return true;
        }
        else
            return false;
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
