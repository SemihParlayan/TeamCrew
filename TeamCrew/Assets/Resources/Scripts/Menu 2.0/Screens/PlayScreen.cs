using UnityEngine;
using System.Collections;

public class PlayScreen : M_Screen 
{
	//publics
    public GameObject frogClimbersLogo;

	//privates


	//Unity methods

	//public methods
    public override void OnSwitchedFrom()
    {
        base.OnSwitchedFrom();

        if (! (M_ScreenManager.GetCurrentScreen() is StartScreen))
        {
            frogClimbersLogo.SetActive(false);
        }
    }

	//private methods
}
