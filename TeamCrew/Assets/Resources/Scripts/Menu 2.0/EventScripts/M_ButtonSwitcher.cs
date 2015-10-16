using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ButtonConnection
{
    public Event eventType;
    public M_Button targetButton;
}

public class M_ButtonSwitcher : M_EventBase 
{

    //Data
    public List<ButtonConnection> eventList = new List<ButtonConnection>();

    //Events
    public override void OnEvent(Event e)
    {
        base.OnEvent(e);

        for (int i = 0; i < eventList.Count; i++)
        {
            if (eventList[i].eventType == e)
            {
                M_Button button = eventList[i].targetButton;
                M_Screen screen = transform.GetComponentInParent<M_Screen>();

                if (button == null)
                    break;

                screen.SwitchButton(button);
            }
        }
    }
}
