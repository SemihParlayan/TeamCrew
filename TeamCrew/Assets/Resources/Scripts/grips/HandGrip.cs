using UnityEngine;
using System.Collections;

[RequireComponent(typeof(VersusGripController))]
public class HandGrip : MonoBehaviour
{
    //Hand states
    public bool isOnGrip;
    public bool lastIsOngrip;
    public bool isOnWall;
    public bool isGripping;
    public bool isGrippingTutorial;
    public bool isGrippingInsect;
    public bool isVersusGripping;
    public bool hackGrip;

    public bool JustGripped { get { return (!lastIsOngrip && isOnGrip); } }

    private bool allowNewGrip = true;
    private bool allowVersusGrab = true;

    //Time of last grip
    public float lastGripTime;

    //Axis of which to grip with
    public string axis;

    //Sprites for different hand states
    private SpriteRenderer spriteRenderer;
    public Sprite open;
    public Sprite semiOpen;
    public Sprite closed;

    //Current grip and joint
    public GripPoint gripPoint;
    private HingeJoint2D joint;

    //Locked
    private bool handIsLocked;
    private float handLockTimer;

    //Sound
    public AudioSource gripSoundSource;
    public AudioSource scream;
    private RandomSoundFromList randSoundGen;

    //Insect reference
    public Insect insectScript;

    //Game manager reference
    private GameManager gameManager;

    //Versus frog reference
    private FrogPrototype versusFrog;

    public ParticleSystem stoneParticles;

    //Grip animations
    public GripAnimation gripAnimation;

    //VersusGripController
    [HideInInspector]
    public VersusGripController versusGripController;

    public Vector3 GripPosition
    {
        get 
        {
            return gripPoint.transform.position;
        }
    }
    private bool lastGripValue;

	void Start () 
    {
        //Aquire spriterenderer and sound
        spriteRenderer = GetComponent<SpriteRenderer>();
        randSoundGen = gripSoundSource.GetComponent<RandomSoundFromList>();
        
        //Aquire joint and disable it
        joint = transform.parent.GetComponent<HingeJoint2D>();
        joint.enabled = false;

        //Aquire game manager
        GameObject game = GameObject.FindWithTag("GameManager");
        if (game)
        {
            gameManager = game.GetComponent<GameManager>();
        }

        //scream sound
        scream = transform.GetComponent<AudioSource>();

        versusGripController = GetComponent<VersusGripController>();
	}

	void Update ()
    {
        if (gameManager)
        {
            if (!gameManager.gameActive)
                return;
        }

        //Set last grip time
        if (JustGripped)
        {
            lastGripTime = Time.timeSinceLevelLoad;
        }

        isGripping = false;
        if (GameManager.GetGrip(axis) || hackGrip) //Grip button down is down
        {
            //Set gripping to true
            isGripping = true;

            //Change hand sprite to semi-open
            if (!isOnGrip)
            {
                spriteRenderer.sprite = semiOpen;
                gripAnimation.Activate("air");
            }
        }
        else if (lastGripValue && ! (GameManager.GetGrip(axis) || hackGrip)) //Grip button goes up
        {
            if (!handIsLocked)
            {
                ReleaseGrip();
            }
        }

        lastIsOngrip = isOnGrip;

        if (handLockTimer > 0)
        {
            handLockTimer -= Time.deltaTime;

            if (handLockTimer <= 0)
            {
                handLockTimer = 0;
                DeLockHand();
            }
        }

        lastGripValue = GameManager.GetGrip(axis) || hackGrip;
	}
    bool AllowGrip(Grip g)
    {
        if (!allowNewGrip)
        {
            return false;
        }

        //Find name of the hand
        string holdername = axis.Substring(0, 2);

        //Check for grip input
        if ((GameManager.GetGrip(axis) || hackGrip) && !isOnGrip)
        {
            //Aquire grip point
            gripPoint = g.GetClosestGrip(transform.position);

            //Do we have a grip point?
            if (gripPoint != null)
            {
                if (g is MovingGrip && gripPoint.holderName != string.Empty)
                {
                    //VERSUS GRIP
                    if (holdername == gripPoint.holderName)
                    {
                        if (allowVersusGrab)
                        {
                            isOnGrip = true;
                            gripPoint.holderName = holdername;
                            gripPoint.numberOfHands++;

                            spriteRenderer.sprite = closed;
                            joint.enabled = true;

                            randSoundGen.GenerateGrip();
                            gripSoundSource.Play();
                            return true;
                        }
                    }
                }
                else
                {
                    //NORMAL AND MOVING GRIP


                    //Play animation and stone particles
                    stoneParticles.Play();
                    gripAnimation.Activate("normal");

                    //Hand is on a grip
                    isOnGrip = true;

                    //Set grips holder and number of hands
                    gripPoint.holderName = holdername;
                    gripPoint.numberOfHands++;

                    //Change hand sprite
                    spriteRenderer.sprite = closed;

                    //Enable hand joints
                    joint.enabled = true;
                    joint.connectedAnchor = gripPoint.transform.position;

                    //Playing a randomly chosen grip sound
                    randSoundGen.GenerateGrip();
                    gripSoundSource.Play();

                    if (g.winningGrip)
                    {
                        if (gameManager)
                        {
                            int frogNumber = int.Parse(axis[1].ToString());
                            gameManager.Win(frogNumber);
                        }
                    }
                    else if (g.tutorialStart)
                    {
                        isGrippingTutorial = true;
                    }
                    return true;
                }
            }
        }
        return false;
    }
    void OnTriggerStay2D(Collider2D c)
    {
        if (gameManager)
        {
            if (!gameManager.gameActive)
                return;
        }

        if (c.transform.tag == "Insect")
        {
            insectScript = c.transform.parent.GetComponent<Insect>();
            if (insectScript != null)
            {
                if (insectScript.motionState == MotionState.chasing)
                {
                    MovingGrip movingGrip = c.transform.GetComponent<MovingGrip>();
                    if (movingGrip)
                    {
                        if (AllowGrip(movingGrip))
                        {
                            joint.connectedBody = movingGrip.connectedBody;
                            joint.connectedAnchor = movingGrip.anchor;
                            insectScript.AddHand();
                            LockHand(1.5f);
                            isGrippingInsect = true;
                        }
                    }
                }
            }
            
        }
        else if (c.transform.tag == "Grip")
        {
            //Aquire grip script
            Grip grip = c.transform.GetComponent<Grip>();

            if (grip)
            {
                //Attach to grip if possible
                AllowGrip(grip);
            }

        }
        else if (c.transform.tag == "VersusGrip" || c.transform.tag == "MovingGrip")
        {
            //Aquire moving grip script
            MovingGrip movingGrip = c.transform.GetComponent<MovingGrip>();

            if (movingGrip)
            {
                //Attach to moving grip if possible
                if (AllowGrip(movingGrip))
                {
                    joint.connectedBody = movingGrip.connectedBody;
                    joint.connectedAnchor = movingGrip.anchor;

                    if (c.transform.tag == "VersusGrip")
                    {
                        isVersusGripping = true;
                        versusGripController.ActivateBlink();
                        versusFrog = FindVersusBody(c.transform).GetComponent<FrogPrototype>();
                        versusFrog.versusHands++;

                        //HERE IS CODE:
                        if (scream)
                        {
                            scream.volume = 0.15f;
                            scream.pitch = Random.Range(1.1f, 1.4f);
                            scream.Play();
                        }
                        else
                        {
                            Debug.LogError("ERROR: Scream sound is missing!");
                        }
                    }   
                }
            }
        }
        else if (c.transform.tag == "Wall")
        {
            isOnWall = true;
        }

        if (!isGrippingInsect)
        {
            insectScript = null;
        }
    }
    void OnTriggerExit2D(Collider2D c)
    {
        if (c.transform.tag == "Wall")
        {
            isOnWall = false;
        }
    }

    void DeLockHand()
    {
        handIsLocked = false;
        if (!isGripping)
        {
            ReleaseGrip();
        }
    }
    void LockHand(float time)
    {
        handIsLocked = true;
        handLockTimer = time;
    }
    public void ReleaseGrip(float newGripDelay = 0)
    {
        //Reset hand sprite
        if (isOnGrip)
            spriteRenderer.color = Color.white;
        spriteRenderer.sprite = open;

        //If hand is on a grip
        if (isOnGrip)
        {
            //Reset on grip
            isOnGrip = false;

            //Decrease number of hands on grip point
            gripPoint.numberOfHands--;

            //if hand count is zero.... reset owner of the grip
            if (gripPoint.numberOfHands <= 0 && !isVersusGripping)
            {
                gripPoint.holderName = "";
            }

            //Disable connected hand joint
            joint.enabled = false;
            joint.connectedBody = null;

            //Disable grip point
            gripPoint = null;

            //Play release sound
            randSoundGen.GenerateRelease();
            gripSoundSource.Play();
        }

        //Reset grip
        isGripping = false;
        isVersusGripping = false;
        isGrippingTutorial = false;
        versusGripController.DeActivateBlink();

        if (versusFrog)
            versusFrog.versusHands--;

        if (isGrippingInsect)
        {
            insectScript.RemoveHand();
            insectScript = null;
        }
        isGrippingInsect = false;

        if (newGripDelay > 0)
        {
            allowNewGrip = false;
            Invoke("ActivateAllowGrip", newGripDelay);
        }
    }
    public void ReleaseVersusGrip(float grabDelay)
    {
        if (isVersusGripping)
            ReleaseGrip();

        if (!isOnGrip)
            spriteRenderer.color = Color.red;

        allowVersusGrab = false;
        Invoke("AllowVersusGrab", grabDelay);
    }
    private void AllowVersusGrab()
    {
        spriteRenderer.color = Color.white;
        allowVersusGrab = true;
    }
    private void ActivateAllowGrip()
    {
        allowNewGrip = true;
    }
    Transform FindVersusBody(Transform branch)
    {
        while(branch.FindChild("body") == null)
        {
            branch = branch.parent;
        }

        return branch.FindChild("body");
    }
}
