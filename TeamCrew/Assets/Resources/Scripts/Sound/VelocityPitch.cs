using UnityEngine;
using System.Collections;

public class VelocityPitch : MonoBehaviour {

    public AudioSource soundSource;
    public float pitchModifier = 45f;

    Rigidbody2D body;
	void Start () {
        body = GetComponent<Rigidbody2D>();
	}

    void FixedUpdate()
    {
        soundSource.pitch = 1 + body.velocity.magnitude / pitchModifier;
    }
}
