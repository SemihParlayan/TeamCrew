using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Timer
{
    public int minutes { get { return (Mathf.FloorToInt(milliSeconds / 100) / 60);} }
    public int seconds { get { return Mathf.FloorToInt(milliSeconds / 100) % 60; } }
    public int milliSeconds { get { return Mathf.FloorToInt(time * 100); } }

    public float time;

    public Timer()
    {

    }
    public Timer(int offsetInMilliSeconds)
    {
        this.time = (float)offsetInMilliSeconds / 100f;
    }
    public void Increment()
    {
        time += Time.deltaTime;
    }
    public void Reset()
    {
        time = 0;
    }
    public string GetTimeString()
    {
        string text = (minutes < 10) ? "0" + minutes : minutes.ToString();
        text += ":" + ((seconds < 10) ? "0" + seconds : seconds.ToString());

        int newMilliseconds = milliSeconds % 100;
        text += ":" + ((newMilliseconds < 10) ? "0" + newMilliseconds : newMilliseconds.ToString());

        return text;
    }

    public bool HasBetterTimeThan(Timer other)
    {
        return this.milliSeconds < other.milliSeconds;
    }
}
public class DailyMountainGameMode : MonoBehaviour 
{
	//publics
    public Text currentTimeText;
    public Timer timer;

	//privates
    private GameManager gameManager;

	//Unity methods
    void Awake()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        currentTimeText.gameObject.SetActive(false);
    }
    void Start()
    {
        timer = new Timer();
    }
	void Update () 
	{
        if (gameManager == null || !gameManager.isInDailyMountain)
            return;

        if (!gameManager.tutorialComplete)
            return;


        timer.Increment();



        currentTimeText.text = timer.GetTimeString();
	}

	//public methods
    public void OnPlayGame()
    {
        timer.Reset();
        currentTimeText.gameObject.SetActive(true);
        currentTimeText.text = "00:00:00";
    }
    public void OnExplosion()
    {
        currentTimeText.gameObject.SetActive(false);
    }
	//private methods
}
