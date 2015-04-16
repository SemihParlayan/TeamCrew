﻿using UnityEngine;
using System.Collections;

public class FrogPrototype : MonoBehaviour
{
    public ParticleSystem rightParticle;
    public ParticleSystem leftParticle;

    public float speed;

    //For fly power up
    public float speedBoost;
    public float BoostDuration;
    float boostTimer;


    public string player;

    public HandGrip leftGripScript;
    public HandGrip rightGripScript;

    public HingeJoint2D leftJoint;
    public HingeJoint2D rightJoint;

    public Transform leftHandOrigin;
    public Transform leftHandNeutral;
    public Transform leftHand;
    private Rigidbody2D leftBody;

    public Transform rightHandOrigin;
    public Transform rightHandNeutral;
    public Transform rightHand;
    private Rigidbody2D rightBody;

    private Rigidbody2D body;

    public GripMagnet leftHandMagnet;
    public GripMagnet rightHandMagnet;

    void Start()
    {
        leftBody = leftHand.GetComponent<Rigidbody2D>();
        rightBody = rightHand.GetComponent<Rigidbody2D>();
        body = GetComponent<Rigidbody2D>();

        body.isKinematic = true;
    }

    void Update()
    {
        //TEMPORARY RESTART
        if (Input.GetButtonDown("Start"))
        {
            Application.LoadLevel(Application.loadedLevel);
        }

        //Keeps body still until a grip is made
        ActivateBody();

        //Activate scratch
        rightParticle.enableEmission = false;
        leftParticle.enableEmission = false;
        ControlScratch();

        //Control Hands
        ControlHand(leftGripScript, player + "HL", player + "VL", leftJoint, 1, leftBody, leftHandMagnet, leftHand, leftHandNeutral, leftHandOrigin);
        ControlHand(rightGripScript, player + "HR", player +"VR", rightJoint, -1, rightBody, rightHandMagnet, rightHand, rightHandNeutral, rightHandOrigin);
    }

    void ControlScratch()
    {
        if (leftGripScript.isOnGrip || rightGripScript.isOnGrip)
            return;

        if (body.velocity.y > -1)
            return;

        float vertical = Input.GetAxis(player + "VL");
        if (leftGripScript.isOnWall && vertical > 0)
        {
            leftParticle.enableEmission = true;
        }

        vertical = Input.GetAxis(player + "VR");
        if (rightGripScript.isOnWall && vertical > 0) 
        {
            rightParticle.enableEmission = true;
        }
    }
    void ControlHand(HandGrip handScript, string horizontalAxis, string verticalAxis, HingeJoint2D joint, int motorDir, Rigidbody2D body, GripMagnet magnet, Transform hand, Transform handNeutral, Transform handOrigin)
    {
        bool grip = joint.useMotor = body.isKinematic = handScript.isOnGrip;

        Vector3 input = new Vector3(Input.GetAxis(horizontalAxis), Input.GetAxis(verticalAxis));
        float angle = Mathf.Rad2Deg * (float)Mathf.Atan2(input.x, input.y);
        if (angle < 0)
        {
            angle = 180 + (180 - Mathf.Abs(angle));
        }
        float i = (int)(angle / 45.0f);
        angle = (45 * i) * Mathf.Deg2Rad;

        //Left hand movement
        //if (grip && input.y < 0)
        //{
        //    JointMotor2D motor = new JointMotor2D();
        //    motor.motorSpeed = (input.sqrMagnitude - 0.5f) * 500 * motorDir;
        //    motor.maxMotorTorque = 1000;

        //    joint.motor = motor;
        //}
        joint.useMotor = (grip && input.y < 0);

        //Hand grip
        if (grip) //If hand is on a grip
        {
            //Move towards grip point
            Vector3 targetPosition = handScript.GripPosition;
            body.velocity = (targetPosition - hand.position) * speed;
        }
        else if ((input.x != 0 || input.y != 0)) //If hand is moving and not on a grip
        {
            //Move towards joystick Direction
            Vector3 dir = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle));
            Vector3 targetPosition = handOrigin.position + dir * 2.0f + magnet.magnetDir;
            body.velocity = (targetPosition - hand.position) * speed;
        }
        else //If hand is not moving and not on grip
        {
            //Move towards neutral position
            Vector3 targetPosition = handNeutral.position;
            body.velocity = (targetPosition - hand.position) * speed;
        }
    }
    void ActivateBody()
    {
        if (body.isKinematic)
        {
            if (leftGripScript.isOnGrip || rightGripScript.isOnGrip)
            {
                body.isKinematic = false;
            }
        }
    }

    public void EnergyBoost()
    {
    }
}

