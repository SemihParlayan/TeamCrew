using UnityEngine;
using System.Collections;

public class OptionScreen : M_Screen
{
    public SpriteRenderer fullscreenCheckbox;
    public TextMesh monitorText;

    protected override void OnAwake()
    {
        base.OnAwake();
        bool fullScreen = PlayerPrefs.GetInt("FullScreen", 1) == 1 ? true : false;
        QualitySettings.SetQualityLevel(5);
    }
    protected override void OnStart()
    {
        base.OnStart();
        fullscreenCheckbox.enabled = Screen.fullScreen;
    }
    public void OnFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
    void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("FullScreen", Screen.fullScreen ? 1 : 0);
    }

    //Handle monitor switching
    public void OnMonitor()
    {
        int newMonitorIndex = NextMonitor();
        monitorText.text = "Display " + (newMonitorIndex + 1).ToString();
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

        Screen.SetResolution(Display.displays[index].systemWidth, Display.displays[index].systemHeight, true);
    }
}
