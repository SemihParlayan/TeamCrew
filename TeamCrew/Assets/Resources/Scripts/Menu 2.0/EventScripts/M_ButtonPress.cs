using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class M_ButtonPress : M_EventBase
{
    //References
    private M_Screen screen;

    //Data
    public List<ButtonConnection> eventList = new List<ButtonConnection>();

    void Awake()
    {
        screen = GetComponentInParent<M_Screen>();
    }

    //Events
    public override void OnEvent(Event e)
    {
        base.OnEvent(e);

        for (int i = 0; i < eventList.Count; i++)
        {
            if (eventList[i].eventType == e)
            {
                M_Button targetButton = eventList[i].targetButton;
                if (targetButton == null)
                    break;

                screen.PressButton(targetButton);
            }
        }
    }
}
