using UnityEngine;
using System.Collections;

public class FrogPrototype : MonoBehaviour
{
    public ParticleSystem rightParticle;
    public ParticleSystem leftParticle;

    public float speed;
    public float yVelocityClamp = 10;

    public string player;
    public Emotions emotionsScript;
    public HandGrip[] gripScript;

    public HingeJoint2D[] joints;

    public Transform[] handOrigin;
    public Transform[] handNeutral;
    public Transform[] hand;
    private Rigidbody2D[] handBody = new Rigidbody2D[2];
    public GripMagnet[] handMagnet;

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

    public Rigidbody2D body;

    public GripMagnet leftHandMagnet;
    public GripMagnet rightHandMagnet;

    public float motorSpeed = 350;
    public float versusMotorBoost = 350;

    public int versusHands;

    public bool Ready { get { return (leftGripScript.isGrippingTutorial || rightGripScript.isGrippingTutorial);} }

    private GameManager gameManager;

    private RandomSoundFromList leftHandSoundChooser;
    private RandomSoundFromList rightHandSoundChooser;

    private bool leftScratchSounding;
    private bool rightScratchSounding;

    private VelocityVolume velVolLeft;
    private VelocityVolume velVolRight;
        
    //Sound Stuff
    private AudioSource boilSounder;
    private AudioSource shhSounder;


    private bool uberHax = false;

    void Start()
    {
        leftBody  = handBody[0] = leftHand.GetComponent<Rigidbody2D>();
        if (leftBody == null) { Debug.Log("leftBody is null"); }

        
        rightBody = handBody[1] = rightHand.GetComponent<Rigidbody2D>();
        if (rightBody == null) { Debug.Log("rightBody is null");}


        body = GetComponent<Rigidbody2D>();
        if (body == null) { Debug.Log("body is null");}
        
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        if (gameManager == null) { Debug.Log("GameManager is null"); Debug.Break(); }


        leftHandSoundChooser = leftGripScript.GetComponentInChildren<RandomSoundFromList>();
        if (leftHandSoundChooser == null) { Debug.Log("leftHandSoundChooser is null"); }

        velVolLeft = leftGripScript.GetComponentInChildren<VelocityVolume>();
        if (velVolLeft == null) { Debug.Log("velVolLeft is null"); }


        rightHandSoundChooser = rightGripScript.GetComponentInChildren<RandomSoundFromList>();
        if (rightHandSoundChooser == null) { Debug.Log("rightHandSoundChooser is null"); }

        velVolRight = rightGripScript.GetComponentInChildren<VelocityVolume>();


        //Sound Stuff
        boilSounder = gameObject.AddComponent<AudioSource>();
        shhSounder = gameObject.AddComponent<AudioSource>();

        boilSounder.clip = Resources.Load("kettle") as AudioClip;
        shhSounder.clip = Resources.Load("shh") as AudioClip;
    }

    private void Update()
    {
        if (gameManager.hacks)
        {
            if (Input.GetButtonDown("Select") || Input.GetKey(KeyCode.B))
            {
                body.AddForce(new Vector2(0, 100000));
            }
            if (uberHax)
            {
                if(Input.GetMouseButton(0))
                {
                    body.isKinematic = false;
                    body.velocity = Vector2.zero;
                    body.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    handBody[0].position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0,2.5f,0);
                }
                if(Input.GetMouseButtonDown(1))
                {
                    leftGripScript.hackGrip = true;
                }
                else if(Input.GetMouseButtonUp(1))
                {
                    leftGripScript.hackGrip = false;
                }
            }
            else if (Input.GetKeyDown(KeyCode.H))
            {
                uberHax = true;
            }
        }
    }
    private void FixedUpdate()
    {
        //Disable new grips on tutorial block
        leftGripScript.allowNewGrip = !Ready;
        rightGripScript.allowNewGrip = !Ready;

        //Keeps body still until a grip is made
        ActivateBody();

        //Activate scratch
        rightParticle.enableEmission = false;
        leftParticle.enableEmission = false;
        ControlScratch();

        //Control Hands
        ControlHand(leftGripScript, GameManager.GetInput(player + "HL", player + "VL"), leftJoint, 1, leftBody, leftHandMagnet, leftHand, leftHandNeutral, leftHandOrigin, rightGripScript);
        ControlHand(rightGripScript, GameManager.GetInput(player + "HR", player +"VR"), rightJoint, -1, rightBody, rightHandMagnet, rightHand, rightHandNeutral, rightHandOrigin, leftGripScript);

        //Shake loose body
        ShakeLooseBody();

        //Limit y velocity for body
        Vector2 velocity = body.velocity;
        velocity.y = Mathf.Clamp(velocity.y, -int.MaxValue, yVelocityClamp);
        body.velocity = velocity;

        //scratch rotation
        leftHandMagnet.transform.rotation = leftHandMagnet.transform.parent.rotation;
        rightHandMagnet.transform.rotation = leftHandMagnet.transform.parent.rotation;

        //left Scratch sound
         if(leftParticle.enableEmission == true)
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
        if (rightParticle.enableEmission == true)
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

    private float maxVersusGripTime = 8.0f;
    public float versusGripTimer = 8.0f;

    void ShakeLooseBody()
    {
        if (leftGripScript.isVersusGripping || rightGripScript.isVersusGripping)
        {
            versusGripTimer -= Time.deltaTime;


            if (!boilSounder.isPlaying)
            {
                boilSounder.time = Mathf.Clamp(8.0f - versusGripTimer, 0, 8.0f);
                boilSounder.volume = 1;
                boilSounder.Play();
            }

            

            leftGripScript.versusGripController.blinkTime = versusGripTimer / maxVersusGripTime;
            rightGripScript.versusGripController.blinkTime = versusGripTimer / maxVersusGripTime;
            //Release versus grips
            if (versusGripTimer <= 0)
            {
                versusGripTimer = maxVersusGripTime;

                leftGripScript.ReleaseVersusGrip(1.0f);
                rightGripScript.ReleaseVersusGrip(1.0f);

                //Release sound
                shhSounder.volume = .1f;
                shhSounder.Play();
            }
        }
        else
        {
            if (versusGripTimer < maxVersusGripTime)
            {
                versusGripTimer += Time.deltaTime * 1.5f;
            }
            if (boilSounder.isPlaying)
            {
                boilSounder.Stop();
            }
           
        }
    }
    void ControlScratch()
    {
        if (leftGripScript.isOnGrip || rightGripScript.isOnGrip)
            return;

        if (body.velocity.y > -1)
            return;


        float vertical = GameManager.GetInput("P1HL", player + "VL").y;

        if (leftGripScript.isOnWall && vertical != 0)
        {
            leftParticle.enableEmission = true;
        }


        vertical = GameManager.GetInput("P1HL", player + "VR").y;

        if (rightGripScript.isOnWall && vertical != 0) 
        {
            rightParticle.enableEmission = true;
        }
    }
	
    void ControlHand2(int a)
    {
        //leftGripScript, player + "HL", player + "VL", leftJoint, 1, leftBody, leftHandMagnet, leftHand, leftHandNeutral, leftHandOrigin, rightGripScript
        //rightGripScript, player + "HR", player + "VR", rightJoint, -1, rightBody, rightHandMagnet, rightHand, rightHandNeutral, rightHandOrigin, leftGripScript);
        string horizontalAxis = player + (a == 0 ? "HLX" : "HRX");
        string verticalAxis   = player + (a == 0 ? "VLX" : "VRX");
        int motorDir = (a == 0 ? 1 : -1);

        joints[a].useMotor = gripScript[a].isOnGrip;
        bool grip = gripScript[a].isOnGrip;
        handBody[a].isKinematic = false;

        Vector3 input = new Vector3(Input.GetAxis(horizontalAxis), Input.GetAxis(verticalAxis));
        if (!leftGripScript.isOnGrip && !rightGripScript.isOnGrip)
        {
            if (input.y < 0)
            {
                input.y = 0.5f;
            }
        }

        float angle = Mathf.Rad2Deg * (float)Mathf.Atan2(input.x, input.y);
        if (angle < 0)
        {
            angle = 180 + (180 - Mathf.Abs(angle));
        }
        float i = (int)(angle / 45.0f);
        angle = (45 * i) * Mathf.Deg2Rad;


        HingeJoint2D otherJoint = null;
        JointMotor2D motor = new JointMotor2D();
        motor.motorSpeed = motorSpeed;

        if (versusHands > 0)
            motor.motorSpeed += versusMotorBoost;
        else if (rightGripScript.isOnGrip && leftGripScript.isOnGrip)
        {
            motor.motorSpeed /= 1.2f;
        }

        if (joints[a] == leftJoint)
        {
            otherJoint = rightJoint;
        }
        else
        {
            motor.motorSpeed *= -1;
            otherJoint = leftJoint;
        }

        motor.maxMotorTorque = 1500;
        joints[a].motor = motor;
        joints[a].useMotor = (grip && input.y < 0);


        if (!grip)
        {
            if ((input.x != 0 || input.y != 0)) //If hand is moving and not on a grip
            {
                //Move towards joystick Direction
                Vector3 dir = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle));
                Vector3 targetPosition = handOrigin[a].position + dir * 2.0f + handMagnet[a].magnetDir;
                handBody[a].velocity = (targetPosition - hand[a].position) * speed;
            }
            else if (gripScript[1-a].isOnGrip && otherJoint.useMotor && gripScript[a].isGripping) // Move towards other hand when neutral
            {
                Vector3 targetPosition = gripScript[1-a].gripPoint.transform.position;
                handBody[a].velocity = (targetPosition - hand[a].position) * speed;
            }
            else //If hand is not moving and not on grip
            {
                //Move towards neutral position
                Vector3 targetPosition = handNeutral[a].position;
                handBody[a].velocity = (targetPosition - hand[a].position) * speed;
            }
        }
    }
    void ControlHand(HandGrip handScript, Vector3 input, HingeJoint2D joint, int motorDir, Rigidbody2D body, GripMagnet magnet, Transform hand, Transform handNeutral, Transform handOrigin, HandGrip otherGripScript)
    {
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
            if (input.y < 0)
            {
                input.y = 0.5f;
            }
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
                Vector3 targetPosition = otherGripScript.gripPoint.transform.position;
                body.velocity = (targetPosition - hand.position) * speed;
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
}

