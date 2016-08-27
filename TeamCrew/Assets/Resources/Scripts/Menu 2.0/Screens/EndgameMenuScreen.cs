using UnityEngine;
using System.Collections;

public class EndgameMenuScreen : M_Screen
{
    //References
    public LevelGeneration generator;
    public GameObject menuMountain;
    public M_Screen mainMenuScreen;
    private M_FadeToColor fade;
    private GameManager gameManager;

    //Data
    public Vector3 offsetFromTop;

    protected override void OnAwake()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
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


    public override void OnSwitchedFrom()
    {
        base.OnSwitchedFrom();
        gameManager.ResetWinVariable();
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

        gameManager.ResetGameVariables();
        gameManager.DestroyFrogs();
        gameManager.DestroyTopFrog();
        generator.DestroyLevel(false);
        menuMountain.SetActive(true);
        M_ScreenManager.SetActive(true);
        GameObject.FindWithTag("MenuManager").GetComponent<M_ScreenManager>().enabled = true;
    }
}
