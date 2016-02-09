using UnityEngine;
using System.Collections;

public class BurningHands : MonoBehaviour 
{
    private HandGrip handGrip;

    public float gripLimit;

    void Awake()
    {
        handGrip = GetComponent<HandGrip>();
    }

    void Update()
    {

    }
    public void OnUpdate(bool justGripped, bool justReleased)
    {
        if (!enabled)
            return;

        if (handGrip.versusGripController.Complete() && handGrip.isOnGrip && !handGrip.isVersusGripping)
        {
            handGrip.ReleaseGrip(0.25f);
            handGrip.versusGripController.DeActivate();
        }


        if (justGripped)
        {
            handGrip.versusGripController.Activate(gripLimit);
        }
        if (justReleased)
        {
            handGrip.versusGripController.DeActivate();
        }
    }
}
