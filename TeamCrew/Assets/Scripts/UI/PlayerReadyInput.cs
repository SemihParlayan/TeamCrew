using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerReadyInput : MonoBehaviour 
{
    public string player;
    public bool ready;
    public bool singlePlayerReady;
    public float timer;

    //Components
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        timer = 10f;
    }

    void Update()
    {
        ready = (GameManager.GetGrip(player + "GL") || GameManager.GetGrip(player + "GR"));
        if (ready)
        {
            button.image.sprite = button.spriteState.pressedSprite;
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                singlePlayerReady = true;
            }
        }
        else
        {
            timer = 10f;
            button.image.sprite = button.spriteState.disabledSprite;
        }
    }
}
