using UnityEngine;
using System.Collections;

public class BurningHands : MonoBehaviour 
{
    private HandGrip handGrip;

    public float gripLimit;
    private float timer;

    void Awake()
    {
        handGrip = GetComponent<HandGrip>();
    }

    void Update()
    {

    }
    private bool invoking = false;
    public void OnUpdate(bool justGripped, bool justReleased)
    {
        if (!enabled)
            return;

        if (justGripped)
        {
            if (!invoking)
            {
                InvokeRepeating("Vibrate", 0.4f, 0.4f);
                invoking = true;
            }
            handGrip.versusGripController.SetState(true, timer, gripLimit);
            handGrip.versusGripController.ActivateBoiler(8 - gripLimit);
            timer = gripLimit;
        }
        if (justReleased)
        {
            CancelInvoke("Vibrate");
            invoking = false;
            handGrip.versusGripController.SetState(false, 0f, 0f);
        }

        timer -= Time.deltaTime;
        handGrip.versusGripController.SetTime(timer, gripLimit);
        if (handGrip.versusGripController.Complete() && handGrip.isOnGrip && !handGrip.isVersusGripping)
        {
            handGrip.ReleaseGrip(0.25f);
        }
    }

    private void Vibrate()
    {
        Vibration.instance.SetVibration(handGrip.player, 0.3f, 0.3f, 0.1f);
    }
}
