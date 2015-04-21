using UnityEngine;
using System.Collections;

public class VelocityPitch : MonoBehaviour {

    public AudioSource soundSource;

    Rigidbody2D body;
	void Start () {
        body = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        soundSource.pitch = 1 + body.velocity.magnitude / 20;
	}
}
