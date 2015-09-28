using UnityEngine;
using System.Collections;

public class InactivityFrog : MonoBehaviour 
{
    //Data
    public string frog;
    public float inactivityTimer;
    public float inactivityLimit;

    public bool IsInactive { get { return inactivityTimer >= inactivityLimit; } }

    //Components

    //References
}
