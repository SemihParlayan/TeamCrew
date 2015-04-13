using UnityEngine;
using System.Collections;

public class Grip : MonoBehaviour
{
    public Sprite open;
    public Sprite closed;
    public string axis;
    private SpriteRenderer renderer;
    private bool isOnGrip;

    private Vector3 offset;

    private Insect insectScript;
    public Transform grip;

    public Vector3 GripPosition
    {
        get
        {
            return grip.position;
        }
    }

	void Start () 
    {
        renderer = GetComponent<SpriteRenderer>();
	}
	
	void Update ()
    {
        if (Input.GetButtonUp(axis))
        {
            renderer.sprite = open;
            if (isOnGrip)
            {
                isOnGrip = false;
                grip = null;
            } 
            else if (insectScript != null)
            {
                insectScript.SetParalyze(false);
            }
        }
	}

    public bool IsOnGrip()
    {
        return isOnGrip;
    }

    void OnTriggerStay2D(Collider2D c)
    {
        if (c.transform.tag == "Grip")
        {
            if (Input.GetButton(axis))
            {
                isOnGrip = true;
                grip = c.transform;
                renderer.sprite = closed;
            }
        }
        else if (c.transform.tag == "Insect")
        {
            if (Input.GetButton(axis))
            {
                renderer.sprite = closed;
                insectScript = c.transform.GetComponent<Insect>();
                insectScript.SetParalyze(true);
                insectScript.SetHand(transform);
            }
        }
    }
}
