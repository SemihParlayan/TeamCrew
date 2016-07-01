using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DailyEndgameScreen : M_Screen 
{
	//publics
    public DailyMountainScreen dailyScreen;
    public Fireworks fireWorks;
    public SpriteRenderer whiteFade;
    public Text feedbackText;
    public Text diffTimeText;
    public Text timeText;
    public DailyMountainGameMode dailyGamemode;
    [HideInInspector]
    public bool canContinueToStats;

    [Header("Feedback info for ending")]
    public string failedText = "Try again";
    public string succeedText = "Good job";
    public Color failedColor = Color.red;
    public Color succedColor = Color.green;

	//privates
    private bool fade;
    private float fadeSpeed = 0.5f;
    private bool showTimeText;

	//Unity methods
    protected override void OnAwake()
    {
        base.OnAwake();
        feedbackText.gameObject.SetActive(false);
        diffTimeText.gameObject.SetActive(false);
        timeText.gameObject.SetActive(false);
    }
    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (fade)
        {
            Color c = whiteFade.color;
            c.a = Mathf.MoveTowards(c.a, 0, Time.deltaTime * fadeSpeed);
            whiteFade.color = c;

            if (c.a <= 0.05f)
            {
                whiteFade.gameObject.SetActive(false);
                fade = false;

                FadeComplete();
            }
        }
    }
    

	//public methods
    public void OnEnter(Vector3 topMountainPosition)
    {
        movementProperties.cameraLocation.position = topMountainPosition + new Vector3(0, 2, 0);
        movementProperties.zoom = 6.0f;

        Invoke("OnExplosion", 1f);
    }
    public void ContinueToStats()
    {
        if (canContinueToStats)
        {
            GameManager gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

            gameManager.DestroyFrogs();
            gameManager.ResetGameVariables();
            gameManager.ResetWinVariable();

            M_ScreenManager.SetActive(true);
            M_ScreenManager.SwitchScreen(dailyScreen);
        }
    }

    public override void OnSwitchedTo()
    {
        base.OnSwitchedTo();

        GameObject.FindWithTag("GameManager").GetComponent<GameManager>().SetInactivityState(false, 15f);

        canContinueToStats = false;
        bool sentForUpload = dailyScreen.UploadHighscoreToLeaderboard(dailyGamemode.timer, OnUploadCompleteMethod, OnUploadFailedMethod);

        timeText.text = "Your time: " + dailyGamemode.timer.GetTimeString();
        showTimeText = false;
        if (!sentForUpload)
        {
            canContinueToStats = true;

            //Feedback failed
            feedbackText.text = failedText;

            //Does player have a previous score
            if (dailyScreen.clientEntry != null)
            {
                Timer clientTimer = dailyScreen.clientEntry.timer;
                Timer currentTimer = dailyGamemode.timer;

                //Failed with this much +time
                if (clientTimer.HasBetterTimeThan(currentTimer))
                {
                    Timer diff = new Timer(currentTimer.milliSeconds - clientTimer.milliSeconds);
                    diffTimeText.text = "+" + diff.GetTimeString();
                    diffTimeText.color = failedColor;

                    showTimeText = true;
                }
            }
        }
        else
        {
            Debug.Log("Highscore sent to leaderboard, waiting for response");

            //Feedback success
            feedbackText.text = succeedText;

            //Does player have a previous score
            if (dailyScreen.clientEntry != null)
            {
                Timer clientTimer = dailyScreen.clientEntry.timer;
                Timer currentTimer = dailyGamemode.timer;

                //Succeded with this much -time
                if (currentTimer.HasBetterTimeThan(clientTimer))
                {
                    Timer diff = new Timer(clientTimer.milliSeconds - currentTimer.milliSeconds);
                    diffTimeText.text = "-" + diff.GetTimeString();
                    diffTimeText.color = succedColor;

                    showTimeText = true;
                }
            }
        }
    }
    public override void OnSwitchedFrom()
    {
        base.OnSwitchedFrom();

        timeText.gameObject.SetActive(false);
        feedbackText.gameObject.SetActive(false);
        diffTimeText.gameObject.SetActive(false);
        whiteFade.color = Color.white;
        whiteFade.gameObject.SetActive(false);
    }


	//private methods
    private void OnUploadCompleteMethod()
    {
        canContinueToStats = true;
        Debug.Log("Upload complete");
    }
    private void OnUploadFailedMethod()
    {
        canContinueToStats = true;
        Debug.Log("Upload failed");
    }
    private void OnExplosion()
    {
        whiteFade.gameObject.SetActive(true);
        fireWorks.ExplodeBig();
        Invoke("Fade", 1f);
        GameObject.FindObjectOfType<GameManager>().SpawnHangingFrogs();

        dailyGamemode.OnExplosion();
        dailyScreen.previousTimeObject.gameObject.SetActive(false);
    }
    private void FadeComplete()
    {
        feedbackText.gameObject.SetActive(true);
        timeText.gameObject.SetActive(true);
        if (showTimeText)
        {
            diffTimeText.gameObject.SetActive(true);
        }
    }
    private void Fade()
    {
        fade = true;
    }
}
