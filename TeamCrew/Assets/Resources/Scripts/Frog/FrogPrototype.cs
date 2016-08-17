using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class FrogPrototype : MonoBehaviour
{
    public Transform topPrefab;
    public Transform characterSelectPrefab;
    public Color respawnArrowColor;
    private ParticleSystem rightParticle;
    private ParticleSystem leftParticle;

    public bool characterSelectFrog;
    public float speed;
    public float yVelocityClamp = 10;

    public bool forceArmsUp = false;

    public int player = -1;
    public Emotions emotionsScript;
    public HandGrip[] gripScript;

    public HingeJoint2D[] joints;

    public Transform[] handOrigin;
    public Transform[] handNeutral;
    public Transform[] hand;
    private Rigidbody2D[] handBody = new Rigidbody2D[2];
    public GripMagnet[] handMagnet;

    public GameObject tutorialSymbolLeft;
    public GameObject tutorialSymbolRight;
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

    [HideInInspector]
    public Rigidbody2D body;

    public GripMagnet leftHandMagnet;
    public GripMagnet rightHandMagnet;

    public AudioSource croakSound;

    [Header("Motor boosts")]
    public float motorSpeed = 350;
    public float versusMotorBoost = 350;
    public float oneArmedMotorBoost = 500;

    public int versusHands;
    private float armsCantGoDownTimer;

    [HideInInspector]
    public bool oneArmedModeEnabled;
    public bool IsGrippingTutorial { get { return (leftGripScript.isGrippingTutorial || rightGripScript.isGrippingTutorial);} }

    private GameManager gameManager;

    private RandomSoundFromList leftHandSoundChooser;
    private RandomSoundFromList rightHandSoundChooser;

    private bool leftScratchSounding;
    private bool rightScratchSounding;

    private VelocityVolume velVolLeft;
    private VelocityVolume velVolRight;

    void Start()
    {
        if (leftHandMagnet != null)
        {
            leftParticle = leftHandMagnet.GetComponent<ParticleSystem>();
        }
        if (rightHandMagnet != null)
        {
            rightParticle = rightHandMagnet.GetComponent<ParticleSystem>();
        }

        leftBody = handBody[0] = leftHand.GetComponent<Rigidbody2D>();
        if (leftBody == null) { Debug.Log("leftBody is null"); }


        rightBody = handBody[1] = rightHand.GetComponent<Rigidbody2D>();
        if (rightBody == null) { Debug.Log("rightBody is null"); }


        body = GetComponent<Rigidbody2D>();
        if (body == null) { Debug.Log("body is null"); }

        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        if (gameManager == null) { Debug.Log("GameManager is null"); Debug.Break(); }


        leftHandSoundChooser = leftGripScript.GetComponentInChildren<RandomSoundFromList>();

        velVolLeft = leftGripScript.GetComponentInChildren<VelocityVolume>();


        rightHandSoundChooser = rightGripScript.GetComponentInChildren<RandomSoundFromList>();

        velVolRight = rightGripScript.GetComponentInChildren<VelocityVolume>();

        forceArmsUp = false;

        croakSound = rightGripScript.transform.GetComponent<AudioSource>();

        yVelocityClamp = 10f;
    }
    private void Update()
    {
        if (GameManager.Hacks)
        {
            if (GameManager.GetCheatButton())
            {
                body.AddForce(new Vector2(0, 100000));
            }
        }

        if (croakSound != null && gameManager.gameActive)
        {
            bool croak = (GameManager.GetButtonPress(XboxButton.X, player) || GameManager.GetButtonPress(XboxButton.Y, player) || GameManager.GetButtonPress(XboxButton.B, player) || GameManager.GetButtonPress(XboxButton.A, player));
            if (croak)
            {
                croakSound.pitch = Random.Range(0.7f, 1.4f);
                croakSound.Play();
            }
        }
    }
    private void FixedUpdate()
    {

        //Keeps body still until a grip is made
        ActivateBody();

        //Activate scratch
        if (rightParticle != null && rightParticle != null)
        {
            rightParticle.enableEmission = false;
            leftParticle.enableEmission = false;
            ControlScratch();
        }

        ControlVersusGripController();

        //Control Hands
        if (!GameManager.UseMouseAsInput || player != 0)
        {
            ControlHand(leftGripScript, GameManager.GetThumbStick(XboxThumbStick.Left, player), leftJoint, 1, leftBody, leftHandMagnet, leftHand, leftHandNeutral, leftHandOrigin, rightGripScript);
            ControlHand(rightGripScript, GameManager.GetThumbStick(XboxThumbStick.Right, player), rightJoint, -1, rightBody, rightHandMagnet, rightHand, rightHandNeutral, rightHandOrigin, leftGripScript);
        }
        else
        {
            ControlHandMouse(leftGripScript, leftJoint, 1, leftBody, leftHandMagnet, leftHand, leftHandNeutral, leftHandOrigin, rightGripScript);
            ControlHandMouse(rightGripScript, rightJoint, -1, rightBody, rightHandMagnet, rightHand, rightHandNeutral, rightHandOrigin, leftGripScript);
        }

        //Limit y velocity for body
        Vector2 velocity = body.velocity;
        velocity.y = Mathf.Clamp(velocity.y, -int.MaxValue, yVelocityClamp);
        body.velocity = velocity;

        //scratch rotation
        if (leftHandMagnet != null && rightHandMagnet != null)
        {
            leftHandMagnet.transform.rotation = leftHandMagnet.transform.parent.rotation;
            rightHandMagnet.transform.rotation = leftHandMagnet.transform.parent.rotation;
        }
       

        //left Scratch sound
         if(leftParticle != null && leftParticle.enableEmission == true)
        {
            if (!leftScratchSounding)
            {
                leftHandSoundChooser.GenerateScratch();
                leftScratchSounding = true;
                velVolLeft.enabled = true;
            }

            leftHandSoundChooser.GetComponent<VelocityVolume>();


        }
        else
        {
            if(leftScratchSounding)
            {
                leftHandSoundChooser.Stop();
                leftScratchSounding = false;
                velVolLeft.enabled = false;
            }
        }

        //right scratch sound
        if (rightParticle != null && rightParticle.enableEmission == true)
        {
            if (!rightScratchSounding)
            {
                rightHandSoundChooser.GenerateScratch();
                rightScratchSounding = true;
                velVolRight.enabled = true;
            }
            
        }
        else
        {
            if (rightScratchSounding)
            {
                rightHandSoundChooser.Stop();
                rightScratchSounding = false;
                velVolRight.enabled = false;
            }
        }
    }


    public void ForceArmsUp()
    {
        Invoke("ArmsUp", 0.2f);
    }
    private void ArmsUp()
    {
        forceArmsUp = true;
    }


    public float maxVersusGripTime = 8.0f;
    public float versusGripTimer = 8.0f;
    void ControlVersusGripController()
    {
        if ((leftGripScript.isVersusGripping || rightGripScript.isVersusGripping) && (!leftGripScript.forcedGrip && !rightGripScript.forcedGrip))
        {
            versusGripTimer -= Time.deltaTime;

            if (leftGripScript != null && leftGripScript.versusGripController != null)
                leftGripScript.versusGripController.SetTime(versusGripTimer, maxVersusGripTime, true);
            if (rightGripScript != null && rightGripScript.versusGripController != null)
                rightGripScript.versusGripController.SetTime(versusGripTimer, maxVersusGripTime, true);
        }
        else
        {
            if (versusGripTimer < maxVersusGripTime)
            {
                versusGripTimer += Time.deltaTime * 1.5f;
            }
        }
    }
    public void ActivateVersusController(HandGrip grip)
    {
        grip.versusGripController.SetState(true, versusGripTimer, maxVersusGripTime);
        grip.versusGripController.ActivateBoiler(maxVersusGripTime - versusGripTimer);
    }
    public void ResetVersusTimer()
    {
        versusGripTimer = maxVersusGripTime / 4;
    }

    void ControlScratch()
    {
        if (leftGripScript.isOnGrip || rightGripScript.isOnGrip)
            return;

        if (body.velocity.y > -1)
            return;


        float vertical = GameManager.GetThumbStick(XboxThumbStick.Left).y;
        if (forceArmsUp)
            vertical = 1f;

        if (leftGripScript.isOnWall && vertical != 0)
        {
            leftParticle.enableEmission = true;
        }


        vertical = GameManager.GetThumbStick(XboxThumbStick.Right).y;
        if (forceArmsUp)
            vertical = 1f;
        if (rightGripScript.isOnWall && vertical != 0) 
        {
            rightParticle.enableEmission = true;
        }
    }

    void ControlHandMouse(HandGrip handScript, HingeJoint2D joint, int motorDir, Rigidbody2D body, GripMagnet magnet, Transform hand, Transform handNeutral, Transform handOrigin, HandGrip otherGripScript)
    {
        if (handScript.disabled)
            return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        Vector3 input = mouseWorldPos - hand.position;
        input.Normalize();

        if (forceArmsUp)
        {
            if (input == Vector3.zero)
                input = new Vector3(0, 0.75f, 0);
            else
                forceArmsUp = false;
        }
        ///////////////////////////////////////////////////////////////////////////
        //                      Is hand gripping or not?
        ///////////////////////////////////////////////////////////////////////////
        bool grip = joint.useMotor = handScript.isOnGrip;
        body.isKinematic = false;

        

        ///////////////////////////////////////////////////////////////////////////
        //                      Make hand heave when gripping
        ///////////////////////////////////////////////////////////////////////////
        if (leftGripScript.isOnGrip && !rightGripScript.isOnGrip && gameManager.tutorialComplete)
        {
            armsCantGoDownTimer += Time.deltaTime;

            if (input.y < 0 && armsCantGoDownTimer < 3f)
            {
                input.y = 0.5f;
            }
        }
        else
        {
            armsCantGoDownTimer = 0;
        }

        ///////////////////////////////////////////////////////////////////////////
        //                      Make hand go up when no hands are gripping
        ///////////////////////////////////////////////////////////////////////////
        if (handScript.isOnGrip)
        {
            input.y = (input.y * -1);
            input.x = 0;
        }

        ///////////////////////////////////////////////////////////////////////////
        //                      Aquire angle from input
        ///////////////////////////////////////////////////////////////////////////
        float angle = GetAngleFromInput(input);



        ///////////////////////////////////////////////////////////////////////////
        //                      Set appropriate settings for the motor
        ///////////////////////////////////////////////////////////////////////////
        HingeJoint2D otherJoint = null;
        JointMotor2D motor = new JointMotor2D();
        motor.motorSpeed = motorSpeed;

        if (oneArmedModeEnabled)
        {
            motor.motorSpeed += oneArmedMotorBoost;
        }

        if (versusHands > 0)
            motor.motorSpeed += versusMotorBoost;
        else if (rightGripScript.isOnGrip && leftGripScript.isOnGrip)
        {
            motor.motorSpeed /= 1.2f;
        }

        if (joint == leftJoint)
            otherJoint = rightJoint;
        else
        {
            motor.motorSpeed *= -1;
            otherJoint = leftJoint;
        }

        motor.maxMotorTorque = 1500;
        joint.motor = motor;
        joint.useMotor = (grip && input.y < 0);


        ///////////////////////////////////////////////////////////////////////////
        //                  Steer hand in different directions
        ///////////////////////////////////////////////////////////////////////////
        if (!grip)
        {
            //Move towards joystick Direction
            if ((input.x != 0 || input.y != 0))
            {
                Vector3 dir = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle));

                if (!GameManager.DigitalInput)
                {
                    dir.x *= Mathf.Abs(input.x * 1.3f);
                    dir.y *= Mathf.Abs(input.y * 1.3f);
                }

                Vector3 targetPosition = handOrigin.position + dir * 2.0f + magnet.magnetDir;
                body.velocity = (targetPosition - hand.position) * speed;
            }
            // Move towards other hand when neutral
            else if (otherGripScript.isOnGrip && otherJoint.useMotor && handScript.isGripping)
            {
                if (otherGripScript.gripPoint)
                {
                    Vector3 targetPosition = otherGripScript.gripPoint.transform.position;
                    body.velocity = (targetPosition - hand.position) * speed;
                }
            }
            //Move towards neutral position
            else
            {
                Vector3 targetPosition = handNeutral.position;
                body.velocity = (targetPosition - hand.position) * speed;
            }
        }
    }
    void ControlHand(HandGrip handScript, Vector3 input, HingeJoint2D joint, int motorDir, Rigidbody2D body, GripMagnet magnet, Transform hand, Transform handNeutral, Transform handOrigin, HandGrip otherGripScript)
    {
        if (handScript.disabled)
            return;

        if (forceArmsUp)
        {
            if (input == Vector3.zero)
                input = new Vector3(0, 0.75f, 0);
            else
                forceArmsUp = false;
        }
        ///////////////////////////////////////////////////////////////////////////
        //                      Is hand gripping or not?
        ///////////////////////////////////////////////////////////////////////////
        bool grip = joint.useMotor = handScript.isOnGrip;
        body.isKinematic = false;



        ///////////////////////////////////////////////////////////////////////////
        //                      Make hand go up when no hands are gripping
        ///////////////////////////////////////////////////////////////////////////
        if (!leftGripScript.isOnGrip && !rightGripScript.isOnGrip && gameManager.tutorialComplete)
        {
            armsCantGoDownTimer += Time.deltaTime;

            if (input.y < 0 && armsCantGoDownTimer < 3f)
            {
                input.y = 0.5f;
            }
        }
        else
        {
            armsCantGoDownTimer = 0;
        }



        ///////////////////////////////////////////////////////////////////////////
        //                      Aquire angle from input
        ///////////////////////////////////////////////////////////////////////////
        float angle = GetAngleFromInput(input);



        ///////////////////////////////////////////////////////////////////////////
        //                      Set appropriate settings for the motor
        ///////////////////////////////////////////////////////////////////////////
        HingeJoint2D otherJoint = null;
        JointMotor2D motor = new JointMotor2D();
        motor.motorSpeed = motorSpeed;

        if (oneArmedModeEnabled)
        {
            motor.motorSpeed += oneArmedMotorBoost;
        }

        if (versusHands > 0)
            motor.motorSpeed += versusMotorBoost;
        else if (rightGripScript.isOnGrip && leftGripScript.isOnGrip)
        {
            motor.motorSpeed /= 1.2f;
        }

        if (joint == leftJoint)
            otherJoint = rightJoint;
        else
        {
            motor.motorSpeed *= -1;
            otherJoint = leftJoint;
        }

        motor.maxMotorTorque = 1500;
        joint.motor = motor;
        joint.useMotor = (grip && input.y < 0);


        ///////////////////////////////////////////////////////////////////////////
        //                  Steer hand in different directions
        ///////////////////////////////////////////////////////////////////////////
        if (!grip)
        {
            //Move towards joystick Direction
            if ((input.x != 0 || input.y != 0))
            {
                Vector3 dir = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle));

                if (!GameManager.DigitalInput)
                {
                    dir.x *= Mathf.Abs(input.x * 1.3f);
                    dir.y *= Mathf.Abs(input.y * 1.3f);
                }

                Vector3 targetPosition = handOrigin.position + dir * 2.0f + magnet.magnetDir;
                body.velocity = (targetPosition - hand.position) * speed;
            }
            // Move towards other hand when neutral
            else if (otherGripScript.isOnGrip && otherJoint.useMotor && handScript.isGripping)
            {
                if (otherGripScript.gripPoint)
                {
                    Vector3 targetPosition = otherGripScript.gripPoint.transform.position;
                    body.velocity = (targetPosition - hand.position) * speed;
                }
            }
            //Move towards neutral position
            else
            {
                Vector3 targetPosition = handNeutral.position;
                body.velocity = (targetPosition - hand.position) * speed;
            }
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
    float GetAngleFromInput(Vector3 input)
    {
        float angle = Mathf.Rad2Deg * (float)Mathf.Atan2(input.x, input.y);

        //Convert angle to stay between 0-360 degrees
        if (angle < 0)
        {
            angle = 180 + (180 - Mathf.Abs(angle));
        }

        if (GameManager.DigitalInput)
        {
            //Set angle to snap each 45 degree
            float i = (int)(angle / 45.0f);
            angle = (45 * i);
        }

        angle *= Mathf.Deg2Rad;

        return angle;
    }


    public void EnableTutorial()
    {
        tutorialSymbolLeft.SetActive(true);
        tutorialSymbolRight.SetActive(true);
    }
    public void DisableTutorial()
    {
        tutorialSymbolLeft.SetActive(false);
        tutorialSymbolRight.SetActive(false);
    }
}

