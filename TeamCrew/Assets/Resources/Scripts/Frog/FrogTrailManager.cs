using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FrogTrailManager : MonoBehaviour 
{
	//publics
    public float theshold = 8f;
	//privates
    private List<GhostTrail> ghostTrails = new List<GhostTrail>();
    private bool trailIsActive = true;

	//Unity methods
	void Start () 
	{
        ghostTrails = transform.GetComponentsInChildren<GhostTrail>().ToList();
        ActivateTrail();
	}
	void Update () 
	{

	}

	//public methods
    public void ActivateTrail()
    {
        if (trailIsActive)
            return;

        trailIsActive = true;
        foreach(GhostTrail trail in ghostTrails)
        {
            trail.Show();
        }
    }
    public void DeactivateTrail()
    {
        if (!trailIsActive)
            return;

        trailIsActive = false;
        foreach (GhostTrail trail in ghostTrails)
        {
            trail.Hide();
        }
    }
	//private methods
}
