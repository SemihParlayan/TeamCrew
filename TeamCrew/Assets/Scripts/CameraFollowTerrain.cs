using UnityEngine;
using System.Collections;

public class CameraFollowTerrain : MonoBehaviour 
{
    public LayerMask terrainMask;
    public float movementSpeed = 0.5f;
    private Camera cam;

	void Start ()
    {
        cam = Camera.main;
	}

	void Update () 
    {
        RaycastHit2D hit = Physics2D.BoxCast(new Vector2(transform.position.x - 50, transform.position.y), new Vector2(10, 10), 0, Vector2.right, 100, terrainMask);
        float targetX = transform.position.x;
        if (hit)
        {
            targetX = hit.transform.position.x;
        }

        Vector3 targetPos = cam.transform.position;
        targetPos.x = targetX;
        cam.transform.position = Vector3.Lerp(cam.transform.position, targetPos, Time.deltaTime * movementSpeed);
	}
}
