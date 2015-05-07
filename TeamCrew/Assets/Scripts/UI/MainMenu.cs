using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour 
{
    //Components
    public Transform UIParent;
    public PlayerReadyInput playerOneReadyInput;
    public PlayerReadyInput playerTwoReadyInput;
    public Image playerOneReady;
    public Image playerTwoReady;

    private Animator anim;

    //Countdown 3-2-1-GO! variables
    [HideInInspector]public bool goReady;
    public List<Sprite> go_sprites = new List<Sprite>();
    public Image goImage;
    private int goIndex;
    private float goTimer;

    void Start()
    {
        anim = GetComponent<Animator>();
    }
	void Update () 
    {
        ///Countdown 3-2-1-GO!
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
        anim.SetTrigger("DisableUI");
    }
    public void EnableUI()
    {
        UIParent.gameObject.SetActive(true);
        anim.SetTrigger("EnableUI");
    }

    public void DisableUIGameObject()
    {
        UIParent.gameObject.SetActive(false);
    }
}
