using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour 
{
    //Components
    public GameObject UIParent;
    public PlayerReadyInput playerOneReadyInput;
    public PlayerReadyInput playerTwoReadyInput;
    public Image playerOneReady;
    public Image playerTwoReady;
    public Animator singleplayerSign;
    public Text singleplayerText;

    public Image exitImage;
    public Sprite[] exitSprites;
    public Text exitText;
    private float exitTimer;

    private Animator anim;
    private GameManager gameManager;
    private AudioSource goSound;

    //Countdown 3-2-1-GO! variables
    [HideInInspector]public bool goReady;
    public Sprite[] goSprites;
    public SpriteRenderer[] goRenderers;
    private int goIndex;
    private float goTimer;
    private bool startGo;
    private bool uiEnabled;

    private MenuMusicController menuMusicController;

    void Start()
    {
        goSound = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        menuMusicController = transform.GetComponentInChildren<MenuMusicController>();
        EnableUI();
    }
	void Update () 
    {
        ///Countdown 3-2-1-GO!
        if (startGo)
        {
            goTimer += Time.deltaTime;

            if (goTimer >= 1)
            {       
                goTimer = 0;
                goIndex++;

                goRenderers[goIndex].sprite = goSprites[goIndex];

                if (goIndex >= 2)
                {
                    goReady = true;
                    startGo = false;
                    playerOneReady.gameObject.SetActive(false);
                    playerTwoReady.gameObject.SetActive(false);
                }

                if (goIndex == 0)
                    goSound.Play();
            }
        }


        ////Single player
        //float moveSpeed = 5f; //Never used
        if (uiEnabled)
        {
            if (playerOneReadyInput.timer < 10)
            {
                singleplayerSign.SetBool("Ready", true);

                string t = "Singleplayer starts in " + Mathf.RoundToInt(playerOneReadyInput.timer).ToString() + "...";
                singleplayerText.text = t;
            }
            else if (playerTwoReadyInput.timer < 10)
            {
                singleplayerSign.SetBool("Ready", true);

                string t = "Singleplayer starts in " + Mathf.RoundToInt(playerTwoReadyInput.timer).ToString() + "...";
                singleplayerText.text = t;
            }
            else
            {
                singleplayerSign.SetBool("Ready", false);
            }
        }
        else
        {
            singleplayerSign.SetBool("Ready", false);
        }

        if (!gameManager.gameActive)
        {
            bool exit = false;
            if (GameManager.Xbox && Input.GetButton("ExitX"))
                exit = true;
            else if (GameManager.PS4 && Input.GetButton("ExitPS"))
                exit = true;

            if (exit)
            {
                exitText.gameObject.SetActive(true);
                exitImage.sprite = exitSprites[1];
                exitTimer -= Time.deltaTime;

                exitText.text = "Quitting in " + Mathf.RoundToInt(exitTimer).ToString() + "...";

                if (exitTimer < 0)
                {
                    Application.Quit();
                }
            }
            else
            {
                exitText.gameObject.SetActive(false);
                exitImage.sprite = exitSprites[0];
                exitTimer = 5f;
            }
        }
	}

    public void StartGoImage(SpriteRenderer[] renderers)
    {
        if (startGo || goReady)
            return;
        goRenderers = renderers;
        startGo = true;
        goIndex = -1;
    }

    public void DisableUI()
    {
        uiEnabled = false;
        anim.SetTrigger("DisableUI");
    }
    public void DisableUIParent()
    {
        playerOneReadyInput.ready = false;
        playerOneReadyInput.singlePlayerReady = false;

        playerTwoReadyInput.ready = false;
        playerTwoReadyInput.singlePlayerReady = false;
        UIParent.SetActive(false);
    }
    public void EnableUI()
    {
        uiEnabled = true;
        Invoke("EnableUIParent", 0.1f);
        anim.SetTrigger("EnableUI");
    }
    private void EnableUIParent()
    {
        UIParent.SetActive(true);
        menuMusicController.Play();
        menuMusicController.ChangeFadeState(FadeState.IN);
    }

    public Animator p1Win;
    public Animator p2Win;
    public Animator p1DeathCounter;
    public Animator p2DeathCounter;
    private int winFrog;
    private int victoryNumber;

    public void StartMenuCycle(int frogNumber, int victoryNumber)
    {
        this.victoryNumber = victoryNumber;
        winFrog = frogNumber;
        Invoke("EnableWinSign", 1.5f);
    }

    void EnableWinSign()
    {
        if (winFrog == 1)
        {
            p1Win.SetTrigger("Activate");
        }
        else
        {
            p2Win.SetTrigger("Activate");
        }
    }
    public void WinSignComplete()
    {
        EnableDeathCounter();
    }

    public Animator victoryTextSign;
    void EnableDeathCounter()
    {
        p1DeathCounter.SetTrigger("Activate");
        p2DeathCounter.SetTrigger("Activate");

        Vector2 deathCount = gameManager.GetFrogDeathCount();
        p1DeathCounter.transform.GetChild(0).GetComponent<Text>().text = deathCount.x.ToString();
        p2DeathCounter.transform.GetChild(0).GetComponent<Text>().text = deathCount.y.ToString();

        string t = "";

        if (victoryNumber <= -3)
            t = "Disgusting victory!";
        else if (victoryNumber == -2)
            t = "Douchefrog victory";
        else if (victoryNumber == -1)
            t = "Bandaged victory";
        else if (victoryNumber == 0)
            t = "Fair victory";
        else if (victoryNumber == 1)
            t = "Beautiful victory";
        else if (victoryNumber == 2)
            t = "Dominant victory";
        else if (victoryNumber >= 3)
            t = "Crushingly dominant victory";
        else
            t = "Victory";

        victoryTextSign.SetTrigger("Activate");
        victoryTextSign.transform.GetChild(0).GetComponent<Text>().text = t;
    }
    public void DeathCounterComplete()
    {
        EnableUI();
    }
}
