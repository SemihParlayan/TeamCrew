using UnityEngine;
using System.Collections;

public class ReadySetGo : MonoBehaviour 
{
    //Data
    public AudioSource bip;
    public SpriteRenderer[] lights;
    public Sprite[] sprites;
    private GameManager gamemanager;
    private bool started;
    private int index;

    void Start()
    {
        gamemanager = GetComponent<GameManager>();
    }

    public void StartSequence()
    {
        if (started)
            return;

        bip.Play();

        index = 0;
        started = true;
        Invoke("ShowLight", 0f);
        Invoke("ShowLight", 1f);
        Invoke("ShowLight", 2f);
    }

    private void ShowLight()
    {
        lights[index].sprite = sprites[index];
        index++;

        if (index == 3)
        {
            gamemanager.TutorialComplete();
            gamemanager.GetComponent<LevelGeneration>().ActivateFirstBlock();
        }
    }
    public void ResetLights()
    {
        started = false;
        for (int i = 0; i < lights.Length; i++)
        {
            if (lights[i] != null)
                lights[i].sprite = sprites[i + 3];
        }
    }
}
