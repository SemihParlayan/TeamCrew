using UnityEngine;
using System.Collections;

public class GameScreen : M_Screen 
{
    //Data
    public bool started;
    private bool replay;

    //References
    public GameObject menuMountain;
    public GameManager gameManager;
    public LevelGeneration generator;


    //Unity methods
    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (started)
            return;

        bool reachedBottom = false;
        if (Camera.main.orthographicSize <= movementProperties.zoom + 0.1f)
        {
            Vector3 cameraPos = Camera.main.transform.position; cameraPos.z = 0;
            Vector3 screenPos = movementProperties.cameraLocation.position; screenPos.z = 0;
            float dist = Vector3.Distance(cameraPos, screenPos);

            if (Vector3.Distance(cameraPos, screenPos) < 0.2f)
            {
                reachedBottom = true;
            }
        }

        //Zoomed in at bottom
        if (reachedBottom)
        {
            started = true;

            this.enabled = false;
            GameObject.FindWithTag("MenuManager").GetComponent<M_ScreenManager>().enabled = false;
            GameObject.FindWithTag("GameManager").GetComponent<TopFrogSpawner>().RemoveFrog();

            if (replay)
            {
                generator.Generate(true);
                generator.SetLevelHeight();
                replay = false;
            }
            gameManager.StartGame();
        }
    }

    //Methods
    public override void OnSwitchedTo()
    {
        base.OnSwitchedTo();

        started = false;

        if (!replay)
        {
            gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
            generator = GameObject.FindWithTag("GameManager").GetComponent<LevelGeneration>();

            generator.Generate();
            generator.SetLevelHeight();
            gameManager.CreateNewFrogs();
        }
        else
        {
            gameManager.CreateNewFrogs();
        }
        menuMountain.gameObject.SetActive(false);
    }
    public void Replay()
    {
        replay = true;
    }
}
