using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ModIconController : MonoBehaviour {
    public List<GameObject> icons = new List<GameObject>();
    int currentID = 0;

	// Use this for initialization
	void Start () {
	
        for(int i=0;i<icons.Count;i++)
        {
            icons[i].SetActive(false);

        }

        icons[0].SetActive(true);

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void changeToNone()
    {
        changeIcon(0);  
    }
    public void changeToOneArm()
    {
        changeIcon(1);
    }
    public  void changeToLowGrav()
    {
        changeIcon(2);
    }
    public  void changeToKOTH()
    {
        changeIcon(3);
    }
    public void changeToBurningHands()
    {
        changeIcon(4);
    }

    void changeIcon(int ID)
    {
        icons[currentID].SetActive(false);
        icons[ID].SetActive(true);
        currentID = ID;
    }
    
}
