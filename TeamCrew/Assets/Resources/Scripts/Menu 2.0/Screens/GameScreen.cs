using UnityEngine;
using System.Collections;

public class GameScreen : M_Screen 
{
    //Data
    public bool started;

    //References
    public GameObject menuMountain;
    private GameManager gameManager;
    private LevelGeneration generator;


    //Unity methods
    void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        generator = GameObject.FindWithTag("GameManager").GetComponent<LevelGeneration>();
    }
    void Update()
    {
        if (started)
            return;

        if (Camera.main.orthographicSize < movementProperties.zoom + 1)
        {
            started = true;

            gameManager.StartGame();
            generator.DeactivateEasyBlock();
            this.enabled = false; 
        }
    }

    //Methods
    public override void OnSwitchedTo()
    {
        base.OnSwitchedTo();

        menuMountain.SetActive(false);
        generator.Generate();
        gameManager.CreateNewFrogs();        
    }
}
