using UnityEngine;
using System.Collections;

public class FrogPrototype : MonoBehaviour
{
    public float speed;

    //For fly power up
    public float speedBoost;
    public float BoostDuration;
    float boostTimer;


    public string horizontalLeft;
    public string verticalLeft;

    public string horizontalRight;
    public string verticalRight;

    public Grip leftGripScript;
    public Grip rightGripScript;

    public HingeJoint2D leftJoint;
    public HingeJoint2D rightJoint;

    public Transform originLeft;
    public Transform originLeftHand;
    public Transform leftHand;
    private Rigidbody2D leftBody;

    public Transform originRight;
    public Transform originRightHand;
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
        if (Input.GetButtonDown("Start"))
        {
            Application.LoadLevel(Application.loadedLevel);
        }
        Vector3 input = new Vector3(Input.GetAxis(horizontalLeft), Input.GetAxis(verticalLeft));
        float angle = Mathf.Rad2Deg * (float)Mathf.Atan2(input.x, input.y);
        if (angle < 0)
        {
            angle = 180 + (180 - Mathf.Abs(angle));
        }

        float i = (int)(angle / 45.0f);
        angle = 45 * i;
        angle *= Mathf.Deg2Rad;
        bool leftGrip = leftJoint.useMotor = leftBody.isKinematic = leftGripScript.IsOnGrip();
        bool rightGrip = rightJoint.useMotor = rightBody.isKinematic = rightGripScript.IsOnGrip();


        if (leftGrip)
        {
            JointMotor2D motor = new JointMotor2D();
            motor.motorSpeed = (input.sqrMagnitude - 0.5f) * 500;
            motor.maxMotorTorque = 1000;

            leftJoint.motor = motor;
        }



        //Left Input
        if ((input.x != 0 || input.y != 0) && !leftGrip)
        {
            Vector3 dir = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle));
            Vector3 targetPosition = originLeft.position + dir * 1.5f + leftHandMagnet.magnetDir;
            leftBody.velocity = (targetPosition - leftHand.position) * speed;
        }
        else if (!leftGrip)
        {
            Vector3 targetPosition = originLeftHand.position;
            leftBody.velocity = (targetPosition - leftHand.position) * speed;
        }
        if (leftGrip)
        {
            Vector3 targetPosition = leftGripScript.GripPosition;
            leftBody.velocity = (targetPosition - leftHand.position) * speed;
        }

        input = new Vector3(Input.GetAxis(horizontalRight), Input.GetAxis(verticalRight));
        angle = Mathf.Rad2Deg * (float)Mathf.Atan2(input.x, input.y);
        if (angle < 0)
        {
            angle = 180 + (180 - Mathf.Abs(angle));
        }

        i = (int)(angle / 45.0f);
        angle = 45 * i;
        angle *= Mathf.Deg2Rad;

        if (rightGrip)
        {
            JointMotor2D motor = new JointMotor2D();
            motor.motorSpeed = (input.sqrMagnitude - 0.5f) * -500;
            motor.maxMotorTorque = 1000;

            rightJoint.motor = motor;
        }

        //Right Input
        if ((input.x != 0 || input.y != 0) && !rightGrip)
        {
            Vector3 dir = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle));
            Vector3 targetPosition = originRight.position + dir * 1.5f + rightHandMagnet.magnetDir;
            rightBody.velocity = (targetPosition - rightHand.position) * speed;
        }
        else if (!rightGrip)
        {
            Vector3 targetPosition = originRightHand.position;
            rightBody.velocity = (targetPosition - rightHand.position) * speed;
        }
        if (rightGrip)
        {
            Vector3 targetPosition = rightGripScript.GripPosition;
            rightBody.velocity = (targetPosition - rightHand.position) * speed;
        }

        if (body.isKinematic)
        {
            if (leftGrip || rightGrip)
            {
                body.isKinematic = false;
            }
        }
    }
    public void EnergyBoost()
    {
    }
}

