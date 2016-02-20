using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class VineBuilder : MonoBehaviour 
{
    [HideInInspector]
    public bool searchingForNewLink = false;

    [HideInInspector]
    public BuildPart part = null;
    [HideInInspector]
    public BuildPart prevPart = null;

    [HideInInspector]
    public VineBuilder_Object closest = null;
    public List<VineBuilder_Object> vines = new List<VineBuilder_Object>();
    

    void OnDrawGizmos()
    {
        if (searchingForNewLink)
        {
            if (closest != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(closest.bottomPoint.position, 0.1f);
            }
        }
        else
        {
            if (prevPart.obj != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(prevPart.obj.bottomPoint.position, 0.1f);
            }
        }
    }
}
