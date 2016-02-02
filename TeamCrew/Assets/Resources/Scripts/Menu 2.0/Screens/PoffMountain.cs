﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public class ParallaxConnection
{
    public Parallax parallax;
    public float yValue;
    public float initialYValue;
}
public class PoffMountain : MonoBehaviour
{
    //References
    public GameObject menuMountain;
    public LevelGeneration generator;
    public ParallaxConnection[] parallaxConnections;

    //Data
    public float poffRepeatRate = 3.0f;
    public bool poffing;


    void Start()
    {
        for (int i = 0; i < parallaxConnections.Length; i++)
        {
            ParallaxConnection c = parallaxConnections[i];
            if (c == null)
                continue;

            c.initialYValue = c.parallax.transform.localPosition.y;
        }
    }

    //Methods
    public void SetPoffState(bool state)
    {
        CancelInvoke("PoffRepeating");
        poffing = state;

        if (poffing)
        {
            InvokeRepeating("PoffRepeating", 0f, poffRepeatRate);
        }
    }
    public void PoffRepeating()
    {
        Poff();

        generator.GenerateFullMountain(true);
    }
    private void Poff()
    {
    }

    private IEnumerator SetMenuMountainStateTime(bool value, float time)
    {
        yield return new WaitForSeconds(time);

        Poff();
        menuMountain.SetActive(value);
    }
    public void SetMenuMountainState(bool value, float time)
    {
        StopAllCoroutines();
        StartCoroutine(SetMenuMountainStateTime(value, time));
    }
}
