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
    public Text timeText;
    public DailyMountainGameMode dailyGamemode;

	//privates
    private bool fade;
    private float fadeSpeed = 0.5f;

	//Unity methods
    protected override void OnAwake()
    {
        base.OnAwake();
        feedbackText.gameObject.SetActive(false);
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
        GameManager gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        gameManager.DestroyFrogs();
        gameManager.ResetGameVariables();
        gameManager.ResetWinVariable();

        M_ScreenManager.SetActive(true);
        M_ScreenManager.SwitchScreen(dailyScreen);
    }

    public override void OnSwitchedTo()
    {
        base.OnSwitchedTo();

        GameObject.FindWithTag("GameManager").GetComponent<GameManager>().SetInactivityState(false, 15f);
    }
    public override void OnSwitchedFrom()
    {
        base.OnSwitchedFrom();

        feedbackText.gameObject.SetActive(false);
        timeText.gameObject.SetActive(false);
    }


	//private methods
    private void OnExplosion()
    {
        whiteFade.gameObject.SetActive(true);
        fireWorks.ExplodeBig();
        Invoke("Fade", 0.5f);
        GameObject.FindObjectOfType<GameManager>().SpawnHangingFrogs();

        feedbackText.gameObject.SetActive(true);
        timeText.gameObject.SetActive(true);

        dailyGamemode.OnExplosion();
        //feedbackText.text = dailyGamemode.GetFeedback();
        timeText.text = dailyGamemode.timer.GetTimeString();
    }
    private void Fade()
    {
        fade = true;
    }
}
