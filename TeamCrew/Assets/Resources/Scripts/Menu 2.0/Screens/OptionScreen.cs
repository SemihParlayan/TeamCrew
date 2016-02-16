using UnityEngine;
using System.Collections;

public class OptionScreen : M_Screen
{

    public void OnFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}
