using UnityEngine;
using System.Collections;

public class TutorialBubbles : MonoBehaviour 
{
    public GameObject bubble, bubbleTwo;
    private FrogPrototype frog, frogTwo;
    private float startY;
	void Update ()
    {
        //Aquire first frog
        if (frog == null)
        {
            if (GameManager.playerOne)
            {
                startY = GameManager.playerOne.position.y;
                frog = GameManager.playerOne.GetComponent<FrogPrototype>();
                Debug.DrawRay(new Vector3(GameManager.playerOne.position.x, startY + 5), Vector3.right);
            }
        }


        //Aquire second frog
        if (frogTwo == null)
        {
            if (GameManager.playerTwo)
            {
                startY = GameManager.playerTwo.position.y;
                frogTwo = GameManager.playerTwo.GetComponent<FrogPrototype>();
            }
        }

        if (frog != null)
            ActivateBubble(bubble, frog, 2);
        if (frogTwo != null)
            ActivateBubble(bubbleTwo, frogTwo, -2);
	}

    void ActivateBubble(GameObject b, FrogPrototype f, float xOffset)
    {
        if (f.leftGripScript.isOnGrip || f.rightGripScript.isOnGrip)
        {
            if (f.transform.position.y < startY + 5)
            {
                b.SetActive(true);
                b.transform.position = f.transform.position - new Vector3(xOffset, 1.7f);
            }
        }
        else
        {
            b.SetActive(false);
        }
    }

    public void Remove()
    {
        Destroy(bubble);
        Destroy(this);
    }
}
