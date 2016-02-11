using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ScreenConnection
{
    public XboxEvent eventType;
    public M_Screen targetScreen;
}

public class M_ScreenSwitcher : M_EventBase
{
    //Data
    public List<ScreenConnection> eventList = new List<ScreenConnection>();

    //Events
    public override void OnEvent(XboxEvent e)
    {
        base.OnEvent(e);

        for (int i = 0; i < eventList.Count; i++)
        {
            if (eventList[i].eventType == e)
            {
                M_Screen targetScreen = eventList[i].targetScreen;
                M_Screen screen = transform.GetComponentInParent<M_Screen>();

                if (screen == null)
                    break;

                screen.SwitchScreen(targetScreen);
            }
        }
    }
}
