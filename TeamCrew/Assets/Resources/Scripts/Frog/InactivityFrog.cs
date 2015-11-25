using UnityEngine;
using System.Collections;

public class InactivityFrog : MonoBehaviour 
{
    //Data
    public string frog;
    public float timer;
    [HideInInspector]
    public float limit;

    public bool IsInactive { get { return timer >= limit; } }

    //Components

    //References
}
