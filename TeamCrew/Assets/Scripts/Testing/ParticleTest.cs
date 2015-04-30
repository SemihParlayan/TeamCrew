using UnityEngine;
using System.Collections;

public class ParticleTest : MonoBehaviour 
{
    public ParticleSystem p;

	void Start () 
    {
        p = GetComponent<ParticleSystem>();
	}

	void Update () 
    {
        if (Input.GetKeyDown(KeyCode.B))
            p.Play();
	}
}
