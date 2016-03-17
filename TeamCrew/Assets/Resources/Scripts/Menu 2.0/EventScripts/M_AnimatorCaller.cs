using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AnimatorMethod { SetFloat, SetInteger, SetBool, SetTrigger}
[System.Serializable]
public class AnimatorConnection
{
    public XboxEvent eventType;
    public Animator animator;
    public AnimatorMethod method;
    public string parameterName;

    public float floatValue;
    public int integerValue;
    public bool boolValue;
}
public class M_AnimatorCaller : M_EventBase
{
    public List<AnimatorConnection> eventList = new List<AnimatorConnection>();

    public override void OnEvent(XboxEvent e)
    {
        base.OnEvent(e);

        for (int i = 0; i < eventList.Count; i++)
        {
            if (eventList[i].eventType == e)
            {
                Animator anim = eventList[i].animator;
                string param = eventList[i].parameterName;

                if (anim == null)
                    break;


                switch(eventList[i].method)
                {
                    case AnimatorMethod.SetInteger:
                        anim.SetInteger(param, eventList[i].integerValue);
                        break;
                    case AnimatorMethod.SetBool:
                        anim.SetBool(param, eventList[i].boolValue);
                        break;
                    case AnimatorMethod.SetFloat:
                        anim.SetFloat(param, eventList[i].floatValue);
                        break;
                    case AnimatorMethod.SetTrigger:
                        anim.SetTrigger(param);
                        break;
                }
            }
        }
    }
}
