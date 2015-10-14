using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class MenuScreen : MonoBehaviour 
{
    //Data
    public List<Button> buttonList = new List<Button>();
    private int index;
    private float selectionTimer;
    private bool canSelect;

    //Static
    public static float selectionDelay = 0.2f;

    void Update()
    {
        //Stick input
        float horizontalInput = Input.GetAxis("P1HLX");

        if (horizontalInput < -0.8f)
        {
            Select(-1);
        }
        else if (horizontalInput > 0.8f)
        {
            Select(1);
        }

        //Button select input
        bool buttonInput = Input.GetButtonDown("MenuSelectX");

        if (buttonInput)
        {
            Press();
        }

        //Timer
        if (!canSelect)
        {
            selectionTimer += Time.deltaTime;
            if (selectionTimer >= selectionDelay)
            {
                selectionTimer = 0;
                canSelect = true;
            }
        }
    }


    void Press()
    {
        Button b = buttonList[index];
        var pointer = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(b.gameObject, pointer, ExecuteEvents.pointerClickHandler);
    }
    void Select(int indexIncrement)
    {
        if (!canSelect)
            return;

        int newIndex = Mathf.Clamp(index + indexIncrement, 0, buttonList.Count - 1);
        if (newIndex == index)
            return;


        canSelect = false;



        //Deselect
        Deselect(index);




        //Select
        index = newIndex;
        buttonList[index].OnSelect(null);
    }
    void Deselect(int index)
    {
        buttonList[index].OnDeselect(null);
    }
}
