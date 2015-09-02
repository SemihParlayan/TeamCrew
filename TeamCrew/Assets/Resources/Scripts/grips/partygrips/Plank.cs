using UnityEngine;
using System.Collections;

public class Plank : MonoBehaviour 
{
	void Start () 
	{
        GetComponent<HingeJoint2D>().connectedAnchor = transform.localPosition - new Vector3(0, 0.25f);
	}
}
