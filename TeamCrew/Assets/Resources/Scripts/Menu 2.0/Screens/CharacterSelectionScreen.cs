using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterSelectionScreen : M_Screen 
{
    //Components
    public M_Screen continueScreen;

    //Data
    private bool[] joinArray = new bool[4];
    private bool[] readyArray = new bool[4];
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


    void Start()
    {
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
    }
    public void ContinueToModeSelection()
    {
        if (!CanContinue || continueScreen == null)
            return;

        gameManager.frogsReady = readyArray;

        SwitchScreen(continueScreen);
    }
}
