using UnityEngine;
using System.Collections;

public class M_EventBase : MonoBehaviour 
{
    public virtual void OnStickLeft()
    {
        OnEvent(Event.OnStickLeft);
    }
    public virtual void OnStickRight()
    {
        OnEvent(Event.OnStickRight);
    }
    public virtual void OnStickUp()
    {
        OnEvent(Event.OnStickUp);
    }
    public virtual void OnStickDown()
    {
        OnEvent(Event.OnStickDown);
    }
    public virtual void OnPress()
    {
        OnEvent(Event.OnPress);
    }
    public virtual void OnReturn()
    {
        OnEvent(Event.OnReturn);
    }
    public virtual void OnSelect()
    {
        OnEvent(Event.OnSelect);
    }
    public virtual void OnDeSelect()
    {
        OnEvent(Event.OnDeSelect);
    }

    public virtual void OnEvent(Event e)
    {

    }
}
