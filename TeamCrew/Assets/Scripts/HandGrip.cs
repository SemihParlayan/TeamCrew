using UnityEngine;
using System.Collections;

public class HandGrip : MonoBehaviour
{
    public bool isVersus;
    public bool isOnGrip;
    public bool isOnWall;
    public bool isGripping;
    public string axis;
    public Sprite open;
    public Sprite semiOpen;
    public Sprite closed;

    public GripPoint gripPoint;

    private HingeJoint2D joint;
    private Vector2 jointDefaultAnchor;

    public AudioSource gripSoundSource;

    private RandomSoundFromList randSoundGen;

    private SpriteRenderer renderer;
    private Vector3 offset;
    private Insect insectScript;
    

    

    public Vector3 GripPosition
    {
        get 
        {
            return gripPoint.transform.position;
        }
    }

	void Start () 
    {
        renderer = GetComponent<SpriteRenderer>();
        randSoundGen = gripSoundSource.GetComponent<RandomSoundFromList>();

        joint = transform.parent.GetComponent<HingeJoint2D>();
        joint.enabled = false;
        jointDefaultAnchor = joint.connectedAnchor;
	}
	
	void Update ()
    {
        if(Input.GetButton(axis))
        {
            isGripping = true;
            if (!isOnGrip)
                renderer.sprite = semiOpen;
        }
        else if (Input.GetButtonUp(axis))
        {
            isGripping = false;
            renderer.sprite = open;
            if (isOnGrip)
            {
                isOnGrip = false;
                gripPoint.numberOfHands--;

                if (gripPoint.numberOfHands <= 0)
                {
                    gripPoint.holderName = "";
                }
                joint.enabled = false;
                joint.connectedBody = null;
                isVersus = false;
                gripPoint = null;

                //Playing release sound
                randSoundGen.GenerateRelease();
                gripSoundSource.Play();

                
            }
            else if (insectScript != null)
            {
                insectScript.SetParalyze(false);
            }
        }
	}
    public void ResetGrip()
    {
        isOnGrip = false;
        if (gripPoint)
        {
            gripPoint.holderName = "";
            gripPoint.numberOfHands--;
            gripPoint = null;
            renderer.sprite = open;
        }
    }

    void OnTriggerStay2D(Collider2D c)
    {
        if (c.transform.tag == "Grip")
        {
            string holdername = axis.Substring(0, 2);
            if (Input.GetButton(axis) && !isOnGrip)
            {
                gripPoint = c.GetComponent<Grip>().GetClosestGrip(transform.position, holdername);
                if (gripPoint != null)
                {
                    if (gripPoint.holderName == string.Empty || gripPoint.holderName == holdername)
                    {
                        if (gripPoint.numberOfHands < 3)
                        {
                            gripPoint.holderName = holdername;
                            gripPoint.numberOfHands++;
                            isOnGrip = true;
                            
                            renderer.sprite = closed;

                            joint.enabled = true;
                            Transform parentparent = gripPoint.transform.parent.parent;
                            if (gripPoint.transform.parent.name.Contains("foot"))
                            {
                                joint.connectedBody = c.transform.parent.GetComponent<Rigidbody2D>();
                                joint.connectedAnchor = jointDefaultAnchor;
                                isVersus = true;
                            }
                            else
                            {
                                joint.connectedAnchor = gripPoint.transform.position;
                            }

                            //Playing a randomly chosen grip sound
                            randSoundGen.GenerateGrip();
                            gripSoundSource.Play();

                            
                        }
                    }
                }
            }
        }
        /*else if (c.transform.tag == "Insect")
        {
            if (Input.GetButton(axis))
            {
                renderer.sprite = closed;
                insectScript = c.transform.GetComponent<Insect>();
                insectScript.SetParalyze(true);
                insectScript.SetHand(transform);
            }
        }*/
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
}
