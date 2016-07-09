using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ModIconController : MonoBehaviour {
    public List<GameObject> icons = new List<GameObject>();
    int currentID = 0;
    int IDtoShowWhenReady = 0;
   public  Animator anime;

	// Use this for initialization
	void Start () {
       // anime = GetComponent<Animator>();

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
        changeModIcon(0);  
    }
    public void changeToOneArm()
    {
        changeModIcon(1);
    }
    public  void changeToLowGrav()
    {
        changeModIcon(2);
    }
    public  void changeToKOTH()
    {
        changeModIcon(3);
    }
    public void changeToBurningHands()
    {
        changeModIcon(4);
    }

    void changeModIcon(int butt)//Starts the animator. 
    {
        IDtoShowWhenReady = butt;
        anime.SetTrigger("changeIcon");
      
        
    }
    public void updateIconGraphic() //called by ANIMATOR when the time is right
    {
        icons[currentID].SetActive(false);
        icons[IDtoShowWhenReady].SetActive(true);
        currentID = IDtoShowWhenReady;
    }
    
}
