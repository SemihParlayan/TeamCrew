using UnityEngine;
using System.Collections;

public class ImpactSound : MonoBehaviour
{
    AudioSource aSource;
    RandomSoundFromList randScript;

    void OnBecameVisible()
    {
        enabled = true;

    }

    void OnBecameInvisible()
    {
        enabled = false;
    }


	void Start ()
    {
        aSource = transform.GetComponent<AudioSource>();
        randScript = transform.GetComponent<RandomSoundFromList>();


        randScript.GenerateRockImpact();
	}

	void Update ()
    {
	
	}

    void OnCollisionEnter2D(Collision2D c)
    {
        //Debug.Log(c.transform.tag);
        //Semih it is untagged. Why?
        //if (c.transform.tag == "Frog")
        {
            //Debug.Log(c.relativeVelocity.magnitude);
            aSource.volume = c.relativeVelocity.magnitude * 0.05f;
            //aSource.volume = c.relativeVelocity.Scale;
            randScript.GenerateRockImpact();
            aSource.Play();
        }
    }
}
