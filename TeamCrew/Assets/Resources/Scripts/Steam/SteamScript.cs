using UnityEngine;
using System.Collections;
using Steamworks;

public class SteamScript : MonoBehaviour 
{
	//publics

	//privates

	//Unity methods
    void Start()
    {

    }
	void Update () 
	{
	
	}

	//public methods

	//private methods
    private void OnEnable()
    {
        if (SteamManager.Initialized)
        {
            Debug.Log("Connection to steam established!\nUsername: " + SteamFriends.GetPersonaName() + "\nSteamID: " + SteamUser.GetSteamID());
        }
        else
        {
            Debug.Log("Could not connect to steam properly");
        }
    }
}
