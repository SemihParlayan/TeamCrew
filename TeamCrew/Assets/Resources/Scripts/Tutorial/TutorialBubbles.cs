using UnityEngine;
using System.Collections;

public class TutorialBubbles : MonoBehaviour 
{
    public Animator bubble, bubbleTwo;
    private FrogPrototype frog, frogTwo;
	void Update ()
    {
        //Aquire first frog
        if (frog == null)
        {
            if (GameManager.players[0])
            {
                frog = GameManager.players[0].GetComponent<FrogPrototype>();
            }
        }


        //Aquire second frog
        if (frogTwo == null)
        {
            if (GameManager.players[1])
            {
                frogTwo = GameManager.players[1].GetComponent<FrogPrototype>();
            }
        }

        if (frog != null)
            ActivateBubble(bubble, frog, -2);
        if (frogTwo != null)
            ActivateBubble(bubbleTwo, frogTwo, 2);
	}

    void ActivateBubble(Animator b, FrogPrototype f, float xOffset)
    {
        if (f.leftGripScript.isOnGrip || f.rightGripScript.isOnGrip)
        {
            b.gameObject.SetActive(true);
            b.transform.position = f.transform.position + new Vector3(0, 5.5f);
        }
        else
        {
            b.gameObject.SetActive(false);
        }

        if (b.gameObject.activeInHierarchy)
        {
            if (f.leftGripScript.isOnGrip)
            {
                b.SetBool("left", true);
            }
            else
                b.SetBool("left", false);

            if (f.rightGripScript.isOnGrip)
            {
                b.SetBool("right", true);
            }
            else
                b.SetBool("right", false);
        }
    }

    public void EnableScript()
    {
        this.enabled = true;
    }
    public void DisableScript()
    {
        this.enabled = false;
        bubble.gameObject.SetActive(false);
        bubbleTwo.gameObject.SetActive(false);
    }
}
