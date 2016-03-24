using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class PauseController : MonoBehaviour 
{
    public AudioMixer mixer;
    public M_Screen pauseScreen;
    private GameManager gameManager;
    private M_ScreenManager screenManager;
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

        if (GameManager.GetButtonPress(XboxButton.Start))
        {
            PauseButton();
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

    void GoToModeSelection()
    {
        paused = false;
        UnPause();
        M_ScreenManager.SetActive(true);
        gameManager.ResetGameVariables();
    }
}
