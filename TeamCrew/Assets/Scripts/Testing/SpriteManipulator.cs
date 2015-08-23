using UnityEngine;
using System.Collections;

public class SpriteManipulator : MonoBehaviour 
{
    public Transform anchor;
    public Vector2 offset;
    public string sortingLayerName;
    public int sortingOrder;

    void Start()
    {
        GetComponent<MeshRenderer>().sortingLayerName = sortingLayerName;
        GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
    }
	void Update () 
	{
        Mesh m = GetComponent<MeshFilter>().mesh;

        Vector3[] vertices = m.vertices;

        vertices[6] = transform.InverseTransformPoint(anchor.position + (transform.up.normalized * -offset.y) - (transform.right.normalized * -offset.x));
        vertices[10] = transform.InverseTransformPoint(anchor.position - (transform.up.normalized * -offset.y) - (transform.right.normalized * -offset.x));

        m.vertices = vertices;
	}
}
