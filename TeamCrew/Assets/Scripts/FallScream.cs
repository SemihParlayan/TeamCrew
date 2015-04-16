using UnityEngine;
using System.Collections;

public class FallScream : MonoBehaviour
{
    Rigidbody2D body;
    AudioSource mouth;
	void Start ()
    {
        body = GetComponent<Rigidbody2D>();
        mouth = GetComponent<AudioSource>();
	}
	
	
	void Update ()
    {
        if(!body.isKinematic)
        {
            if (body.velocity.y < -18)
            {
                if(!mouth.isPlaying)
                {
                    mouth.Play();
                }
                
            }
        }
	    
	}
}
