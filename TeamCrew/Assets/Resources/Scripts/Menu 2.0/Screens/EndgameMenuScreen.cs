using UnityEngine;
using System.Collections;

public class EndgameMenuScreen : M_Screen
{
    //References
    public SpriteRenderer fadeToBlack;
    public LevelGeneration generator;
    public GameObject menuMountain;
    public M_Screen mainMenuScreen;

    //Data
    public Vector3 offsetFromTop;
    private bool fade;

    protected override void OnStart()
    {
        generator = GameObject.FindWithTag("GameManager").GetComponent<LevelGeneration>();
    }

    protected override void OnUpdate()
    {
        if (fade)
        {
            Color c = fadeToBlack.color;
            c.a = Mathf.MoveTowards(c.a, 1f, Time.deltaTime);
            fadeToBlack.color = c;

            if (c.a >= 1f)
            {
                ActivateMenuMountain();
                SwitchScreen(mainMenuScreen);
                M_ScreenManager.TeleportToCurrentScreen();
            }
        }
    }

    public override void OnSwitchedTo()
    {
        base.OnSwitchedTo();

        movementProperties.cameraLocation.position += offsetFromTop;
    }

    public void FadeToBlack()
    {
        fade = true;
        Color c = Color.black;
        c.a = 0;
        fadeToBlack.color = c;
    }

    public void ActivateMenuMountain()
    {
        if (menuMountain.activeInHierarchy)
            return;

        generator.DestroyLevel(false);
        menuMountain.SetActive(true);
    }
}
