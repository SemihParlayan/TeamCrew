using UnityEngine;
using System.Collections;

public class M_EventBase : MonoBehaviour 
{
    public virtual void OnStickLeft()
    {
        OnEvent(XboxEvent.OnStickLeft);
    }
    public virtual void OnStickRight()
    {
        OnEvent(XboxEvent.OnStickRight);
    }
    public virtual void OnStickUp()
    {
        OnEvent(XboxEvent.OnStickUp);
    }
    public virtual void OnStickDown()
    {
        OnEvent(XboxEvent.OnStickDown);
    }
    public virtual void OnPress()
    {
        OnEvent(XboxEvent.OnPress);
    }
    public virtual void OnReturn()
    {
        OnEvent(XboxEvent.OnReturn);
    }
    public virtual void OnSelect()
    {
        OnEvent(XboxEvent.OnSelect);
    }
    public virtual void OnDeSelect()
    {
        OnEvent(XboxEvent.OnDeSelect);
    }

    public virtual void OnEvent(XboxEvent e)
    {

    }
}
