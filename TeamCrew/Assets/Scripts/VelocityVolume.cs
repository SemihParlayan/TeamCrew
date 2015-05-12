using UnityEngine;
using System.Collections;

public class VelocityVolume : MonoBehaviour {

    public AudioSource soundSource;

    Rigidbody2D body;
	void Start () {
        body = GetComponent<Rigidbody2D>();
        soundSource = GetComponent<AudioSource>();
        if(body == null)
        {
            body = GetComponentInParent<Rigidbody2D>();
        }
	}

    void FixedUpdate()
    {
        soundSource.volume = body.velocity.magnitude / 30 *.5f ;
        Debug.Log("my velocity is: " + body.velocity.magnitude);
        Debug.Log("volume is: " + soundSource.volume);
    }
}
