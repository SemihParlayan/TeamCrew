using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class M_ScreenManager : MonoBehaviour
{
    //Data
    public M_Screen startScreen;

    //References
    static private M_Screen currentScreen;
    static private M_Screen previousScreen;
    private Camera cam;

    //Components

    void Start()
    {
        if (startScreen == null)
        {
            Debug.LogError("Assign a starting screen to " + transform.name);
        }
        cam = Camera.main;

        DisableScreenScripts();
        SwitchScreen(startScreen);
        
    }

    void Update()
    {
        if (currentScreen == null)
            return;

        if (currentScreen.movementProperties.cameraLocation != null)
        {
            //Move camera position
            Vector3 newPos = Vector3.Lerp(cam.transform.position, currentScreen.movementProperties.cameraLocation.position, Time.deltaTime * currentScreen.movementProperties.movementSpeed);
            newPos.z = -1;
            cam.transform.position = newPos;
        }

        //Zoom camera
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, currentScreen.movementProperties.zoom, currentScreen.movementProperties.zoomSpeed * Time.deltaTime);
    }
    public void DisableScreenScripts()
    {
        M_Screen[] screens = transform.GetComponentsInChildren<M_Screen>();

        for (int i = 0; i < screens.Length; i++)
        {
            screens[i].enabled = false;
            screens[i].active = false;
        }
    }
    public static void SetActive(bool state)
    {
        GameObject.FindWithTag("MenuManager").GetComponent<M_ScreenManager>().enabled = state;
    }
    public static void SwitchScreen(M_Screen newScreen)
    {
        if (currentScreen == newScreen)
            return;

        previousScreen = currentScreen;
        currentScreen = newScreen;

        if (previousScreen != null)
        {
            previousScreen.OnSwitchedFrom();
            previousScreen.enabled = false;
        }

        currentScreen.enabled = true;
        currentScreen.OnSwitchedTo();
    }

    public static M_Screen GetCurrentScreen()
    {
        return currentScreen;
    }
    public static void TeleportToCurrentScreen()
    {
        if (!currentScreen)
            return;

        Vector3 targetLocation = currentScreen.movementProperties.cameraLocation.position;
        targetLocation.z = Camera.main.transform.position.z;
        Camera.main.transform.position = targetLocation;
    }
}