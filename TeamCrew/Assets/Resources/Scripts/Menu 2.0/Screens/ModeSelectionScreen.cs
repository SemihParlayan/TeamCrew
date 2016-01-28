using UnityEngine;
using System.Collections;

public class ModeSelectionScreen : M_Screen
{
    //Gamemode selection
    public GameModes gameModes;
    public int gamemodeIndex = 0;
    public TextMesh gamemodeText;
    public TextMesh gamemodeDescription;
    public SpriteRenderer gamemodePicture;

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
    public override void OnSwitchedTo()
    {
        base.OnSwitchedTo();
        canPressPlay = false;
        DisplayMode();

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

    //Mode selection left
    public void OnLeft()
    {
        SwitchMode(-1);
    }
    //Mode selection right
    public void OnRight()
    {
        SwitchMode(1);
    }

    //Switch gamemode
    private void SwitchMode(int dir)
    {
        int maxIndex = gameModes.gameModes.Count - 1;
        int newIndex = gamemodeIndex + dir;

        if (newIndex < 0)
            newIndex = maxIndex;
        else if (newIndex > maxIndex)
            newIndex = 0;

        gamemodeIndex = newIndex;
        DisplayMode();
    }

    private void DisplayMode()
    {
        GameMode mode = gameModes.gameModes[gamemodeIndex];

        GameManager.CurrentGameMode = mode;
        gamemodePicture.sprite = mode.picture;
        gamemodeDescription.text = mode.description;
        gamemodeText.text = mode.name;
    }
}
