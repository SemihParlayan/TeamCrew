using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerReadyInput : MonoBehaviour 
{
    public string player;
    public bool ready;
    public bool singlePlayerReady;
    public float timer;
    public Sprite[] sprites;

    //Components
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        timer = 10f;
    }

    void Update()
    {
        bool leftReady = GameManager.GetGrip(player + "GL");
        bool rightReady = GameManager.GetGrip(player + "GR");

        if (leftReady && rightReady)
        {
            button.image.sprite = sprites[0];
        }
        else if (leftReady)
        {
            button.image.sprite = sprites[1];
        }
        else if (rightReady)
        {
            button.image.sprite = sprites[2];
        }
        else
        {
            button.image.sprite = sprites[3];
        }

        if (leftReady && rightReady)
        {
            ready = true;
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                singlePlayerReady = true;
            }
        }
        else
        {
            ready = false;
            timer = 10f;
        }
    }
}
