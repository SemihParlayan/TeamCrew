using UnityEngine;
using System.Collections;

public class VersusGrip : MovingGrip 
{
    public int parentedPlayer;

	void Start () 
    {
        Transform i = transform;

        while (i.FindChild("body") == null)
        {
            i = i.parent;
        }

        parentedPlayer = i.FindChild("body").GetComponent<FrogPrototype>().player;
	}

	void Update () 
    {
	
	}
}
