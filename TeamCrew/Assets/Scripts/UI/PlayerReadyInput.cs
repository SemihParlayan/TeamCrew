using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerReadyInput : MonoBehaviour 
{
    public string player;
    public bool ready;

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
        }
        else
        {
            button.image.sprite = button.spriteState.disabledSprite;
        }
    }
}
