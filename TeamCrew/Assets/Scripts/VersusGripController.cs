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
    private float blinkTimer = 0;
    private float lastBlinkTimer;
    private SpriteRenderer renderer;

    //Components
    public GripAnimation gripAnimation;

    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        originalStartPosition = transform.localPosition;
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
            if (blinkTime < 0.15 && !shake)
            {
                ActivateShake();
            }
            blinkTimer += Time.deltaTime;

            if (blinkTimer >= blinkTime / 2)
            {
                if (lastBlinkTimer < blinkTime / 2)
                    gripAnimation.Activate("red");

                renderer.color = Color.red;
            }
            else
            {
                renderer.color = Color.white;
            }

            if (blinkTimer >= blinkTime)
            {
                blinkTimer = 0;
            }
        }

        lastBlinkTimer = blinkTimer;
    }
    public void ActivateBlink()
    {
        blink = true;
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
