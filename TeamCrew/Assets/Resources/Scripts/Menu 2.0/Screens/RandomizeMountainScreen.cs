using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomizeMountainScreen : M_Screen 
{
	//References
    public GameScreen gameScreen;
    public GameObject menuMountain;
    private LevelGeneration generator;

	//Data
    private bool waitForSlotMachine;

	//Components



    protected override void OnStart()
    {
        base.OnStart();
        generator = GameObject.FindWithTag("GameManager").GetComponent<LevelGeneration>();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (waitForSlotMachine)
        {
            if (generator.lockComplete)
            {
                waitForSlotMachine = false;

                //Start game
                M_ScreenManager.SwitchScreen(gameScreen);
                GameObject.FindWithTag("GameManager").GetComponent<GameManager>().CreateNewFrogs();
            }
        }
    }

    public override void OnSwitchedTo()
    {
        base.OnSwitchedTo();

        menuMountain.SetActive(false);
        generator.GenerateMountainSlotmachineStyle();
        waitForSlotMachine = true;
    }
}
