using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class PauseController : MonoBehaviour 
{
    public M_Sounds soundManager;
    public AudioMixer mixer;
    public M_Screen pauseScreen;
    public M_Screen pauseScreenDailymountain;
    public EndgameMenuScreen endgameMenuScreen;
    public ModeSelectionScreen modeSelectionScreen;
    public DailyMountainScreen dailyMountainScreen;
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
                if (GameManager.GetPlayer(i).GetButtonDown("Start"))
                {
                    PauseButton();
                }
            }
        }
            

        if (paused && pauseScreen.enabled)
        {
            pauseScreen.movementProperties.zoom = Camera.main.orthographicSize;
        }
        if (paused && pauseScreenDailymountain.enabled)
        {
            pauseScreenDailymountain.movementProperties.zoom = Camera.main.orthographicSize;
        }
    }

    public void PauseButton()
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

        if (gameManager.isInDailyMountain)
        {
            M_ScreenManager.SwitchScreen(pauseScreenDailymountain);

            pauseScreenDailymountain.active = true;
            pauseScreenDailymountain.enabled = true;
        }
        else
        {
            M_ScreenManager.SwitchScreen(pauseScreen);

            pauseScreen.active = true;
            pauseScreen.enabled = true;
        }

        Vector3 camPos = Camera.main.transform.position; camPos.z = 0;
        pauseScreen.movementProperties.cameraLocation.position = camPos;
        pauseScreenDailymountain.movementProperties.cameraLocation.position = camPos;

    }
    void UnPause()
    {
        mixer.SetFloat("EnvironmentVolume", environmentVolume);
        mixer.SetFloat("SFXVolume", sfxVolume);
        mixer.SetFloat("MusicVolume", musicVolume);

        Time.timeScale = 1;
        M_ScreenManager.SetActive(false);

        if (gameManager.isInDailyMountain)
        {
            pauseScreenDailymountain.active = false;
            pauseScreenDailymountain.enabled = false;
            pauseScreenDailymountain.CancelInvoke();
        }
        else
        {
            pauseScreen.active = false;
            pauseScreen.enabled = false;
            pauseScreen.CancelInvoke();
        }
    }

    public void RestartDaily()
    {
        paused = false;
        UnPause();
        dailyMountainScreen.RestartDaily();
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
