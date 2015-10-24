using UnityEngine;
using System.Collections;

public class EndgameScreen : M_Screen
{
    //Data
    public Fireworks fireWorks;
    public SpriteRenderer whiteFade;
    public EndgameMenuScreen endgameMenuScreen;
    public float danceTime = 5f;
    public float fadeSpeed = 0.5f;
    private bool fade;


    void Update()
    {
        if (fade)
        {
            Color c = whiteFade.color;
            c.a = Mathf.MoveTowards(c.a, 0, Time.deltaTime * fadeSpeed);
            whiteFade.color = c;

            if (c.a <= 0.05f)
            {
                whiteFade.gameObject.SetActive(false);
                fade = false;
            }
        }
    }

    public void OnEnter(Vector3 topMountainPosition)
    {
        //Set explosionSettings
        movementProperties.cameraLocation.position = topMountainPosition + new Vector3(0, 3, 0);
        endgameMenuScreen.movementProperties.cameraLocation.position = movementProperties.cameraLocation.position;
        Invoke("OnExplosion", 1f);
    }
    private void OnExplosion()
    {
        //Boom
        whiteFade.gameObject.SetActive(true);
        fireWorks.ExplodeBig();


        Invoke("Fade", 0.5f);

        GameManager gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        gameManager.DestroyFrogs();
        gameManager.SpawnTopFrog();
        Invoke("OnMenu", danceTime);

    }
    private void Fade()
    {
        fade = true;
    }
    private void OnMenu()
    {
        SwitchScreen(endgameMenuScreen);
    }

    public override void OnSwitchedTo()
    {
        whiteFade.color = Color.white;
        base.OnSwitchedTo();
    }
}
