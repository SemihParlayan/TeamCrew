using UnityEngine;
using System.Collections;

public class BlobSpawner : MonoBehaviour
{
    //Data
    public float spawnRate;

    //References
    public Transform smallBlobPrefab;

	void Start () 
    {
        InvokeRepeating("Spawn", spawnRate, spawnRate);
	}

	void Update ()
    {
	}

    void Spawn()
    {
        Vector3 point = GetRandomPointInBox();
        RaycastHit2D hit = Physics2D.Raycast(point, Vector2.zero, 1f);
        if (hit.transform == transform)
        {
            Instantiate(smallBlobPrefab, point, Quaternion.identity);
        }
    }
    Vector3 GetRandomPointInBox()
    {
        Vector3 pos = Vector3.zero;
        BoxCollider2D c = GetComponent<BoxCollider2D>();

        pos.x = Random.Range(c.bounds.min.x, c.bounds.max.x);
        pos.y = Random.Range(c.bounds.min.y, c.bounds.max.y);

        return pos;
    }
}
