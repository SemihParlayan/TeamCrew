using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour 
{
    public Button singleplayerButton;
    public Button multiplayerButton;

    private Animator anim;

    private int selectedButton = 1;

	void Start () 
    {
        //ERROR CHECK RAWR
        anim = GetComponent<Animator>();
	}

	void Update () 
    {
        //Select buttons
        float horizontal = Mathf.RoundToInt(Input.GetAxis("P1HL") + Input.GetAxis("P1HR"));

        if (horizontal > 0)
        {
            selectedButton = 2;
            multiplayerButton.Select();
        }
        else if (horizontal < 0)
        {
            selectedButton = 1;
            singleplayerButton.Select();
        }


        //Click buttons
        if (Input.GetButtonDown("P1GL") || Input.GetButtonDown("P1GR"))
        {
            if (selectedButton == 1)
            {

            }
            else if (selectedButton == 2)
            {
                multiplayerButton.onClick.Invoke();
                //gameObject.SetActive(false);
            }

            anim.SetTrigger("AnimationStart");
        }

	}

    public void SignAnimationComplete()
    {
        anim.SetTrigger("AnimationComplete");
        gameObject.SetActive(false);
    }
}
