using UnityEngine;
using System.Collections;

public class EndgameMenuScreen : M_Screen
{
    //References
    public LevelGeneration generator;
    public GameObject menuMountain;
    public M_Screen mainMenuScreen;
    private M_FadeToColor fade;

    //Data
    public Vector3 offsetFromTop;

    protected override void OnAwake()
    {
        generator = GameObject.FindWithTag("GameManager").GetComponent<LevelGeneration>();
        fade = GetComponent<M_FadeToColor>();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (fade.Halfway)
        {
            ActivateMenuMountain();
            SwitchScreen(mainMenuScreen);
            M_ScreenManager.TeleportToCurrentScreen();
        }
    }


    public override void OnSwitchedTo()
    {
        base.OnSwitchedTo();

        GameObject.FindWithTag("GameManager").GetComponent<GameManager>().SetInactivityState(true, 15f);
        movementProperties.cameraLocation.position += offsetFromTop;
    }

    public void FadeToBlack()
    {
        fade.StartFade();
    }

    public void ActivateMenuMountain()
    {
        if (menuMountain.activeInHierarchy)
            return;

        generator.DestroyLevel(false);
        menuMountain.SetActive(true);
    }
}
