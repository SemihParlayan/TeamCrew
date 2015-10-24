﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CharacterSelectionScreen : M_Screen 
{
    //Components
    public M_Screen continueScreen;

    //[HideInInspector]
    public List<Transform> availableFrogs = new List<Transform>();
    public List<FrogPrototype> prefabFrogs = new List<FrogPrototype>();

    //Data
    public bool[] joinArray = new bool[4];
    public bool[] readyArray = new bool[4];
    public bool CanContinue
    {
        get
        {
            int joinCount = 0;
            int readyCount = 0;
            for (int i = 0; i < readyArray.Length; i++)
            {
                if(joinArray[i])
                {
                    joinCount++;
                }
                if (readyArray[i])
                {
                    readyCount++;
                }
            }


            if (joinCount == 0 && readyCount == 0)
                return false;

            return readyCount == joinCount;
        }
    }

    //References
    private GameManager gameManager;

    protected override void OnAwake()
    {
        base.OnAwake();
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    //Methods
    public void P1Join()
    {
        joinArray[0] = !joinArray[0];
    }
    public void P2Join()
    {
        joinArray[1] = !joinArray[1];
    }
    public void P3Join()
    {
        joinArray[2] = !joinArray[2];
    }
    public void P4Join()
    {
        joinArray[3] = !joinArray[3];
    }
    public void P1Ready()
    {
        readyArray[0] = !readyArray[0];
    }
    public void P2Ready()
    {
        readyArray[1] = !readyArray[1];
    }
    public void P3Ready()
    {
        readyArray[2] = !readyArray[2];
    }
    public void P4Ready()
    {
        readyArray[3] = !readyArray[3];
    }

    public void ResetPlayers()
    {
        for (int i = 0; i < joinArray.Length; i++)
        {
            joinArray[i] = false;
            readyArray[i] = false;
        }

        gameManager.frogsReady = readyArray;

        for (int i = 0; i < availableFrogs.Count; i++)
        {
            availableFrogs[i].gameObject.SetActive(false);
        }
    }
    public void ContinueToModeSelection()
    {
        if (!CanContinue || continueScreen == null)
            return;

        gameManager.frogsReady = readyArray;

        SwitchScreen(continueScreen);
    }

    public Transform GetFrog(ref int index, int indexDirection)
    {
        int startIndex = index;
        index += indexDirection;

        while(true)
        {
            if (index == startIndex)
            {
                for (int i = 0; i < availableFrogs.Count; i++)
                {
                    if (availableFrogs[i].gameObject.activeInHierarchy)
                    {
                        continue;
                    }

                    index = i;
                    availableFrogs[i].gameObject.SetActive(true);
                    return availableFrogs[i];
                }
            }

            //Loop around edges when selecting frog
            if (index < 0)
            {
                index = availableFrogs.Count - 1;
            }
            else if (index > availableFrogs.Count - 1)
            {
                index = 0;
            }

            if (availableFrogs[index].gameObject.activeInHierarchy)
            {
                index += indexDirection;
                continue;
            }
            else //Disable lastFrog and activate new frog
            {
                availableFrogs[startIndex].gameObject.SetActive(false);
                availableFrogs[index].gameObject.SetActive(true);
                return availableFrogs[index];
            }
        }
    }

    public override void OnSwitchedFrom()
    {
        base.OnSwitchedFrom();

        SelectedFrog[] selectedFrogs = transform.GetComponentsInChildren<SelectedFrog>();
        for (int i = 0; i < selectedFrogs.Length; i++)
        {
            Respawn respawn = gameManager.transform.GetComponent<Respawn>();

            if (selectedFrogs[i].selectedFrog == null)
            {
                int testIndex = 0;
                GetFrog(ref testIndex, 1);

                FrogPrototype frog = prefabFrogs[testIndex];
                TopFrog topFrog = frog.topPrefab.FindChild("body").GetComponent<TopFrog>();
                topFrog.player = frog.player = "P" + (i + 1).ToString();

                respawn.respawnScripts[i].prefab = frog.transform.parent;
                respawn.respawnScripts[i].arrow.color = frog.respawnArrowColor;
                continue;
            }

            FrogPrototype frogScript = prefabFrogs[selectedFrogs[i].index];
            TopFrog topfrogScript = frogScript.topPrefab.FindChild("body").GetComponent<TopFrog>();
            topfrogScript.player = frogScript.player = "P" + (i + 1).ToString();

            respawn.respawnScripts[i].prefab = frogScript.transform.parent;
            respawn.respawnScripts[i].arrow.color = frogScript.respawnArrowColor;
        }


        for (int i = 0; i < availableFrogs.Count; i++)
        {
            DestroyImmediate(availableFrogs[i].gameObject);
        }
        availableFrogs.Clear();
    }
    public override void OnSwitchedTo()
    {
        base.OnSwitchedTo();
        ResetPlayers();

        for (int i = 0; i < prefabFrogs.Count; i++)
        {
            prefabFrogs[i].player = string.Empty;
            Transform t = Instantiate(prefabFrogs[i].transform.parent, Vector3.zero, Quaternion.identity) as Transform;
            t.gameObject.SetActive(false);
            availableFrogs.Add(t);
        }
    }
}