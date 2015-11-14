using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class SpriteConnection
{
    public Event eventType;
    public SpriteRenderer spriteRenderer;
    public bool activate;
}

public class M_SpriteController : M_EventBase 
{
	//References

	//Data
    public List<SpriteConnection> eventList = new List<SpriteConnection>();

	//Components

	void Start () 
	{
        for (int i = 0; i < eventList.Count; i++)
        {
            if (!eventList[i].activate)
            {
                eventList[i].spriteRenderer.enabled = false;
            }
        }
	}

    //Events
    public override void OnEvent(Event e)
    {
        base.OnEvent(e);

        for (int i = 0; i < eventList.Count; i++)
        {
            if (eventList[i].eventType == e)
            {
                SpriteRenderer r = eventList[i].spriteRenderer;
                bool state = eventList[i].activate;

                if (r == null)
                    break;

                r.enabled = state;
            }
        }
    }
}
