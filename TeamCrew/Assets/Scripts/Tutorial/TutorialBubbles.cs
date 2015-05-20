using UnityEngine;
using System.Collections;

public class TutorialBubbles : MonoBehaviour 
{
    public GameObject bubble, bubbleTwo;
    private FrogPrototype frog, frogTwo;
	void Update ()
    {
        //Aquire first frog
        if (frog == null)
        {
            if (GameManager.playerOne)
            {
                frog = GameManager.playerOne.GetComponent<FrogPrototype>();
            }
        }


        //Aquire second frog
        if (frogTwo == null)
        {
            if (GameManager.playerTwo)
            {
                frogTwo = GameManager.playerTwo.GetComponent<FrogPrototype>();
            }
        }

        if (frog != null)
            ActivateBubble(bubble, frog, -2);
        if (frogTwo != null)
            ActivateBubble(bubbleTwo, frogTwo, 2);
	}

    void ActivateBubble(GameObject b, FrogPrototype f, float xOffset)
    {
        if (f.leftGripScript.isOnGrip || f.rightGripScript.isOnGrip)
        {
            b.SetActive(true);
            b.transform.position = f.transform.position + new Vector3(0, 5.5f);
        }
        else
        {
            b.SetActive(false);
        }
    }

    public void EnableScript()
    {
        this.enabled = true;
    }
    public void DisableScript()
    {
        this.enabled = false;
        bubble.SetActive(false);
        bubbleTwo.SetActive(false);
    }
}
