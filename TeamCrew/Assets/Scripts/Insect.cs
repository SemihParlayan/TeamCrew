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
    public float offsetFromFrog = 5;
    public float panicModeTop = 1;

    public float goSlowlyUpForce;
    public float goSlowlyDownForce;
    public float liftPlayerForce;
    
    Rigidbody2D body;
    Transform hand;

    float sittingTime; //timer

    float thing; // explain what this is

    Vector2 startPos;
    float targetY;

    bool grabbed;
	void Start ()
    {
        body = GetComponent<Rigidbody2D>();

        //POSITION
            startPos = new Vector2(0,0);

            //Lowest frog
            Transform lowFrog = (GameManager.playerOne.position.y < GameManager.playerTwo.position.y) ?
                GameManager.playerOne : GameManager.playerTwo;

            startPos = lowFrog.position;
            
            //Spawn to the right?
            if (Random.Range(0.0f, 1.0f) > .5f)
            {
                startPos.x += offsetFromFrog;
            }
            else
            {
                startPos.x -= offsetFromFrog;
                //flip sprite
                transform.localScale = new Vector3(-1, 1, 1);
            }

            transform.position = startPos;

            targetY = startPos.y + panicModeTop;

        //Forces
        goSlowlyDownForce = 500;
        goSlowlyUpForce = 1000;
        liftPlayerForce = 1500;

        //Motionstate
        motionState = MotionState.panicMode;
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
            { 
                if (transform.position.y < targetY)
                {
                    float force = grabbed ? liftPlayerForce : goSlowlyUpForce;
                    body.AddForce(new Vector2(0, force));
                }
                else if (body.velocity.y < 0)
                    body.AddForce(new Vector2(0, goSlowlyDownForce));
            }   break;

            case MotionState.sit:
            {
                
            }   break;
            
            case MotionState.normal:
            {
                
            }   break;
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

    }
    public void SetHand(Transform hand)
    {
        this.hand = hand;
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