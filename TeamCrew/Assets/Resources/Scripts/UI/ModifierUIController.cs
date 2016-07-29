using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModifierUIController : MonoBehaviour {
    //magical seb script, watch out lol

    public List<GameObject> modifier_icons = new List<GameObject>();
    public float offset;
    int numberofactive = 0;

	// Use this for initialization
	void Start () {
	for(int i=0;i<modifier_icons.Count;i++)
        {
            modifier_icons[i].SetActive(false);

        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void updateNumberOfActive()
    {
        numberofactive = 0;
        for (int i = 0; i < modifier_icons.Count; i++)
        {
            if (modifier_icons[i].activeInHierarchy)
            {
                numberofactive++;
            }
        }
    }

    public void DisableAll()
    {
        for (int i = 0; i < modifier_icons.Count; i++)
        {
            deactivateicon(i);
        }
    }
    public void SwitchIconState(int iconID)
    {
        //refresh number of active icons

        updateNumberOfActive();

        //activate/deactivate target icon
        if(modifier_icons[iconID].activeInHierarchy)
        {
            deactivateicon(iconID);
        }
        else
        {
            activateicon(iconID);
        }

    }

    void deactivateicon(int iconID)
    {
        
        modifier_icons[iconID].SetActive(false);

        //loops through all icons and checks if their x position is larger than the just deactivated icon. If so they're moved to the left one offset to replace the old icon. Magic. 
        for(int i=0;i<modifier_icons.Count;i++)
        {
            if(modifier_icons[i].transform.position.x>modifier_icons[iconID].transform.position.x)
            {
                Vector3 oldpos = modifier_icons[iconID].transform.position;
                modifier_icons[i].transform.position=new Vector3(modifier_icons[i].transform.position.x - offset, oldpos.y, oldpos.z);

            }

        }
        modifier_icons[iconID].transform.position.Set(0, 0, 0);
        updateNumberOfActive();
    }

    void activateicon(int iconID)
    {
        
        modifier_icons[iconID].SetActive(true);
        Vector3 oldpos = modifier_icons[iconID].transform.position;
        modifier_icons[iconID].transform.position= new Vector3(numberofactive* offset,oldpos.y,oldpos.z);
        updateNumberOfActive();
    }
}
