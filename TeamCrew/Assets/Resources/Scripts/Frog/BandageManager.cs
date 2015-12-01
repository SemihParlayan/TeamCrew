using UnityEngine;
using System.Collections;

public class BandageManager : MonoBehaviour 
{
    public Bandage[] bandages = new Bandage[4];

	void Start () 
    {
        ResetBandages();

        Respawn respawn = GetComponent<Respawn>();

        Debug.Log("Count: " + respawn.respawnScripts.Count);
        for (int i = 0; i < respawn.respawnScripts.Count; i++)
        {
            Transform body = respawn.respawnScripts[i].prefab.FindChild("body");
            Bandage b = body.GetComponent<Bandage>();
            bandages[i] = b;
        }
	}

    public void GameStart()
    {
        
    }

    public void PlayerRespawned(int player)
    {
        if (bandages[player] != null)
            bandages[player].AddBandage(2);
    }
    public void ResetBandages()
    {
        for (int i = 0; i < bandages.Length; i++)
        {
            if (bandages[i] != null)
            {
                bandages[i].ResetBandages();
            }
        }
    }

    void OnApplicationQuit()
    {
        ResetBandages();
    }
}
