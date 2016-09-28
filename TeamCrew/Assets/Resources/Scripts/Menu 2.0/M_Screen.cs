using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rewired;

public enum XboxEvent 
{ 
    OnStickLeft,
    OnStickRight,
    OnStickUp, 
    OnStickDown,
    OnPress,
    OnReturn,
    OnSelect,
    OnDeSelect
}

[System.Serializable]
public struct ScreenMovementProperties
{
    public Transform cameraLocation;
    public float zoom;
    public float zoomSpeed;
    public float movementSpeed;
}
public class M_Screen : MonoBehaviour 
{
    //Inspector
    public bool active;
    public bool subScreen;
    public int player = 1;
    public M_Button entryButton;
    public ScreenMovementProperties movementProperties;

    //References
    private M_Button currentButton;

    //Data
    private bool CanSelect
    {
        get
        {
            return canSelect;
        }
        set
        {
            canSelect = value;
        }
    }
    private bool canSelect;
    
    //Static
    public static float selectionDelay = 0.2f;
    public static float pressDelay = 0.2f;

    void Awake()
    {
        OnAwake();
    }
    void Start()
    {
        CanSelect = true;
        HighlightDefaultButton();

        OnStart();
    }
    protected virtual void OnAwake()
    {

    }
    protected virtual void OnStart()
    {

    }
    void Update()
    {
        OnUpdate();
    }

    protected virtual void OnUpdate()
    {
        if (!active)
            return;

        //Stick input
        Vector2 input = Vector2.zero;
        input.x = GameManager.GetPlayer(player).GetAxis("LeftStick Horizontal");
        input.y = GameManager.GetPlayer(player).GetAxis("LeftStick Vertical");


        if (CanSelect)
        {
            if (input.x < -0.9f)
            {
                CanSelect = !SendEventToCurrentButton(XboxEvent.OnStickLeft);
            }
            else if (input.x > 0.9f)
            {
                CanSelect = !SendEventToCurrentButton(XboxEvent.OnStickRight);
            }
            else if (input.y > 0.9f)
            {
                CanSelect = !SendEventToCurrentButton(XboxEvent.OnStickUp);
            }
            else if (input.y < -0.9f)
            {
                CanSelect = !SendEventToCurrentButton(XboxEvent.OnStickDown);
            }
        }
        else
        {
            if ((input.x > -0.1f && input.x < 0.1f) && (input.y > -0.1f && input.y < 0.1f))
                CanSelect = true;
            else if ((input.y > -0.1f && input.y < 0.1f) && (input.x > -0.1f && input.x < 0.1f))
                CanSelect = true;
        }

        //Button select input
        bool selectPress = false;
        bool returnPress = false;

        if (!subScreen)
        {
            selectPress = GameManager.defaultPlayer.GetButtonDown("Button A");
            returnPress = GameManager.defaultPlayer.GetButtonDown("Button B");

        }
        else
        {
            selectPress = GameManager.GetPlayer(player).GetButtonDown("Button A");
            returnPress = GameManager.GetPlayer(player).GetButtonDown("Button B");
        }

        if (selectPress)
        {
            Press();
        }
        else if (returnPress)
        {
            Return();
        }
    }

    //Events
    public virtual void OnSwitchedTo()
    {
        Invoke("Activate", 0.2f);

        M_Screen[] subScreens = transform.GetComponentsInChildren<M_Screen>();

        for (int i = 1; i < subScreens.Length; i++)
        {
            if (subScreens[i].subScreen)
            {
                subScreens[i].enabled = true;
                subScreens[i].OnSwitchedTo();
            }
        }
    }
    public virtual void OnSwitchedFrom()
    {
        active = false;

        M_Screen[] subScreens = transform.GetComponentsInChildren<M_Screen>();
        for (int i = 1; i < subScreens.Length; i++)
        {
            if (subScreens[i].subScreen)
            {
                subScreens[i].OnSwitchedFrom();
                subScreens[i].enabled = false;
            }
        }
    }

    //Methods
    bool SendEventToCurrentButton(XboxEvent e)
    {
        bool canSend = (currentButton != null && !currentButton.Disabled);

        if (canSend)
            currentButton.SendMessage(e.ToString(), SendMessageOptions.DontRequireReceiver);

        return canSend;
    }
    bool SendEventToObject(XboxEvent e, GameObject o)
    {
        bool canSend = (o != null);

        if (canSend)
            o.SendMessage(e.ToString(), SendMessageOptions.DontRequireReceiver);

        return canSend;
    }

    IEnumerator ResetCanSelect()
    {
        yield return new WaitForSeconds(selectionDelay);

        CanSelect = true;
    }
    void HighlightDefaultButton()
    {
        if (entryButton == null)
            return;

        //Select
        if (currentButton)
            SendEventToCurrentButton(XboxEvent.OnDeSelect);
        currentButton = entryButton;
        SendEventToCurrentButton(XboxEvent.OnSelect);
    }
    bool Press()
    {
        if (currentButton == null)
            return false;
        return SendEventToCurrentButton(XboxEvent.OnPress);
    }
    bool Return()
    {
        if (currentButton == null)
            return false;
        return SendEventToCurrentButton(XboxEvent.OnReturn);
    }
    void Activate()
    {
        active = true;
        HighlightDefaultButton();
    }

    public void SwitchButton(M_Button targetButton)
    {
        if (SendEventToCurrentButton(XboxEvent.OnDeSelect))
        {
            currentButton = targetButton;
            SendEventToCurrentButton(XboxEvent.OnSelect);
        }
    }
    public void SwitchScreen(M_Screen targetScreen)
    {
        SendEventToCurrentButton(XboxEvent.OnDeSelect);
        M_ScreenManager.SwitchScreen(targetScreen);
    }
    public void RefreshScreen()
    {
        if (M_ScreenManager.GetCurrentScreen() == this)
        {
            OnSwitchedFrom();
            OnSwitchedTo();
        }
    }
    public void PressButton(M_Button targetButton)
    {
        if (targetButton == null)
            return;

        SendEventToObject(XboxEvent.OnPress, targetButton.gameObject);
        StopCoroutine("UnPressButton");
        StartCoroutine(UnPressButton(targetButton));
    }
    IEnumerator UnPressButton(M_Button button)
    {
        yield return new WaitForSeconds(M_Button.pressDelay);
        SendEventToObject(XboxEvent.OnDeSelect, button.gameObject);
    }
}
