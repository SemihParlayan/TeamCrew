using UnityEngine;
using System.Collections;

public class FloatingIsland : MonoBehaviour 
{
    public float frequency = 20.0f;
    public float magnitude = 0.5f;
    public float time = 0;

    private Vector3 pos;
    private Rigidbody2D body;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        pos = transform.position;
    }

    void Update()
    {
        time += Time.deltaTime;
        Vector3 newPos = pos + Vector3.up * Mathf.Sin(time * frequency) * magnitude;

        body.MovePosition(newPos);
    }
}
