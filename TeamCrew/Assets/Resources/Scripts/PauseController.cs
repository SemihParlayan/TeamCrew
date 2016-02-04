using UnityEngine;
using System.Collections;

public class PauseController : MonoBehaviour 
{
    public M_Screen pauseScreen;
    private GameManager gameManager;
    private M_ScreenManager screenManager;
    private bool paused;

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
