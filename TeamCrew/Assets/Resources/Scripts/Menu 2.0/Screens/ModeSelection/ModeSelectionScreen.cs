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
    public ModeFade modeFade;

    //References
    public M_Screen gameScreenReference;
    public Animator pressXGenerateButton;
    private GameModifiers gameModifiers;
    private PoffMountain poff;
    private GameManager gameManager;
    private M_FadeOnScreenSwitch fadeModifier;

    //Data
    private bool canPressPlay;

    protected override void OnAwake()
    {
        base.OnAwake();
        fadeModifier = GetComponent<M_FadeOnScreenSwitch>();
        poff = GetComponent<PoffMountain>();
        gameManager = GameObject.FindWithTag("GameManager"). GetComponent<GameManager>();

        gameModifiers = gameModes.transform.GetComponent<GameModifiers>();
    }
    protected override void OnUpdate()
    {
        base.OnUpdate();

    /* //X button is no longer relevant since new crazy menu update /seb  
    if (GameManager.GetButtonPress(XboxButton.X))
        {
            poff.PoffRepeating();
            pressXGenerateButton.SetTrigger("press");
        }
        */
    }

    public void generateMountain()
    {
        poff.PoffRepeating();

    }
    public override void OnSwitchedTo()
    {
        base.OnSwitchedTo();
        gameModifiers.OnModifierSelection();
        modeFade.FadeToDesc();
        Invoke("CreateFrogs", 1f);
        DisplayMode();

        gameManager.DestroyTopFrog();
        poff.SetMenuMountainState(false, 0.0f);
    }
    public override void OnSwitchedFrom()
    {
        base.OnSwitchedFrom();
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
            M_ScreenManager.SwitchScreen(gameScreenReference);
            fadeModifier.enabled = true;
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
   
    public void switchToClassic() //Is used by function caller buttons, which is why there is one for each mode. Super hard coded <3 Seb code
    {
        gamemodeIndex = 0;
        DisplayMode();

    }
    public void switchToHard()
    {
        gamemodeIndex = 1;
        DisplayMode();

    }
    public void switchToWheel()
    {
        gamemodeIndex = 2;
        DisplayMode();

    }
    //End of seb code

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
        poff.PoffRepeating();
    }
}
