using UnityEngine;
using System.Collections;
public enum MotionState
{
    Chasing,
    Panic,
    Exit
}

public class Insect : MonoBehaviour
{
    //Data
    public MotionState motionState;
    public Vector2 relativeSpawnPosToFrog;
    public float panicIncreaseInHeight;

    private Vector2 startPos;
    private int direction;
    public int activeFrogCounter;
    public float playerHeightDifference;
    private int grabbed;
    private float panicTargetY;

    public float exitingSpeed;
    public float chasingSpeed;
    public float panicSpeed;
    public float goSlowlyDownForce;
    public float goSlowlyUpForce;
    public float liftPlayerForce;
    public float currentForce;
    private bool hasSetDifference;

    public Transform topFrog;
    public Transform bottomFrog;

    //References
    public AudioSource soundSource;
    private Rigidbody2D body;

	void Start ()
    {
        //Aquire components
        body = GetComponent<Rigidbody2D>();

        //Check side to spawn
        direction = (Random.Range(0.0f, 1.0f) > 0.5f) ? 1 : -1;
        if (direction == -1)
            transform.Rotate(Vector3.up, 180f);

        //Set start position
        startPos = transform.position;
        startPos.x += relativeSpawnPosToFrog.x * -direction;
        startPos.y += relativeSpawnPosToFrog.y;
        transform.position = startPos;

        ChangeState(MotionState.Chasing);
	}
	
	
	void Update ()
    {
        topFrog = GameManager.GetTopFrog();
        bottomFrog = GameManager.GetBottomFrog();
        //Count how many active frogs we have
        activeFrogCounter = 0;
        for (int i = 0; i < GameManager.players.Length; i++)
        {
            if (GameManager.players[i] != null)
                activeFrogCounter++;
        }

        //Exit screen if there is not enough frogs
        if (activeFrogCounter <= 1)
        {
            ChangeState(MotionState.Exit);
        }

        if (activeFrogCounter > 1)
        {
            //Difference between top and bottom frog in Y position
            playerHeightDifference = Mathf.Abs(GameManager.GetTopFrog().position.y - GameManager.GetBottomFrog().position.y);
            hasSetDifference = true;
        }
	}
    void FixedUpdate()
    {
        switch(motionState)
        {
            case MotionState.Exit:
            {
                //Remove insect if it is to far away from targetFrog
                if (transform.position.x > 60 || transform.position.x < -60)
                {
                    Destroy(gameObject);
                }

                //Move in X Direction;
                body.AddForce(new Vector2(currentForce * direction, 0));
                currentForce++;
                       
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
              
            case MotionState.Chasing:
            {
                Transform bottomFrog = GameManager.GetBottomFrog();

                if (bottomFrog == null)
                    break;

                currentForce = chasingSpeed;
                //Follow targetFrog in X
                if (transform.position.x < bottomFrog.position.x)
                    direction = 1;
                else
                    direction = -1;

                body.AddForce(new Vector2(direction * currentForce, 0));

                //Clamp velocity
                Vector2 vel = body.velocity;
                vel.x = Mathf.Clamp(vel.x, -6, 6);
                body.velocity = vel;

                if (transform.position.y < bottomFrog.position.y + 4)
                {
                    //Go upwards
                    body.AddForce(new Vector2(0, goSlowlyUpForce * 0.6f));
                }
                else if (body.velocity.y < 0)
                {
                    //Go upwards slowly
                    body.AddForce(new Vector2(0, goSlowlyDownForce));
                }


                if (playerHeightDifference < 3 && hasSetDifference)
                {
                    ChangeState(MotionState.Exit);
                }
            }
            break;

            case MotionState.Panic:
            {
                Transform topFrog = GameManager.GetTopFrog();

                if (topFrog != null)
                {
                    //Follow targetFrog in X
                    if (transform.position.x < topFrog.position.x)
                        direction = 1;
                    else
                        direction = -1;

                    body.AddForce(new Vector2(direction * currentForce, 0));

                    //Clamp velocity
                    Vector2 vel = body.velocity;
                    vel.x = Mathf.Clamp(vel.x, -6, 6);
                    body.velocity = vel;
                }

                if (transform.position.y < panicTargetY)
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
        }
    }

    void ChangeState(MotionState state)
    {
        //Return if we are changing to the same state
        if(state == motionState) 
            return;

        //Set new state
        motionState = state;


        //React to the new state
        switch (motionState)
        {
            case MotionState.Panic:
            {
                currentForce = panicSpeed;
                panicTargetY = transform.position.y + panicIncreaseInHeight;
            }
            break;

            case MotionState.Exit:
            {
                currentForce = exitingSpeed;
                Destroy(GetComponent<HingeJoint2D>());
            }
            break;

            case MotionState.Chasing:
            {
                currentForce = chasingSpeed;
            }
            break;
        }
    }

    public void AddHand()
    {
        if(grabbed == 0) 
            ChangeState(MotionState.Panic);

        grabbed++;
    }
    public void RemoveHand()
    {
        grabbed--;
        if (grabbed <= 0)
        {
            ChangeState(MotionState.Exit);
        }
    }
}