﻿using UnityEngine;
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
    public Vector2 relativeSpawnPosToFrog;
    public float panicModeTop;
    public float startLowestFly;
    float targetY;
    private Vector2 startPos;

    //Forces
    public float goSlowlyDownForce = 500;
    public float goSlowlyUpForce   = 1000;
    public float liftPlayerForce   = 1500;

    //Spped
    public float flyHorizontalSpeed = 4;

    private Animator animator;
    public AudioSource soundSource;
    Rigidbody2D body;
    Transform hand;
    int grabbed = 0;
    bool direction;

	void Start ()
    {
        body = GetComponent<Rigidbody2D>();
        startPos = new Vector2();

        //Check if the frogs are present
        if(GameManager.playerOne && GameManager.playerTwo)
        {
            Transform lowestFrog = (GameManager.playerOne.position.y < GameManager.playerTwo.position.y) ?
                GameManager.playerOne : GameManager.playerTwo;

            startPos = lowestFrog.position;
        }
        else/*if( frog is missing)*/
        { 
            startPos = transform.position;
        }
            
        //Spawn to the right?
        if (Random.Range(0.0f, 1.0f) > .5f)
        {
            startPos.x += relativeSpawnPosToFrog.x;
            direction = false;
            //transform.localScale = new Vector3(-1, 1, 1); //Sprite flip
            transform.RotateAround(new Vector3(0,1,0), Mathf.PI);
        }
        else
        {
            direction = true;
            startPos.x -= relativeSpawnPosToFrog.x;
            
        }

        startPos.y += relativeSpawnPosToFrog.y;

        transform.position = startPos;
        targetY = startPos.y + panicModeTop;

        animator = transform.GetComponent<Animator>();
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
                    if (GameManager.playerOne && GameManager.playerTwo)
                    {
                        Transform lowestFrog = (GameManager.playerOne.position.y < GameManager.playerTwo.position.y) ?
                           GameManager.playerOne : GameManager.playerTwo;

                        if (Mathf.Abs(transform.position.x - lowestFrog.position.x) > 75)
                        {
                            Destroy(gameObject);
                        }
                    }
                    
                        if(direction)
                        {
                             //body.velocity = new Vector2(flyHorizontalSpeed, body.velocity.y);
                            body.AddForce(new Vector2(100.0f,0));
                        }
                        else
                        {
                           // body.velocity = new Vector2(-flyHorizontalSpeed, body.velocity.y);
                            body.AddForce(new Vector2(-100.0f, 0));

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

        motionState = state;
        //getting new behaviour
        switch (motionState)
        {
            case MotionState.rip:
                //turn of sound
            break;
                //turn off sound
            case MotionState.normal:

            break;

            case MotionState.panicMode:
            {
                animator.SetTrigger("goNaked");
                body.fixedAngle = false;
            }
            
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
        Debug.Log("fly is colliding with something");
        if (c.transform.tag == "Grip" && Random.Range(0,2) == 1)
        {
            Debug.Log("fly decides to sit on grip");
            //movementMode = MovementType.NONE;
            sittingTime = Random.Range(MinSittingTime, MaxSittingTime);

            //Todo: Change sprite to sitting!
        }
    }

    public void AddHand()
    {
        if(grabbed == 0) ChangeState(MotionState.panicMode);
        grabbed++;
    }

    public void RemoveHand()
    {
        grabbed--;
        if (grabbed == 0) ChangeState(MotionState.rip);
    }
}