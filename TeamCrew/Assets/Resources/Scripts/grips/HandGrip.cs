using UnityEngine;
using System.Collections;

[RequireComponent(typeof(VersusGripController))]
public class HandGrip : MonoBehaviour
{
    //Hand states
    public bool JustGripped { get { return (!lastIsOngrip && isOnGrip); } }
    public bool JustReleased { get { return (lastIsOngrip && !isOnGrip); } }
    public bool isOnGrip;
    public bool lastIsOngrip;
    public bool isOnWall;
    public bool isGripping;
    public bool isGrippingTutorial;
    public bool isGrippingInsect;
    public bool isVersusGripping;
    public bool hackGrip;

    public bool allowNewGrip = true;
    private bool allowVersusGrab = true;

    //Time of last grip
    public float lastGripTime;

    //Axis of which to grip with
    public int player = int.MaxValue;
    public GripSide hand;

    //Sprites for different hand states
    private SpriteRenderer spriteRenderer;
    public Sprite open;
    public Sprite semiOpen;
    public Sprite closed;

    //Current grip and joint
    public MovingGrip movingGrip;
    public GripPoint gripPoint;
    private HingeJoint2D joint;

    //Locked
    public bool handIsLocked;
    public bool forcedGrip;
    public bool allowStoneGrip = true;

    //Sound
    public AudioSource gripSoundSource;
    public AudioSource scream;
    private RandomSoundFromList randSoundGen;

    //Insect reference
    public Insect insectScript;

    //Game manager reference
    private GameManager gameManager;

    //Versus frog reference
    public VersusHandGrip ownVersusHand;
    private FrogPrototype versusFrog;
    public HandGrip versusHand;

    public ParticleSystem stoneParticles;
    private BurningHands burningHand;

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


    private FrogPrototype parentFrog;
	void Start () 
    {
        //Aquire spriterenderer and sound
        burningHand = GetComponent<BurningHands>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (gripSoundSource != null)
            randSoundGen = gripSoundSource.GetComponent<RandomSoundFromList>();
        
        //Aquire joint and disable it
        joint = transform.parent.GetComponent<HingeJoint2D>();
        joint.enabled = false;
        allowStoneGrip = true;

        //Aquire game manager
        GameObject game = GameObject.FindWithTag("GameManager");
        if (game)
        {
            gameManager = game.GetComponent<GameManager>();
        }

        //scream sound
        scream = transform.GetComponent<AudioSource>();

        versusGripController = GetComponent<VersusGripController>();

        //Set correct axis controller
        parentFrog = transform.parent.parent.FindChild("body").GetComponent<FrogPrototype>();
        player = parentFrog.player;
	}

	void Update ()
    {
        if (gameManager)
        {
            if (!gameManager.gameActive && !forcedGrip)
                return;
        }

        if (movingGrip != null && gripPoint != null)
        {
            if (joint.connectedBody != movingGrip.connectedBody)
                joint.connectedBody = movingGrip.connectedBody;

            Vector3 newPos = movingGrip.connectedBody.transform.InverseTransformPoint(gripPoint.transform.position);
            float distance = Vector3.Distance(joint.connectedAnchor, newPos);
            if (distance > 0.5f)
            {
                joint.connectedAnchor = newPos;
            }
        }

        
        //Release versus hand
        if (versusHand != null)
        {
            if (versusHand.JustReleased)
            {
                versusHand.ReleaseGrip();
                ReleaseGrip();
            }
        }

        //Set last grip time
        if (JustGripped)
        {
            Vibration.instance.SetVibration(player, 0.6f, 0.6f, 0.1f);
            lastGripTime = Time.timeSinceLevelLoad;
        }

        isGripping = false;
        if (GameManager.GetGrip(player, hand) || forcedGrip || hackGrip) //Grip button down is down
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
        else if (lastGripValue && !(GameManager.GetGrip(player, hand) || hackGrip)) //Grip button goes up
        {
            if (!handIsLocked)
            {
                ReleaseGrip();
            }
        }
        if (versusGripController.Complete())
        {
            ReleaseGrip(1f);
            parentFrog.ResetVersusTimer();
        }

        if (burningHand != null)
            burningHand.OnUpdate(JustGripped, JustReleased);

        lastIsOngrip = isOnGrip;

        lastGripValue = GameManager.GetGrip(player, hand) || hackGrip;
	}
    bool AllowGrip(Grip newGrip)
    {
        if (!allowNewGrip)
        {
            return false;
        }
        //Check for grip input
        if ((GameManager.GetGrip(player, hand) || forcedGrip || hackGrip) && !isOnGrip)
        {
            //Aquire grip point
            gripPoint = newGrip.GetClosestGrip(transform.position);

            //Do we have a grip point?
            if (gripPoint != null)
            {
                if (newGrip is VersusHandGrip)
                {
                    VersusHandGrip versusHandGrip = newGrip as VersusHandGrip;

                    if (versusHandGrip.handScript.isGripping && !versusHandGrip.handScript.isOnGrip && versusHandGrip.handScript.player != player)
                    {
                        versusHand = versusHandGrip.handScript;
                        isOnGrip = true;
                        gripPoint.numberOfHands++;
                        spriteRenderer.sprite = closed;
                        joint.enabled = true;

                        randSoundGen.GenerateGrip();
                        gripSoundSource.Play();

                        versusHandGrip.handScript.versusHand = this;
                        versusHandGrip.handScript.isOnGrip = true;
                        versusHandGrip.handScript.spriteRenderer.sprite = versusHandGrip.handScript.closed;
                        versusHandGrip.handScript.joint.enabled = true;
                        return true;
                    }
                    return false;
                }
                else if (newGrip is VersusGrip)
                {
                    VersusGrip versusGrip = newGrip as VersusGrip;

                    //VERSUS GRIP
                    if (versusGrip.parentedPlayer != player)
                    {
                        if (allowVersusGrab)
                        {
                            isOnGrip = true;
                            gripPoint.numberOfHands++;

                            spriteRenderer.sprite = closed;
                            joint.enabled = true;

                            randSoundGen.GenerateGrip();
                            gripSoundSource.Play();
                            return true;
                        }
                    }
                }
                else if (allowStoneGrip)
                {
                    //NORMAL AND MOVING GRIP
                    if (newGrip is MovingGrip)
                    {
                        movingGrip = (MovingGrip)newGrip;
                    }
                    //Play animation and stone particles
                    stoneParticles.Play();
                    gripAnimation.Activate("normal");

                    //Hand is on a grip
                    isOnGrip = true;

                    //Set grips holder and number of hands
                    gripPoint.numberOfHands++;

                    //Change hand sprite
                    spriteRenderer.sprite = closed;

                    //Enable hand joints
                    joint.enabled = true;
                    joint.connectedAnchor = gripPoint.transform.position;

                    //Playing a randomly chosen grip sound
                    randSoundGen.GenerateGrip();
                    gripSoundSource.Play();

                    if (newGrip.winningGrip)
                    {
                        if (gameManager)
                        {
                            gameManager.Win(transform.parent.parent.GetComponentInChildren<FrogPrototype>().topPrefab, player);
                        }
                    }
                    else if (newGrip.tutorialStart)
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
            if (!gameManager.gameActive && !forcedGrip)
                return;
        }

        if (c.transform.tag == "Insect")
        {
            insectScript = c.transform.parent.GetComponent<Insect>();
            if (insectScript != null)
            {
                if (insectScript.motionState == MotionState.Chasing)
                {
                    MovingGrip movingGrip = c.transform.GetComponent<MovingGrip>();
                    if (movingGrip)
                    {
                        if (AllowGrip(movingGrip))
                        {
                            joint.connectedBody = movingGrip.connectedBody;
                            joint.connectedAnchor = gripPoint.transform.localPosition;
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
                if (movingGrip is VersusHandGrip)
                {
                    VersusHandGrip versusHandGrip = movingGrip as VersusHandGrip;
                    if (AllowGrip(movingGrip))
                    {
                        joint.connectedBody = movingGrip.connectedBody;
                        joint.connectedAnchor = movingGrip.connectedBody.transform.InverseTransformPoint(gripPoint.transform.position);

                        versusHandGrip.handScript.gripPoint = ownVersusHand.GetClosestGrip(ownVersusHand.transform.position);
                        versusHandGrip.handScript.joint.connectedBody = ownVersusHand.connectedBody;
                        versusHandGrip.handScript.joint.connectedAnchor = ownVersusHand.connectedBody.transform.InverseTransformPoint(versusHandGrip.handScript.gripPoint.transform.position);
                    }
                }
                else if (AllowGrip(movingGrip))
                {
                    joint.connectedBody = movingGrip.connectedBody;
                    joint.connectedAnchor = movingGrip.connectedBody.transform.InverseTransformPoint(gripPoint.transform.position);

                    if (c.transform.tag == "VersusGrip")
                    {
                        isVersusGripping = true;
                        if (!forcedGrip)
                            parentFrog.ActivateVersusController(this);

                        bool vibrate = (versusFrog == null);
                        versusFrog = FindVersusBody(c.transform).GetComponent<FrogPrototype>();
                        versusFrog.versusHands++;

                        if (versusFrog && vibrate && versusFrog.versusHands <= 1)
                        {
                            Vibration.instance.SetVibration(versusFrog.player, 0.4f, 0.4f, 0.7f);
                        }

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

    IEnumerator DeLockHand(float time)
    {
        yield return new WaitForSeconds(time);

        handIsLocked = false;
        if (!isGripping)
        {
            ReleaseGrip();
        }
    }
    public void LockHand(float time)
    {
        handIsLocked = true;
        StartCoroutine(DeLockHand(time));
    }
    public void SetForcedGrip(bool value, bool allowStoneGrip = true)
    {
        this.allowStoneGrip = allowStoneGrip;
        forcedGrip = value;
    }
    public void ReleaseGrip(float newGripDelay = 0)
    {
        if (forcedGrip)
            return;

        if (versusHand != null)
        {
            versusHand.versusHand = null;
            versusHand.ReleaseGrip();
            versusHand = null;
        }
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
            if (gripPoint != null)
                gripPoint.numberOfHands--;


            //Disable connected hand joint
            joint.enabled = false;
            joint.connectedBody = null;

            //Disable grip point
            gripPoint = null;
            movingGrip = null;

            //Play release sound
            randSoundGen.GenerateRelease();
            gripSoundSource.Play();
        }

        //Reset grip
        isGripping = false;
        isVersusGripping = false;
        isGrippingTutorial = false;
        versusGripController.SetState(false, 0f, 0f);

        if (versusFrog)
        {
            versusFrog.versusHands--;
            versusFrog = null;
        }

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
