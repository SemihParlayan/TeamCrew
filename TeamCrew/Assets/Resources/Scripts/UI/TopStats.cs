using UnityEngine;
using System.Collections;


public class TopStats : MonoBehaviour {
    private GameManager game_manager;
    public GameObject statsParent;
    
    
    //Playerstats[] playerlist = new Playerstats[4];
    public TopStatsInfoBlock[] infoBlocks = new TopStatsInfoBlock[4]; 

	// Use this for initialization
	void Start () {
        game_manager = GetComponent<GameManager>();

       // statsParent.SetActive(false);

    }
	
	// Update is called once per frame
	void Update () {
	
	}
    public void hideStats()
    {
        for (int i = 0; i < game_manager.GetFrogReadyCount(); i++)
        {
            infoBlocks[i].HideBlock();
        }
        statsParent.SetActive(false);

    }
    public void showStats()
    {
        statsParent.SetActive(true);
        Debug.Log("frog ready count: " + game_manager.GetFrogReadyCount());

        for (int i=0;i<game_manager.GetFrogReadyCount(); i++)
        {
            infoBlocks[i].deathcount = getDeathsForPlacement(i);
            infoBlocks[i].ShowBlock();
            
        }
    }

    float getDeathsForPlacement(float p)
    {
       // Debug.Log("hi, p= " + p);
        Vector4 deaths = game_manager.GetFrogDeathCount();
        Vector4 placements = game_manager.GetEndPlacements();

        for(int i=0;i<4;i++)
        {
            if(placements[i]==p)
            {
                //Debug.Log("calculated death count: " + deaths[i]);
                return deaths[i];
            }
 
        }

       // Debug.Log("death count calculator broke! Please send help.");
        return 999;
        
    }
}
