using UnityEngine;
using System.Collections;

public class DailyMountainScreen : M_Screen 
{
	//publics
    public GameModes gameModes;
    public PoffMountain poff;

	//privates
    private GameManager gameManager;
    private int dailySeed = 50;


	//Unity methods
    void Awake()
    {
        Random.seed = dailySeed;
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }
	//public methods
    public override void OnSwitchedTo()
    {
        base.OnSwitchedTo();
        //Set gamemode to random daily gamemode
        GameMode dailyMode = gameModes.GetRandomDailyGameMode();
        if (dailyMode == null)
            return;

        GameManager.CurrentGameMode = dailyMode;

        //Save previous seed
        //int previousSeed = Random.seed;
        //Random.seed = dailySeed;

        //Lock parallaxes
        gameManager.LockParallaxes(true);

        //Set which frogs are ready
        bool[] frogsReady = new bool[4] { true, false, false, false };
        gameManager.frogsReady = frogsReady;

        //Generate a new mountain with the daily mode
        poff.SetMenuMountainState(false, 0f);
        poff.PoffRepeating(dailySeed);

        //Create new frogs
        gameManager.CreateNewFrogs();
        gameManager.DestroyTopFrog();

        //Reset seed
        //Random.seed = previousSeed;
    }
    public override void OnSwitchedFrom()
    {
        base.OnSwitchedFrom();

        //Unlock parallaxes
        gameManager.LockParallaxes(false);

        if (M_ScreenManager.GetCurrentScreen() is StartScreen)
        {
            //Activate menu mountain
            poff.SetMenuMountainState(true, 0f);

            //Remove old frogs
            gameManager.DestroyFrogs();

            //Set which frogs are ready
            bool[] frogsReady = new bool[4] { false, false, false, false };
            gameManager.frogsReady = frogsReady;
            gameManager.DestroyTopFrog();

            //Destroy daily mountain
            gameManager.DestroyCurrentLevel(true);
        }
    }

	//private methods
}
