using UnityEngine;
using System.Collections;

public class BurningHands : MonoBehaviour 
{
    private HandGrip handGrip;

    public float gripLimit;
    private float timer;
    private GameManager gamemanager;

    void Awake()
    {
        handGrip = GetComponent<HandGrip>();
        gamemanager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {

    }
    //private bool invoking = false;
    public void OnUpdate(bool justGripped, bool justReleased)
    {
        //if (!enabled || !gamemanager.tutorialComplete)
        //    return;

        if (!enabled)
            return;

        if (!gamemanager.tutorialComplete && handGrip.isGrippingTutorial)
            return;

        if (justGripped)
        {
            handGrip.versusGripController.SetState(true, timer, gripLimit);
            handGrip.versusGripController.ActivateBoiler(8 - gripLimit);
            timer = gripLimit;
        }
        if (justReleased)
        {
            handGrip.versusGripController.SetState(false, 0f, 0f);
        }

        timer -= Time.deltaTime;
        handGrip.versusGripController.SetTime(timer, gripLimit);
        if (handGrip.versusGripController.Complete() && handGrip.isOnGrip && !handGrip.isVersusGripping)
        {
            handGrip.ReleaseGrip(0.25f);
        }
    }
}
