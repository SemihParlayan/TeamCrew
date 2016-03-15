using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class BreakingVine : MonoBehaviour
{
    //publics
    [Range(0f, 10f)]
    public float delay;
    public AudioSource breakAudio;
    public Grip[] snappingGrips;

    //privates
    private HingeJoint2D breakJoint;
    private bool snapped = false;

	// Use this for initialization
	void Start () 
    {
        breakAudio = gameObject.AddComponent<AudioSource>();
        breakAudio.playOnAwake = false;
        breakAudio.outputAudioMixerGroup = M_Sounds.vineSnapGroupStatic;
        breakAudio.clip = Resources.Load<AudioClip>("Audio/Sound/vinesnap_ww_buildup");
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
        if (breakAudio != null)
        {
            breakAudio.Play();
        }
        Invoke("Break", delay);
    }
    private void Break()
    {
        breakJoint.enabled = false;
    }
}
