using UnityEngine;
using System.Collections;
using Steamworks;

public class SteamScript : MonoBehaviour 
{
	//publics

	//privates

	//Unity methods
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
    }
}
