using UnityEngine;
using System.Collections;

public class BandageManager : MonoBehaviour 
{
    public Bandage playerOne;
    public Bandage playerTwo;

	void Start () 
    {
        ResetBandages();
	}

    public void PlayerOneRespawned()
    {
        playerOne.AddBandage(2);
    }
    public void PlayerTwoRespawned()
    {
        playerTwo.AddBandage(2);
    }

    public void ResetBandages()
    {
        playerOne.ResetBandages();
        playerTwo.ResetBandages();
    }

    void OnApplicationQuit()
    {
        ResetBandages();
    }
}
