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
    }

    void Update()
    {
        ready = (Input.GetButton(player + "GL") || Input.GetButton(player + "GR"));
        if (ready)
        {
            button.image.sprite = button.spriteState.pressedSprite;
            timer += Time.deltaTime;

            if (timer >= 5)
            {
                singlePlayerReady = true;
            }
        }
        else
        {
            timer = 0;
            button.image.sprite = button.spriteState.disabledSprite;
        }
    }
}
