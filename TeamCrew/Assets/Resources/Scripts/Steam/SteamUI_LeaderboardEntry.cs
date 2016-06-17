using UnityEngine;
using System.Collections;
using Steamworks;
using System.Timers;

public class SteamUI_LeaderboardEntry : MonoBehaviour 
{
	//publics
    public TextMesh rankText;
    public TextMesh nameText;
    public TextMesh timeText;

	//privates


	//Unity methods
	void Start () 
	{
	
	}
	void Update () 
	{
	
	}

	//public methods
    public void SetInfo(LeaderboardEntry entry)
    {
        rankText.text = entry.globalRank + ".";
        nameText.text = entry.name;

        int totalSeconds = entry.totalSeconds;
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        timeText.text = minutes + ":" + seconds + ":00";
    }

	//private methods
}
