using UnityEngine;
using System.Collections;
public enum MotionState
{
    rip,
    chasing,
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
    public Vector2 relativeSpawnPosToFrog;
    public Vector2 relativeDeSpawnPosToFrog;
    public float panicModeTop;
    public float playerOffsetY = 3f;
    public float startLowestFly;
    float targetY;
    private Vector2 startPos;

    //Forces
    public float goSlowlyDownForce = 500;
    public float goSlowlyUpForce   = 1000;
    public float liftPlayerForce   = 1500;

    public AudioSource soundSource;
    Rigidbody2D body;
    Transform hand;
    int grabbed = 0;
    int direction = -1;

    public Transform bottomFrog;

	void Start ()
    {
        body = GetComponent<Rigidbody2D>();
        startPos = new Vector2();

        if (GameManager.playerOne && GameManager.playerTwo)
        {
            //Aquire bottomFrog
            bottomFrog = (GameManager.playerOne.position.y < GameManager.playerTwo.position.y) ? GameManager.playerOne : GameManager.playerTwo;

            startPos = bottomFrog.position;
            startPos.x = (GameManager.playerOne.position.x + GameManager.playerTwo.position.x) * .5f;
        }
            
        //Check side to spawn
        if (Random.Range(0.0f, 1.0f) > .5f)
        {
            //Spawn to the Right
            startPos.x += relativeSpawnPosToFrog.x;
            direction = -1;
            transform.Rotate(Vector3.up, Mathf.PI);
        }
        else
        {
            //Spawn to the left
            direction = 1;
            startPos.x -= relativeSpawnPosToFrog.x;
        }

        startPos.y += relativeSpawnPosToFrog.y;

        transform.position = startPos;
        targetY = startPos.y + panicModeTop;

        ChangeState(MotionState.chasing);
	}
	
	
	void Update ()
    {
        if (GameManager.playerOne && GameManager.playerTwo)
        {
            bottomFrog = (GameManager.playerOne.position.y < GameManager.playerTwo.position.y) ? GameManager.playerOne : GameManager.playerTwo;
        }
        if (bottomFrog == null)
        {
            ChangeState(MotionState.normal);
        }
        else
        {

            startPos.y = Mathf.MoveTowards(startPos.y, bottomFrog.position.y + playerOffsetY, Time.deltaTime);
        }

        //Return to normal state after sitting
        if (sittingTime > 0)
        {
            sittingTime -= Time.deltaTime;
            if (sittingTime <= 0)
            {
                sittingTime = 0;
                ChangeState(MotionState.normal);
            }
        }
	}
    void FixedUpdate()
    {
        switch(motionState)
        {
            case MotionState.normal:
            {
                //Remove insect if it is to far away from targetFrog
                if (Mathf.Abs(transform.position.x - startPos.x) > relativeDeSpawnPosToFrog.x)
                {
                    Destroy(gameObject);
                }

                //Move in X Direction;
                body.AddForce(new Vector2(150f * direction, 0));
                       

                if (transform.position.y < startPos.y)
                {
                    //Go upwards
                    body.AddForce(new Vector2(0, goSlowlyUpForce));
                }
                else if (body.velocity.y < 0)
                {
                    //Go upwards slowly
                    body.AddForce(new Vector2(0, goSlowlyDownForce));
                }
                    
            } 
            break;
              
            case MotionState.chasing:
            {
                if (bottomFrog == null)
                    break;

                //Follow targetFrog in X
                if (bottomFrog.position.x > transform.position.x)
                    direction = 1;
                else
                    direction = -1;

                body.AddForce(new Vector2(direction * 150f, 0));

                //Clamp velocity
                Vector2 vel = body.velocity;
                vel.x = Mathf.Clamp(vel.x, -4, 4);
                body.velocity = vel;

                if (transform.position.y < startPos.y)
                {
                    //Go upwards
                    body.AddForce(new Vector2(0, goSlowlyUpForce * 0.75f));
                }
                else if (body.velocity.y < 0)
                {
                    //Go upwards slowly
                    body.AddForce(new Vector2(0, goSlowlyDownForce));
                }
                
            }
            break;

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
            } 
            break;  

            case MotionState.rip:
            {
                if (bottomFrog != null)
                {
                    if (Mathf.Abs(transform.position.y - bottomFrog.position.y) > relativeDeSpawnPosToFrog.y)
                    {
                        Destroy(gameObject);
                    }
                }
                else
                {
                    if (transform.position.y < startPos.y - relativeDeSpawnPosToFrog.y)
                    {
                        Destroy(gameObject);
                    }
                }
            }
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
                return;

            case MotionState.sit:
                break;

            case MotionState.normal:
                break;
            
            case MotionState.panicMode:
                break;
        }

        motionState = state;
        //getting new behaviour
        switch (motionState)
        {
            case MotionState.panicMode:
            {
                if (bottomFrog == null)
                    break;

                targetY = bottomFrog.position.y + panicModeTop;
            }
            break;
        }
    }

    public void AddHand()
    {
        if(grabbed == 0) 
            ChangeState(MotionState.panicMode);

        grabbed++;
    }
    public void RemoveHand()
    {
        grabbed--;
        if (grabbed == 0)
        {
            ChangeState(MotionState.rip);
        }
    }
}