using UnityEngine;
using System.Collections;

public enum MotionState
{
    rip,
    sit,
    panicMode,
    normal
}

public class Insect : MonoBehaviour
{
    public MotionState motionState;
    public AudioSource soundSource;
    public float MinSittingTime;
    public float MaxSittingTime;
    public float speed;
    public float spawnPosX = 5;

    public float intenseForce;
    public float normalForce;

    Rigidbody2D body;
    Transform hand;

    float sittingTime; //timer

    float thing; // explain what this is

    float startY;
    float targetY;

	void Start ()
    {
        //flying forces
        normalForce  = 500;  // For going down slowly.
        intenseForce = 1500; // For lifting players.

        targetY = startY = transform.position.y;

        //movement
        targetY = 20;

        body = GetComponent<Rigidbody2D>();

        if (Random.Range(0.0f, 1.0f) >.5f)
        {
            spawnPosX = -spawnPosX;
            transform.localScale = new Vector3(-1,1,1);
        }
        Vector3 spawnPosition = transform.position;
        spawnPosition.x = spawnPosX;
        transform.position = spawnPosition;
	}
	
	
	void Update ()
    {
        if (sittingTime > 0)
        {
            sittingTime -= Time.deltaTime;
            if (sittingTime <= 0)
            {
                sittingTime = 0;
                // MotionState = MovementType.SIN;
            }
        }
        thing += Time.deltaTime;
	}

    void FixedUpdate()
    {
        switch(motionState)
        {
            case MotionState.panicMode:
                if (transform.position.y < targetY)
                {
                    body.AddForce(new Vector2(0, intenseForce));
                    //soundSource.pitch = intenseForce/normalForce;
                }
                else if (body.velocity.y < 0)
                {
                    body.AddForce(new Vector2(0, normalForce));
                    //soundSource.pitch = 1;

                }
            break;
            case MotionState.sit:

            break;
            case MotionState.normal:
            break;
        }
        
    }

    void ChangeState(MotionState state)
    {
        if(state == motionState) return;

        //ending old behaviour
        switch (motionState)
        {
            case MotionState.rip:
                break;

            case MotionState.sit:
                break;

            case MotionState.normal:
                break;
            
            case MotionState.panicMode:
                break;
        }
        //getting new behaviour
        switch (state)
        {
            case MotionState.rip:
                //turn of sound
            break;
                //turn off sound
            case MotionState.normal:

            break;

            case MotionState.panicMode:

            break;
        }
    }

    public void SetParalyze(bool state)
    {
        //paralyzed = state;
    }
    public void SetHand(Transform hand)
    {
        this.hand = hand;
       //transform.parent = hand;
       //transform.localPosition = Vector3.zero;
       //transform.localScale = Vector3.one;
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.transform.tag == "Grip" && Random.Range(0,2) == 1)
        {
            //movementMode = MovementType.NONE;
            sittingTime = Random.Range(MinSittingTime, MaxSittingTime);

            //Todo: Change sprite to sitting!
        }
    }
}