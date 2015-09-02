using UnityEngine;
using System.Collections;

public class SetLayerOrder : MonoBehaviour 
{
    public string layerName = "Default";
    public int sortingOrder = 0;

	void Awake () 
    {
        Renderer r = GetComponent<Renderer>();
        r.sortingLayerName = layerName;
        r.sortingOrder = sortingOrder;
	}
}
