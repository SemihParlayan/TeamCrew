using UnityEngine;
using System.Collections;

public class ModeSelectionScreen : M_Screen
{
    //Gamemode selection
    public GameModes gameModes;
    public int gamemodeIndex = 0;
    public int tmpGameModeIndex = 0;
    public TextMesh gamemodeText;
    public TextMesh gamemodeDescription;
    public SpriteRenderer gamemodePicture;
    public ModeFade modeFade;
    public AudioSource mountainGenerationSound;
    public Transform selectionArrow;
    public Vector3 selectionArrowTargetPos;

    //References
    public M_Screen gameScreenReference;
    public M_Button[] modeButtons;
    public M_Button continueButton;
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

        selectionArrow.localPosition = Vector3.Lerp(selectionArrow.localPosition, selectionArrowTargetPos, Time.deltaTime * 5);
    }

    public override void OnSwitchedTo()
    {
        base.OnSwitchedTo();
        gameModifiers.OnModifierSelection();
        modeFade.FadeToDesc();
        gamemodeIndex = 0;
        tmpGameModeIndex = 0;
        Invoke("CreateFrogs", 1f);
        UpdateContinueButton();
        GenerateMountain();

        gameManager.DestroyTopFrog();
        poff.SetMenuMountainState(false, 0.0f);
    }
    public void ActivateMenuMountain()
    {

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
   
    private void SwitchMode(int dir)
    {
        int maxIndex = gameModes.gameModes.Count - 1;
        int newIndex = tmpGameModeIndex + dir;

        if (newIndex < 0)
            newIndex = maxIndex;
        else if (newIndex > maxIndex)
            newIndex = 0;

        tmpGameModeIndex = newIndex;
        UpdateInfo(tmpGameModeIndex);
    }
    public void UpdateInfoTMPIndex()
    {
        UpdateInfo(tmpGameModeIndex);
    }
    private void UpdateInfo(int index)
    {
        GameMode mode = gameModes.gameModes[index];
        gamemodePicture.sprite = mode.picture;
        gamemodeDescription.text = mode.description;
        gamemodeText.text = mode.name;
    }
    public void GenerateMountain()
    {
        GameManager.PlayAudioSource(mountainGenerationSound, 0.7f, 1.4f);
        gamemodeIndex = tmpGameModeIndex;
        UpdateContinueButton();
        GameMode mode = gameModes.gameModes[gamemodeIndex];
        GameManager.CurrentGameMode = mode;
        poff.PoffRepeating();

        selectionArrowTargetPos = new Vector3(-2.8f + (3.3f * gamemodeIndex), 2.91f, 0f);
    }
    public void UpdateContinueButton()
    {
        UpdateInfo(gamemodeIndex);
        continueButton.transform.GetComponent<M_ButtonSwitcher>().eventList[0].targetButton = modeButtons[tmpGameModeIndex];
    }
}
