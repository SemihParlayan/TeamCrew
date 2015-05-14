using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Bandage : MonoBehaviour 
{
    public List<GameObject> bandages = new List<GameObject>();
    private int bandageIndex = 0;

    public void AddBandage(int numberOfBandages)
    {
        for (int i = 0; i < numberOfBandages; i++ )
        {
            if (bandageIndex < bandages.Count)
                bandages[bandageIndex++].SetActive(true);
            else
                break;
        }
    }

    public void ResetBandages()
    {
        bandageIndex = 0;
        for (int i = 0; i < bandages.Count; i++)
        {
            bandages[i].SetActive(false);
        }
    }
}
