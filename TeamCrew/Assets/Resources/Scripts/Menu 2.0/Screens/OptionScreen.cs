using UnityEngine;
using System.Collections;

public class OptionScreen : M_Screen
{
    public SpriteRenderer fullscreenCheckbox;
    public SpriteRenderer vsyncCheckbox;
    public TextMesh resolutionText;
    public M_SliderButton resolutionSlider;

    protected override void OnAwake()
    {
        base.OnAwake();
        bool fullScreen = PlayerPrefs.GetInt("FullScreen", 1) == 1 ? true : false;

        QualitySettings.SetQualityLevel(5);

        //Initialize resolution slider
        float nrResolutions = Screen.resolutions.Length;
        float stepSize = 1f / nrResolutions;

        for (int i = 0; i < nrResolutions; i++)
        {
            Resolution c = Screen.resolutions[i];

            if (c.width == Screen.currentResolution.width && c.height == Screen.currentResolution.height)
            {
                resolutionIndex = i;
                resolutionSlider.SetValue(stepSize * resolutionIndex);
                break;
            }
        }
    }
    protected override void OnUpdate()
    {
        base.OnUpdate();

        HandleResolutionSlider();

        fullscreenCheckbox.enabled = Screen.fullScreen;
        vsyncCheckbox.enabled = QualitySettings.vSyncCount != 0;

        if (!resolutionSlider.pressed && Screen.fullScreen)
            resolutionText.text = Screen.currentResolution.width + " x " + Screen.currentResolution.height;
        else
            resolutionText.text = Screen.resolutions[resolutionIndex].width + " x " + Screen.resolutions[resolutionIndex].height;
    }
    protected override void OnStart()
    {
        base.OnStart();
    }
    public void OnFullScreen()
    {
        if (!Screen.fullScreen)
        {
            Resolution newResolution = Screen.resolutions[resolutionIndex];
            Screen.SetResolution(newResolution.width, newResolution.height, true);
        }
        else
        {
            Resolution newResolution = Screen.resolutions[resolutionIndex];
            Screen.SetResolution(newResolution.width, newResolution.height, false);
        }
    }
    public void OnVSYNC()
    {
        if (QualitySettings.vSyncCount != 0)
            QualitySettings.vSyncCount = 0;
        else
            QualitySettings.vSyncCount = 1;
    }
    void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("FullScreen", Screen.fullScreen ? 1 : 0);
    }

    //Handle monitor switching
    public void OnMonitor()
    {
        int newMonitorIndex = NextMonitor();
        //monitorText.text = "Display " + (newMonitorIndex + 1).ToString();
    }
    private int NextMonitor()
    {
        int currentIndex = PlayerPrefs.GetInt("UnitySelectMonitor");
        currentIndex++;
        if (currentIndex >= Display.displays.Length)
            currentIndex = 0;

        StartCoroutine(SetMonitor(currentIndex));
        return currentIndex;
    }
    private IEnumerator SetMonitor(int index)
    {
        PlayerPrefs.SetInt("UnitySelectMonitor", index);

        yield return null;
        yield return null;

        bool goToFullScreen = Screen.fullScreen;
        Screen.SetResolution(500, 500, false);

        yield return null;
        yield return null;

        Screen.SetResolution(Screen.resolutions[resolutionIndex].width, Screen.resolutions[resolutionIndex].height, true);
    }

    int resolutionIndex = 0;
    public bool changeResolution = false;
    private void HandleResolutionSlider()
    {
        float value = resolutionSlider.value;
        float nrResolutions = Screen.resolutions.Length;
        
        float stepSize = 1f / nrResolutions;

        resolutionIndex = (int)Mathf.Clamp((value / stepSize) - 1, 0f, nrResolutions);


        //if (!resolutionSlider.selected && !resolutionSlider.pressed && Screen.currentResolution.width != Screen.resolutions[resolutionIndex].width && Screen.currentResolution.height != Screen.resolutions[resolutionIndex].height)
        //{
        //    Resolution newResolution = Screen.resolutions[resolutionIndex];
        //    Screen.SetResolution(newResolution.width, newResolution.height, Screen.fullScreen);
        //}
        if(GameManager.GetPlayer(-1).GetButtonDown("Button A"))
        {
            if (changeResolution)
            {
                changeResolution = false;

                Resolution newResolution = Screen.resolutions[resolutionIndex];
                Screen.SetResolution(newResolution.width, newResolution.height, Screen.fullScreen);
            }
            if (resolutionSlider.pressed)
            {
                changeResolution = true;
            }
        }
    }
}
