using UnityEngine;
using System.Collections;

public class VelocityVolume : MonoBehaviour {

    private AudioSource soundSource;

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
        soundSource.volume = body.velocity.magnitude / 30 * 1.5f;
        Debug.Log("sound volume " + soundSource.volume);
    }
}
