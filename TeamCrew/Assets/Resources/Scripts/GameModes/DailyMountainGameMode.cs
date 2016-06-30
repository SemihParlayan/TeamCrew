using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Timer
{
    public int minutes { get { return Mathf.FloorToInt(seconds / 60f);} }
    public int seconds { get { return Mathf.FloorToInt(milliSeconds / 100); } }
    public int milliSeconds { get { return Mathf.FloorToInt(timer * 100); } }

    private float timer;

    public void Increment()
    {
        timer += Time.deltaTime;
    }
    public void Reset()
    {
        timer = 0;
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
    }
    public void OnExplosion()
    {
        currentTimeText.gameObject.SetActive(false);
    }
	//private methods
}
