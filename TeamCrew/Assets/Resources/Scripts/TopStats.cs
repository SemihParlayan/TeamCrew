using UnityEngine;
using System.Collections;

public class TopStats : MonoBehaviour {
    private GameManager game_manager;
    Vector4 deaths;
    Vector4 placements;

	// Use this for initialization
	void Start () {
        game_manager = GetComponent<GameManager>();
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void showStats()
    {
        deaths = game_manager.GetFrogDeathCount();
        placements = game_manager.GetEndPlacements();

    }
}
