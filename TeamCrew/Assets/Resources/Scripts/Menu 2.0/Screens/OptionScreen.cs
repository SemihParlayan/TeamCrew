using UnityEngine;
using System.Collections;

public class OptionScreen : M_Screen
{
    void Start()
    {

    }
    public void OnFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}
