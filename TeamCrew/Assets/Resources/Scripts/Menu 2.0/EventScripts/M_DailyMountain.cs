using UnityEngine;
using System.Collections;

public class M_DailyMountain : M_EventBase 
{
	//publics
    public M_Screen dailyMountainSreeen;
    public GameObject steamConnectionObject;

	//privates


	//Unity methods
	void Start () 
	{
        steamConnectionObject.gameObject.SetActive(!SteamManager.Initialized);
	}
    void Update()
    {
    }

	//public methods
    public override void OnPress()
    {
        base.OnPress();
        M_ScreenManager.SwitchScreen(dailyMountainSreeen);
        if (SteamManager.Initialized)
        {
            M_ScreenManager.SwitchScreen(dailyMountainSreeen);
        }
        else
        {
            //The player has not connectec to steam properly
            //Play deny sounds?
        }
    }

	//private methods
}
