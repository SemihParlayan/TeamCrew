using UnityEngine;
using System.Collections;

public class StartScreen : M_Screen
{
    //References
    public M_Sounds soundManager;
    public GameModifiers gameModifiers;
    public GameObject frogClimbersLogo;
    private GameManager gameManagerReference;

    protected override void OnAwake()
    {
        base.OnAwake();
        gameManagerReference = GameObject.FindWithTag("GameManager").transform.GetComponent<GameManager>();
    }

    public override void OnSwitchedTo()
    {
        base.OnSwitchedTo();

        //Activate logo
        frogClimbersLogo.SetActive(true);

        //Deactivate all modifiers
        if (gameModifiers != null)
            gameModifiers.DeactivateAllModifiers();

        gameManagerReference.LockParallaxes(false);
        gameManagerReference.SetInactivityState(false, 15f);
        soundManager.StartMenuMusic();
    }
    public override void OnSwitchedFrom()
    {
        base.OnSwitchedFrom();
    }

    public void StopMenuMusic()
    {
        soundManager.StopMenuMusic();
    }
}
