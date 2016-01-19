using UnityEngine;
using System.Collections;

public class BreakingVine : MonoBehaviour
{
    public Grip[] snappingGrips;
    private HingeJoint2D breakJoint;
    private bool snapped = false;

	// Use this for initialization
	void Start () 
    {
        breakJoint = GetComponent<HingeJoint2D>();
        if (breakJoint == null)
        {
            Debug.LogError("BreakingVine can't find hingejoint on object: " + transform.name);
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (snapped)
            return;

        foreach(Grip g in snappingGrips)
        {
            foreach(GripPoint p in g.gripPoints)
            {
                if (p.numberOfHands > 0)
                {
                    SnapVine();
                    break;
                }
            }

            if (snapped)
                break;
        }

	}

    private void SnapVine()
    {
        snapped = true;
        breakJoint.enabled = false;
    }
}
