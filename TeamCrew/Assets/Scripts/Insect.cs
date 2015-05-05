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
    //State
    public MotionState motionState = MotionState.normal;

    //Timers
    public float MinSittingTime;
    public float MaxSittingTime;
    float sittingTime;

    //Positions
    public float offsetFromFrog;
    public float panicModeTop;
    float targetY;
    Vector2 startPos;

    //Forces
    public float goSlowlyDownForce = 500;
    public float goSlowlyUpForce   = 1000;
    public float liftPlayerForce   = 1500;

    //Spped
    public float flyHorizontalSpeed = 4;
    

    public AudioSource soundSource;
    Rigidbody2D body;
    Transform hand;
    int grabbed = 0;
    bool direction;

	void Start ()
    {
        body = GetComponent<Rigidbody2D>();
        startPos = new Vector2(0,0);

        //Check if there are any missing frogs
        if(!GameManager.playerOne || !GameManager.playerTwo)
        {
            startPos = transform.position;
        }
        else
        { 
            Transform lowestFrog = (GameManager.playerOne.position.y < GameManager.playerTwo.position.y) ?
                GameManager.playerOne : GameManager.playerTwo;

            startPos = lowestFrog.position;
        }
            
        //Spawn to the right?
        if (Random.Range(0.0f, 1.0f) > .5f)
        {
            startPos.x += offsetFromFrog;
            direction = false;
            transform.localScale = new Vector3(-1, 1, 1); //Sprite flip
        }
        else
        {
            direction = true;
            startPos.x -= offsetFromFrog;
            
        }

        transform.position = startPos + new Vector2(0,1);
        targetY = startPos.y + panicModeTop;
	}
	
	
	void Update ()
    {
        if (sittingTime > 0)
        {
            sittingTime -= Time.deltaTime;
            if (sittingTime <= 0)
            {
                sittingTime = 0;
                motionState = MotionState.normal;
            }
        }
	}

    void FixedUpdate()
    {
        switch(motionState)
        {
            case MotionState.normal:
                {
                    Transform lowestFrog = (GameManager.playerOne.position.y < GameManager.playerTwo.position.y) ?
                       GameManager.playerOne : GameManager.playerTwo;
                    if (Mathf.Abs(transform.position.x - lowestFrog.position.x) > 75)
                    {
                        Destroy(gameObject);
                    }
                    
                        if(direction)
                        {
                             body.velocity = new Vector2(flyHorizontalSpeed, body.velocity.y);
                        }
                        else
                        {
                            body.velocity = new Vector2(-flyHorizontalSpeed, body.velocity.y);
                        }
                       

                    if (transform.position.y < startPos.y)
                    {
                        body.AddForce(new Vector2(0, goSlowlyUpForce));
                    }
                    else if (body.velocity.y < 0)
                    {
                        body.AddForce(new Vector2(0, goSlowlyDownForce));
                    }
                    
                } break;
            case MotionState.panicMode:
            { 
                if (transform.position.y < targetY)
                {
                    float force = grabbed > 0 ? liftPlayerForce : goSlowlyUpForce;
                    body.AddForce(new Vector2(0, force));
                }
                else if (body.velocity.y < 0)
                {
                    body.AddForce(new Vector2(0, goSlowlyDownForce));
                }
            }   break;

            case MotionState.sit:
            {
                
            }   break;

            case MotionState.rip:
            {

                if (GameManager.playerOne && GameManager.playerTwo)
                {
                    Transform lowestFrog = (GameManager.playerOne.position.y < GameManager.playerTwo.position.y) ?
                        GameManager.playerOne : GameManager.playerTwo;

                    if (Mathf.Abs(transform.position.y - lowestFrog.position.y) > 100)
                    {
                        Destroy(gameObject);
                    }
                }

               
            } break;
            
            
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

    public void AddHand()
    {
        if(grabbed == 0) motionState = MotionState.panicMode;
        grabbed++;

    }

    public void RemoveHand()
    {
        grabbed--;
        if (grabbed == 0) motionState = MotionState.rip;
    }
}