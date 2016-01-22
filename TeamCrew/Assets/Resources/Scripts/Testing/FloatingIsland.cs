using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class FloatingIsland : MonoBehaviour 
{
    public float frequency = 0.5f;
    public float magnitude = 1f;
    public float time = 0.0f;

    private Vector3 pos;
    private Rigidbody2D body;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        body.mass = int.MaxValue;
        body.isKinematic = true;

        pos = transform.position;
    }

    void Update()
    {
        time += Time.deltaTime;
        Vector3 newPos = pos + Vector3.up * Mathf.Sin(time * frequency) * magnitude;

        body.MovePosition(newPos);
    }
}
