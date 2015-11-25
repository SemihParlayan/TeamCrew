using UnityEngine;
using System.Collections;

public class ModeSelectionScreen : M_Screen
{
    //References
    public M_Screen gameScreenReference;
    private PoffMountain poff;
    private GameManager gameManager;

    //Data
    private bool canPressPlay;

    protected override void OnAwake()
    {
        base.OnAwake();
        poff = GetComponent<PoffMountain>();
        gameManager = GameObject.FindWithTag("GameManager"). GetComponent<GameManager>();
    }

    public void ModeLeft()
    {

    }
    public void ModeRight()
    {

    }

    public override void OnSwitchedTo()
    {
        base.OnSwitchedTo();
        canPressPlay = false;

        gameManager.DestroyTopFrog();
        poff.SetMenuMountainState(false, 2.5f);
        CancelInvoke("CreateFrogs");
        Invoke("CreateFrogs", 2.6f);
    }
    public override void OnSwitchedFrom()
    {
        base.OnSwitchedFrom();
        poff.SetPoffState(false);
    }
    private void CreateFrogs()
    {
        gameManager.CreateNewFrogs();
        canPressPlay = true;
    }

    public void LockParallaxes()
    {
        gameManager.LockParallaxes(true);
    }
    //Play button
    public void Play()
    {
        if (canPressPlay)
        {
            poff.SetPoffState(false);
            M_ScreenManager.SwitchScreen(gameScreenReference);
        }
    }
    //Go back to character selection ( RETURN BUTTON );
    public void GoToCharacterSelection()
    {
        gameManager.DestroyFrogs();
        poff.SetMenuMountainState(true, 0.0f);
    }
}
