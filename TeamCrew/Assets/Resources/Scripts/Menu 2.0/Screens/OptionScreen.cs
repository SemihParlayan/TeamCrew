using UnityEngine;
using System.Collections;

public class OptionScreen : M_Screen
{
    public SpriteRenderer fullscreenCheckbox;
    public TextMesh monitorText;
    public static int monitorIndex = 0;

    void Awake()
    {
        monitorIndex = PlayerPrefs.GetInt("Monitor", 0);
        ClampMonitorIndex(ref monitorIndex);

        Display.displays[monitorIndex].Activate();
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, Screen.fullScreen);
    }
    public void OnFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
    public void OnMonitor()
    {
        monitorIndex++;
        ClampMonitorIndex(ref monitorIndex);

        Display.displays[monitorIndex].Activate();
        monitorText.text = "Display " + (monitorIndex + 1).ToString();
    }

    private void ClampMonitorIndex(ref int index)
    {
        int minIndex = 0;
        int maxIndex = Display.displays.Length - 1;


        if (index < 0)
        {
            index = maxIndex;
        }
        else if (index > maxIndex)
        {
            index = minIndex;
        }
    }
}
