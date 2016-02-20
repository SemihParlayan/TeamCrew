using UnityEngine;
using System.Collections;

public class DestroyAfterTime : MonoBehaviour
{
    public float timeDelay;

    void Start()
    {
        Destroy(gameObject, timeDelay);
    }
}
