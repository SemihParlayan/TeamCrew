using UnityEngine;
using System.Collections;

public class SmallBlobMovement : MonoBehaviour 
{
    //Data
    public Vector2 forceInterval;
    public Vector2 moveInterval;
    public Vector2 startSizeInterval;

    [Range(1, 5)]
    public float slowDownSpeed;

    //Components
    private Rigidbody2D body;

	void Start () 
    {
        body = GetComponent<Rigidbody2D>();
        GetComponent<SpriteRenderer>().color = new Color(Random.Range(0f, 1f), Random.Range(0, 1f), Random.Range(0, 1f), 1f);
        InvokeRepeating("Flow", Random.Range(0f, 3f), Random.Range(moveInterval.x, moveInterval.y));
        float randomScale = Random.Range(startSizeInterval.x, startSizeInterval.y);
        transform.localScale = new Vector3(randomScale, randomScale);
	}
	
	void Update () 
    {

	}

    void FixedUpdate()
    {
        Vector2 vel = body.velocity;

        vel.x = Mathf.MoveTowards(vel.x, 0, Time.deltaTime * slowDownSpeed);
        vel.y = Mathf.MoveTowards(vel.y, 0, Time.deltaTime * slowDownSpeed);

        body.velocity = vel;
    }

    void Flow()
    {
        Vector2 randomDirection = Random.insideUnitCircle;
        float randomForce = Random.Range(forceInterval.x, forceInterval.y);

        body.AddForce(randomDirection * randomForce);
    }
}
