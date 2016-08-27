using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class PauseController : MonoBehaviour 
{
    public M_Sounds soundManager;
    public AudioMixer mixer;
    public M_Screen pauseScreen;
    public EndgameMenuScreen endgameMenuScreen;
    public ModeSelectionScreen modeSelectionScreen;
    public DailyMountainScreen dailyMountainScreen;
    public TextMesh backToPreviosButtonText;
    private GameManager gameManager;
    private M_ScreenManager screenManager;
    public KOTH koth;
    private bool paused;
    private float sfxVolume;
    private float environmentVolume;
    private float musicVolume;

    void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        screenManager = GameObject.FindWithTag("MenuManager").GetComponent<M_ScreenManager>();
    }

    void Update()
    {
        if (!gameManager.gameActive)
            return;

        for (int i = 0; i < gameManager.frogsReady.Length; i++)
        {
            if (gameManager.frogsReady[i])
            {
                if (GameManager.GetButtonPress(XboxButton.Start, i))
                {
                    PauseButton();
                }
            }
        }
            

        if (paused && pauseScreen.enabled)
        {
            pauseScreen.movementProperties.zoom = Camera.main.orthographicSize;
        }
    }

    void PauseButton()
    {
        paused = !paused;

        if (paused)
            Pause();
        else
            UnPause();
    }
    void Pause()
    {
        mixer.GetFloat("SFXVolume", out sfxVolume);
        mixer.GetFloat("EnvironmentVolume", out environmentVolume);
        mixer.GetFloat("MusicVolume", out musicVolume);

        mixer.SetFloat("EnvironmentVolume", -80f);
        mixer.SetFloat("SFXVolume", -80f);
        mixer.SetFloat("MusicVolume", musicVolume - (Mathf.Abs(musicVolume) / 2));

        Time.timeScale = 0;
        M_ScreenManager.SetActive(true);
        M_ScreenManager.SwitchScreen(pauseScreen);

        pauseScreen.active = true;
        pauseScreen.enabled = true;

        Vector3 camPos = Camera.main.transform.position; camPos.z = 0;
        pauseScreen.movementProperties.cameraLocation.position = camPos;

        if (gameManager.isInDailyMountain)
        {
            backToPreviosButtonText.text = "Daily mountain";
        }
        else
        {
            backToPreviosButtonText.text = "Mode selection";
        }
    }
    void UnPause()
    {
        mixer.SetFloat("EnvironmentVolume", environmentVolume);
        mixer.SetFloat("SFXVolume", sfxVolume);
        mixer.SetFloat("MusicVolume", musicVolume);

        Time.timeScale = 1;
        M_ScreenManager.SetActive(false);

        pauseScreen.active = false;
        pauseScreen.enabled = false;
        pauseScreen.CancelInvoke();
    }

    void GoBackToPrevious()
    {
        paused = false;
        UnPause();
        M_ScreenManager.SetActive(true);


        if (gameManager.isInDailyMountain)
        {
            M_ScreenManager.SwitchScreen(dailyMountainScreen);
        }
        else
        {
            M_ScreenManager.SwitchScreen(modeSelectionScreen);
        }

        gameManager.ResetGameVariables();
        soundManager.StartMenuMusic();
    }
    void GoToMainMenu()
    {
        koth.DisableKeepers();
        paused = false;
        UnPause();
        endgameMenuScreen.ActivateMenuMountain();
        M_ScreenManager.SetActive(true);
        soundManager.StartMenuMusic();
    }
}
