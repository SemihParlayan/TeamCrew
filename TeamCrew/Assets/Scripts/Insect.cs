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
    public MotionState motionState;

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
    private int grabbed = 0;
    int direction = -1;

    public Transform bottomFrog;

    public float playerDifference;
    public float chaseForce;

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

        chaseForce = 500f;
        ChangeState(MotionState.chasing);
	}
	
	
	void Update ()
    {
        if (GameManager.playerOne && GameManager.playerTwo)
        {
            bottomFrog = (GameManager.playerOne.position.y < GameManager.playerTwo.position.y) ? GameManager.playerOne : GameManager.playerTwo;

            playerDifference = Mathf.Abs(GameManager.playerOne.position.y - GameManager.playerTwo.position.y);
        }
        else
        {
            playerDifference = -1;
        }

        if (bottomFrog == null)
        {
            if (motionState == MotionState.chasing)
                ChangeState(MotionState.normal);
        }
        else
        {

            startPos.y = Mathf.MoveTowards(startPos.y, bottomFrog.position.y + playerOffsetY, Time.deltaTime);
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

                if (chaseForce > 150)
                {
                    chaseForce -= 1f;
                }
                body.AddForce(new Vector2(direction * chaseForce, 0));

                //Clamp velocity
                Vector2 vel = body.velocity;
                vel.x = Mathf.Clamp(vel.x, -6, 6);
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


                if (playerDifference != -1)
                {
                    if (playerDifference < 3)
                    {
                        ChangeState(MotionState.normal);
                    }
                }
                
            }
            break;

            case MotionState.panicMode:
            {
                Transform topFrog = null;
                if (GameManager.playerOne && GameManager.playerTwo)
                {
                    topFrog = (GameManager.playerOne.position.y > GameManager.playerTwo.position.y) ? GameManager.playerOne : GameManager.playerTwo;
                }

                if (topFrog != null)
                {
                    //Follow targetFrog in X
                    if (bottomFrog.position.x < topFrog.position.x)
                        direction = 1;
                    else
                        direction = -1;

                    body.AddForce(new Vector2(direction * 300, 0));

                    //Clamp velocity
                    Vector2 vel = body.velocity;
                    vel.x = Mathf.Clamp(vel.x, -6, 6);
                    body.velocity = vel;
                }

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

        motionState = state;
        //getting new behaviour
        switch (motionState)
        {
            case MotionState.panicMode:
            {
                targetY = transform.position.y + panicModeTop;
            }
            break;
            case MotionState.chasing:
            {
                chaseForce = 500f;
            }
            break;
            case MotionState.normal:
            {
                Destroy(GetComponent<HingeJoint2D>());
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
        if (grabbed <= 0)
        {
            ChangeState(MotionState.normal);
        }
    }
}