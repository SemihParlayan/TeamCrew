using UnityEngine;
using System.Collections;

public class BandageManager : MonoBehaviour 
{
    public Bandage[] bandages = new Bandage[4];

	void Start () 
    {
        ResetBandages();

        Respawn respawn = GetComponent<Respawn>();

        for (int i = 0; i < respawn.respawnScripts.Count; i++)
        {
            Transform body = respawn.respawnScripts[i].prefab.FindChild("body");
            Bandage b = body.GetComponent<Bandage>();
            bandages[i] = b;
        }
	}

    public Bandage GetBandage(int player)
    {
        for (int i = 0; i < bandages.Length; i++)
        {
            FrogPrototype frog = bandages[i].transform.GetComponent<FrogPrototype>();

            if (frog.player == player)
            {
                return bandages[i];
            }
        }

        return null;
    }
    public void PlayerRespawned(int player)
    {
        Bandage b = GetBandage(player);

        if (b != null)
            b.AddBandage(2);
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
