using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TopStatsInfoBlock : MonoBehaviour {
    public float deathcount=888;
    public GameObject statsholder;
    float flycount;
    public TextMesh death_text;

    // Use this for initialization
    void Start () {
       // death_text = GetComponentInChildren<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ShowBlock()
    {
        statsholder.SetActive(true);
        death_text.text= deathcount.ToString();
    }
    public void HideBlock()
    {
        statsholder.SetActive(false);
    }

}
