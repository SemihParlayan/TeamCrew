using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MethodConnection
{
    public Event eventType;
    public GameObject reciever;
    public string methodName;
}

public class M_MethodCaller : M_EventBase
{

    //Data
    public List<MethodConnection> eventList = new List<MethodConnection>();

    //Events
    public override void OnEvent(Event e)
    {
        base.OnEvent(e);

        for (int i = 0; i < eventList.Count; i++)
        {
            if (eventList[i].eventType == e)
            {
                GameObject reciever = eventList[i].reciever;

                if (reciever == null)
                    break;

                reciever.SendMessage(eventList[i].methodName);
            }
        }
    }
}
