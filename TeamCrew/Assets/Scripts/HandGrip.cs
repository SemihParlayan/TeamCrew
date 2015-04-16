﻿using UnityEngine;
using System.Collections;

public class HandGrip : MonoBehaviour
{
    public bool isOnGrip;
    public bool isOnWall;
    public bool isGripping;
    public Sprite open;
    public Sprite closed;
    public string axis;
    private SpriteRenderer renderer;

    private Vector3 offset;

    private Insect insectScript;
    public GripPoint gripPoint;

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
	}
	
	void Update ()
    {
        if(Input.GetButton(axis))
        {
            isGripping = true;
        }
        else if (Input.GetButtonUp(axis))
        {
            isGripping = false;
            renderer.sprite = open;
            if (isOnGrip)
            {
                isOnGrip = false;
                gripPoint.holderName = "";
                gripPoint = null;
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
            gripPoint = null;
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
                        gripPoint.holderName = holdername;
                        isOnGrip = true;
                        renderer.sprite = closed;
                    }
                }
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
