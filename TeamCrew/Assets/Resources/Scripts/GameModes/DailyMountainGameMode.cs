using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

public class Timer
{
    public int hours { get { return (Mathf.FloorToInt(milliSeconds / 100) / 60) / 60; } }
    public int minutes { get { return ((Mathf.FloorToInt(milliSeconds / 100) / 60) % 60);} }
    public int seconds { get { return Mathf.FloorToInt(milliSeconds / 100) % 60; } }
    public int milliSeconds { get { return Mathf.FloorToInt(time * 100); } }

    public float time;

    public Timer()
    {

    }
    public Timer(float offsetInMilliSeconds)
    {
        this.time = (float)offsetInMilliSeconds / 100f;
    }
    public void Increment()
    {
        time += Time.deltaTime;
    }
    public void Decrement()
    {
        time -= Time.deltaTime;
        if (time < 0)
            time = 0;
    }
    public void Reset()
    {
        time = 0;
    }
    public string GetTimeString(bool inclHour, bool inclMinute, bool inclSecond, bool inclMilliSecond)
    {
        string text = string.Empty;

        //Add hour
        if (inclHour)
        {
            text += (hours < 10 ? "0" + hours : hours.ToString());
        }

        //Add minutes
        if (inclMinute)
        {
            if (inclHour)
                text += ":";

            text += (minutes < 10 ? "0" + minutes : minutes.ToString());
        }

        //Add seconds
        if (inclSecond)
        {
            if (inclHour || inclMinute)
                text += ":";

            text += (seconds < 10 ? "0" + seconds : seconds.ToString());
        }

        //Add milliseconds
        if (inclMilliSecond)
        {
            if (inclHour || inclMinute || inclSecond)
                text += ".";

            int newMilliseconds = milliSeconds % 100;
            text += (newMilliseconds < 10 ? "0" + newMilliseconds : newMilliseconds.ToString());
        }
        return text;
    }

    public bool HasBetterTimeThan(Timer other)
    {
        return this.milliSeconds < other.milliSeconds;
    }

    public void AddSeconds(int amount)
    {
        time += amount;
    }
    public void AddMinutes(int amount)
    {
        time += amount * 60f;
    }
    public void AddHours(int amount)
    {
        time += 3600f * amount;
    }
    public void AddMilliSeconds(int amount)
    {
        time += (float)amount / 100f;
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



        currentTimeText.text = timer.GetTimeString(false, true, true, true);
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
