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
    public GameObject highlight;

    [HideInInspector]
    public LeaderboardEntry entry;

	//privates


	//Unity methods
	void Start () 
	{
        highlight.gameObject.SetActive(false);
	}
	void Update () 
	{
	
	}

	//public methods
    public void SetInfo(LeaderboardEntry entry)
    {
        this.entry = entry;
        if (entry != null)
        {
            rankText.text = entry.globalRank + ".";
            nameText.text = entry.name;
            timeText.text = entry.timer.GetTimeString();

            highlight.SetActive(entry.isClient);
        }
        else
        {
            rankText.text = string.Empty;
            nameText.text = string.Empty;
            timeText.text = string.Empty;
            highlight.SetActive(false);
        }
    }

	//private methods
}
