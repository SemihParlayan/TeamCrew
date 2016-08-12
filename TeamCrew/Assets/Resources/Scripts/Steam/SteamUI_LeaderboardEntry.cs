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
    public SpriteRenderer highlight;
    public SpriteRenderer avatarRenderer;
    public Color localUserColor;
    public Color friendUserColor;
    public Sprite noAvatarSprite;

    [HideInInspector]
    public LeaderboardEntry entry;

	//privates
    private Vector3 noAvatarScale = new Vector3(0.33f, 0.33f, 0.33f);
    private Vector3 avatarScale = new Vector3(0.88f, 0.88f, 0.88f);

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
            rankText.text = (entry.globalRank != 0) ? entry.globalRank + "." : "?";
            nameText.text = entry.name;
            timeText.text = (entry.timer.time != 0) ? entry.timer.GetTimeString(false, true, true, true) : "??:??.??";

            if(nameText.text.Length > 13)
            {
                nameText.text = nameText.text.Substring(0, 13) + "..";
            }

            //Set avatar
            avatarRenderer.gameObject.SetActive(true);
            if (avatarRenderer != null && entry.avatar != null)
            {
                avatarRenderer.transform.localScale = avatarScale;
                avatarRenderer.sprite = entry.avatar;
            }
            else
            {
                avatarRenderer.transform.localScale = noAvatarScale;
                avatarRenderer.sprite = noAvatarSprite;
            }

            //Set highlights
            if (entry.isFriend)
            {
                highlight.color = friendUserColor;
                highlight.gameObject.SetActive(true);
            }
            else if (entry.isClient)
            {
                highlight.color = localUserColor;
                highlight.gameObject.SetActive(true);
            }
            else
            {
                highlight.gameObject.SetActive(false);
            }
        }
        else
        {
            rankText.text = string.Empty;
            nameText.text = string.Empty;
            timeText.text = string.Empty;
            highlight.gameObject.SetActive(false);

            if (avatarRenderer != null)
            {
                avatarRenderer.sprite = null;
                avatarRenderer.gameObject.SetActive(false);
            }
        }
    }

	//private methods
}
