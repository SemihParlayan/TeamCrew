using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour 
{
    public Transform UIParent;
    public Button singleplayerButton;
    public Button multiplayerButton;
    public Image playerOneReady;
    public Image playerTwoReady;
    public Image goImage;

    public List<Sprite> go_sprites = new List<Sprite>();
    private int goIndex;

    private Animator anim;
    private int selectedButton = 1;

	void Start () 
    {
        if (singleplayerButton == null || multiplayerButton == null)
            Debug.LogError("Attach both singleplayer and multiplayer button to MainMenu.cs!");

        anim = GetComponent<Animator>();

        selectedButton = 2;
        multiplayerButton.Select();
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
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Default"))
            {
                if (selectedButton == 1)
                {
                    multiplayerButton.onClick.Invoke();
                }
                else if (selectedButton == 2)
                {
                    multiplayerButton.onClick.Invoke();
                    //gameObject.SetActive(false);
                }

                anim.SetTrigger("SignsOut");
            }
        }

        if (goImage.gameObject.activeInHierarchy)
        {
            goImage.rectTransform.localScale += new Vector3(0.02f, 0.02f, 0.02f);
            goTimer += Time.deltaTime;

            if (goTimer >= 1)
            {
                goTimer = 0;
                goIndex++;

                if (goIndex < go_sprites.Count)
                {
                    goImage.rectTransform.localScale = Vector3.zero;
                    goImage.sprite = go_sprites[goIndex];
                    
                }
                else
                {
                    goReady = true;
                    goImage.gameObject.SetActive(false);
                    playerOneReady.gameObject.SetActive(false);
                    playerTwoReady.gameObject.SetActive(false);
                }
            }
        }
	}
    private float goTimer;
    public bool goReady;

    public void StartGoImage()
    {
        if (goImage.gameObject.activeInHierarchy || goReady)
            return;
        goIndex = 0;
        goImage.sprite = go_sprites[0];
        goImage.gameObject.SetActive(true);
    }
    public void DisableUI()
    {
        UIParent.gameObject.SetActive(false);
    }
    public void EnableUI()
    {
        Invoke("EnableUIPrivate", 3f);
    }
    private void EnableUIPrivate()
    {
        UIParent.gameObject.SetActive(true);
        anim.SetTrigger("SignsIn");
    }
}
