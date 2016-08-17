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
        //steamConnectionObject.gameObject.SetActive(!SteamManager.Initialized);
        if (DateManager.searchingForInternet)
        {
            steamConnectionObject.transform.localPosition = new Vector3(0, -0.7f);
        }
	}
    void Update()
    {
    }

	//public methods
    public override void OnPress()
    {
        base.OnPress();
        if (SteamManager.Initialized && !DateManager.searchingForInternet)
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
