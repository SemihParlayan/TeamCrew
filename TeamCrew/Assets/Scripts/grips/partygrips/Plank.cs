using UnityEngine;
using System.Collections;

public class Plank : MonoBehaviour 
{
	void Start () 
	{
        GetComponent<HingeJoint2D>().connectedAnchor = transform.position;
	}
}
