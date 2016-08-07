using UnityEngine;
using System.Collections;

public class SetTextMeshRenderOrder : MonoBehaviour 
{
	//publics
    public string sortingLayerName;
    public int orderInLayer;
	//privates


	//Unity methods
	void Start () 
	{
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.sortingLayerName = sortingLayerName;
            renderer.sortingOrder = orderInLayer;
        }
	}
	void Update () 
	{
	
	}

	//public methods

	//private methods
}
