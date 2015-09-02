using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class XboxPs4Switcher : MonoBehaviour 
{
    /// <summary>
    /// Variables to set correct mainmenu button sprites depending if played with Xbox or PS4 controllers
    /// </summary>
    public PlayerReadyInput readyOneReference;
    public PlayerReadyInput readyTwoReference;
    public Sprite[] readyOneSprites;
    public Sprite[] readyTwoSprites;



    /// <summary>
    /// Variables to set correct tutorial bubble animations depending if played with Xbox or PS4 controllers
    /// </summary>
    public Animator bubbleOneReference;
    public Animator bubbleTwoReference;
    public RuntimeAnimatorController xboxHeaveController;
    public RuntimeAnimatorController ps4HeaveController;

	void Start () 
	{
        //Set correct bubble animations for PS4 and Xbox
        if (GameManager.Xbox)
        {
            bubbleOneReference.runtimeAnimatorController = bubbleTwoReference.runtimeAnimatorController = xboxHeaveController;
        }
        else if (GameManager.PS4)
        {
            bubbleOneReference.runtimeAnimatorController = bubbleTwoReference.runtimeAnimatorController = ps4HeaveController;
        }

        //Set correct main menu sprites for PS4 and Xbox
        SetMainMenuSprites();
	}

    void SetMainMenuSprites()
    {
        int startIndex = 0;

        if (GameManager.Xbox)
        {
            startIndex = 0;
        }
        else if (GameManager.PS4)
        {
            startIndex = 4;
        }

        for (int i = 0; i < 4; i++)
        {
            readyOneReference.sprites.Add(readyOneSprites[startIndex + i]);
            readyTwoReference.sprites.Add(readyTwoSprites[startIndex + i]);
        }
    }
	void Update () 
	{
	
	}
}
