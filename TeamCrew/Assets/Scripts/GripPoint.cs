using UnityEngine;
using System.Collections;

public class GripPoint : MonoBehaviour 
{
    public string holderName;
    public int numberOfHands;

    public bool Busy 
    { 
        get
        {
            return holderName.Length > 0;
        }
    }
}
