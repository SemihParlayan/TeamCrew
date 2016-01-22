using UnityEngine;
using System.Collections;

public class SpinningWheelAngle : SpinningWheel
{
    //Publics
    [Range(-360f, 360f)]
    public int redAngle;
    [Range(-360f, 360f)]
    public float greenAngle;
    [Range(0.1f, 30f)]
    [Tooltip("How fast does the wheel change direction")]
    public float switchVelocity = 1f;

    //privates
    private Vector2 dirOne;
    private Vector2 dirTwo;
    private Vector2 targetDir;
    private int dirModifier;
    private bool startDirectionFound;
    private bool decreaseSpeed;
    private float angleLimit;
    private float startSpeed;

    protected override void BaseStart()
    {
        base.BaseStart();

        dirModifier = 1;
        angleLimit = 3f;
        startSpeed = spinSpeed;

        if (greenAngle < 0)
            greenAngle = 360 + greenAngle;
        if (redAngle < 0)
            redAngle = 360 + redAngle;

        targetDir = Vector2.zero;
    }
    protected override void BaseUpdate()
    {
        base.BaseUpdate();
        dirOne = new Vector2(Mathf.Cos(Mathf.Deg2Rad * (-redAngle + 90)), Mathf.Sin(Mathf.Deg2Rad * (-redAngle + 90)));
        dirTwo = new Vector2(Mathf.Cos(Mathf.Deg2Rad * (-greenAngle + 90)), Mathf.Sin(Mathf.Deg2Rad * (-greenAngle + 90)));

        FindDirection();
        CheckWhenToRotate();
        ManipulateSpeed();
    }
    protected override void BaseFixedUpdate()
    {
        int dir = (spinRight) ? -1 : 1;
        dir *= dirModifier;
        body.angularVelocity = spinSpeed * dir;
    }

    private void ManipulateSpeed()
    {
        if (decreaseSpeed)
        {
            if (spinSpeed > 0)
            {
                spinSpeed -= switchVelocity;
            }
            else
            {
                decreaseSpeed = false;
                spinSpeed = 0;
                SwitchRotateDirection();
            }
        }
        else
        {
            if (spinSpeed < startSpeed)
            {
                spinSpeed += switchVelocity;
            }
            else if (spinSpeed != startSpeed)
            {
                spinSpeed = startSpeed;
            }
        }
    }
    private void FindDirection()
    {
        if (!startDirectionFound)
        {
            float angleToOne = Vector2.Angle(transform.up, dirOne);
            float angleToTwo = Vector2.Angle(transform.up, dirTwo);

            if (angleToTwo <= angleLimit)
            {
                targetDir = dirTwo;
                startDirectionFound = true;
                decreaseSpeed = true;
            }
            else if (angleToOne <= angleLimit)
            {
                targetDir = dirOne;
                startDirectionFound = true;
                decreaseSpeed = true;
            }
        }
    }
    private void CheckWhenToRotate()
    {
        if (startDirectionFound)
        {
            float diffAngle = Vector2.Angle(transform.up, targetDir);
            if (diffAngle <= angleLimit)
            {
                decreaseSpeed = true;
            }
        }
    }
    private void SwitchRotateDirection()
    {
        dirModifier *= -1;
        targetDir = (targetDir == dirOne) ? dirTwo : dirOne;
    }
    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            dirOne = new Vector2(Mathf.Cos(Mathf.Deg2Rad * (-redAngle + 90)), Mathf.Sin(Mathf.Deg2Rad * (-redAngle + 90)));
            dirTwo = new Vector2(Mathf.Cos(Mathf.Deg2Rad * (-greenAngle + 90)), Mathf.Sin(Mathf.Deg2Rad * (-greenAngle + 90)));
        }

        float rayLength = 4f;
        float radius = 0.15f;

        //Draw current direction
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.up * (rayLength / 1.5f));
        Gizmos.DrawSphere(transform.position + transform.up * (rayLength / 1.5f), radius);

        //Draw maxLimit
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, dirOne * rayLength);
        Gizmos.DrawSphere(transform.position + new Vector3(dirOne.x, dirOne.y) * rayLength, radius);

        //Draw minLimit
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, dirTwo * rayLength);
        Gizmos.DrawSphere(transform.position + new Vector3(dirTwo.x, dirTwo.y) * rayLength, radius);
    }
}
