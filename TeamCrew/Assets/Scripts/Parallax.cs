using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour
{
    Vector2 startPosition;
    public float distance;
    private Camera cam;

	void Start ()
    {
        cam = Camera.main;
        startPosition = transform.position;
	}

	void Update ()
    {
        Vector2 camPos = cam.transform.position;
        transform.position = camPos * (1-(1 / distance)) + startPosition;
	}
}
