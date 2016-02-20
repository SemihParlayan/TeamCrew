using UnityEngine;
using System.Collections;

public class VineBuilder_Object : MonoBehaviour 
{
    public Transform bottomPoint;

    void OnDrawGizmos()
    {
        if (bottomPoint)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(bottomPoint.position, 0.01f);
        }
    }
}
