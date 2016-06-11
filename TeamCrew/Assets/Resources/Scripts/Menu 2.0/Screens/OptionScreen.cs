using UnityEngine;
using System.Collections;

public class OptionScreen : M_Screen
{
    public SpriteRenderer fullscreenCheckbox;

    void Awake()
    {
        int screenWidth = PlayerPrefs.GetInt("ScreenWidth", Screen.width);
        int screenHeight = PlayerPrefs.GetInt("ScreenHeight", Screen.height);

        Screen.SetResolution(screenWidth, screenHeight, Screen.fullScreen);
        fullscreenCheckbox.enabled = Screen.fullScreen;
    }
    public void OnFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}
