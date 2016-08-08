using UnityEngine;
using System.Collections;

public class M_ButtonDisabledSwitcher : M_EventBase 
{
	//publics
    public M_Button nextButton;

	//privates


	//Unity methods
	void Start () 
	{
	
	}
	void Update () 
	{
	
	}

	//public methods
    public void SwitchToButton(M_Screen screen)
    {
        screen.SwitchButton(nextButton);
    }
	//private methods
}
