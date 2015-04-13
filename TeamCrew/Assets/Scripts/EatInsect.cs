using UnityEngine;
using System.Collections;

public class EatInsect : MonoBehaviour {
    private FrogPrototype frogScript;

	void Start () {
        frogScript = GetComponent<FrogPrototype>();
	}
	
	void Update () {
	    
	}

    void OnTriggerEnter2D(Collider2D c)
    {
        if(c.transform.tag == "Insect")
        {
            Destroy(c.gameObject);
            frogScript.EnergyBoost();
        }
    }
}
