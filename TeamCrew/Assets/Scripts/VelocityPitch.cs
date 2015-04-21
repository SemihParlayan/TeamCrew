using UnityEngine;
using System.Collections;

public class VelocityPitch : MonoBehaviour {

    public AudioSource soundSource;

    Rigidbody2D body;
	void Start () {
        body = GetComponent<Rigidbody2D>();
	}

    void FixedUpdate()
    {
        soundSource.pitch = 1 + body.velocity.magnitude / 45;
    }
}
