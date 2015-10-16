using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class M_ButtonPress : M_EventBase
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
                M_Button targetButton = eventList[i].targetButton;
                M_Screen screen = transform.GetComponentInParent<M_Screen>();
                if (targetButton == null)
                    break;

                screen.PressButton(targetButton);
            }
        }
    }
}
