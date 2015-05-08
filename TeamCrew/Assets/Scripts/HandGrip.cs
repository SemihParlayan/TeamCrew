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
    public bool isVersusGripping;

    public bool JustGripped { get { return (!lastIsOngrip && isOnGrip); } }

    public bool allowNewGrip = true;
    private bool allowVersusGrab = true;

    //Axis of which to grip with
    public string axis;

    //Sprites for different hand states
    private SpriteRenderer renderer;
    public Sprite open;
    public Sprite semiOpen;
    public Sprite closed;

    //Current grip and joint
    public GripPoint gripPoint;
    private HingeJoint2D joint;

    //Sound
    public AudioSource gripSoundSource;
    public AudioSource scream;
    private RandomSoundFromList randSoundGen;

    private AudioSource wallScratchSource;

    //Insect reference
    private Insect insectScript;

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

	void Start () 
    {
        //Aquire spriterenderer and sound
        renderer = GetComponent<SpriteRenderer>();
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

        //Set Scratch sound Source
        wallScratchSource = transform.GetComponent<AudioSource>();
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
        if (Input.GetButton(axis)) //Grip button down is down
        {
            //Set gripping to true
            isGripping = true;
            gameManager.DeactivateInactivityCounter(axis);

            //Change hand sprite to semi-open
            if (!isOnGrip)
            {
                renderer.sprite = semiOpen;
                gripAnimation.Activate("air");
            }
        }
        else if (Input.GetButtonUp(axis)) //Grip button goes up
        {
            ReleaseGrip();
            if (insectScript != null)
            {
                insectScript.RemoveHand();
                insectScript = null;
            }
        }

        lastIsOngrip = isOnGrip;
	}
    bool AllowGrip(Grip g)
    {
        //Find name of the hand
        string holdername = axis.Substring(0, 2);

        //Check for grip input
        if (Input.GetButton(axis) && !isOnGrip)
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

                            renderer.sprite = closed;
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
                    if (!gameManager.tutorialComplete)
                    {
                        bool allow = true;
                        if (!allowNewGrip)
                        {
                            allow = false;
                            if (g.tutorialStart)
                            {
                                allow = true;
                            }
                        }
                        if (!allow)
                        {
                            return false;
                        }
                    }
                    //Play animation and stone particles
                    stoneParticles.Play();
                    gripAnimation.Activate("normal");

                    //Hand is on a grip
                    isOnGrip = true;

                    //Set grips holder and number of hands
                    gripPoint.holderName = holdername;
                    gripPoint.numberOfHands++;

                    //Change hand sprite
                    renderer.sprite = closed;

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
                            gameManager.Win();
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
        if (c.transform.tag == "Grip")
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
                            scream.volume = 0.35f;
                            scream.pitch = Random.Range(1.1f, 1.4f);
                            scream.Play();
                        }
                        else
                        {
                            Debug.Log("ERROR: Scream sound is missing!");
                        }
                    }
                }
            }
        }
        else if (c.transform.tag == "Insect")
        {
            MovingGrip movingGrip = c.transform.GetComponent<MovingGrip>();

            if (movingGrip)
            {
                if (AllowGrip(movingGrip))
                {
                    joint.connectedBody = movingGrip.connectedBody;
                    joint.connectedAnchor = movingGrip.anchor;

                    insectScript = c.transform.parent.GetComponent<Insect>();

                    if (insectScript != null)
                    {
                        insectScript.AddHand();
                    }
                }
            }
        }
        else if (c.transform.tag == "Wall")
        {
            isOnWall = true;
        }
    }
    void OnTriggerExit2D(Collider2D c)
    {
        if (c.transform.tag == "Wall")
        {
            isOnWall = false;
        }
    }

    public void ReleaseGrip()
    {
        //Reset hand sprite
        if (isOnGrip)
            renderer.color = Color.white;
        renderer.sprite = open;

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
    }
    public void ReleaseVersusGrip(float grabDelay)
    {
        if (isVersusGripping)
            ReleaseGrip();

        if (!isOnGrip)
            renderer.color = Color.red;

        allowVersusGrab = false;
        Invoke("AllowVersusGrab", grabDelay);
    }
    private void AllowVersusGrab()
    {
        renderer.color = Color.white;
        allowVersusGrab = true;
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
