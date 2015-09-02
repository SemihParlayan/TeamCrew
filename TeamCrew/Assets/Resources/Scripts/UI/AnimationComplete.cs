using UnityEngine;
using System.Collections;

public class AnimationComplete : MonoBehaviour
{
    public GameObject functionHolder;
    public string methodName;


    public void CallFunction()
    {
        functionHolder.SendMessage(methodName);
    }
}
