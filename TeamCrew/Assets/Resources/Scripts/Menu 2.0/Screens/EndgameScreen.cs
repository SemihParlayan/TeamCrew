using UnityEngine;
using System.Collections;

public class EndgameScreen : M_Screen
{
    //Data
    public KOTH koth;
    public Fireworks fireWorks;
    public SpriteRenderer whiteFade;
    public EndgameMenuScreen endgameMenuScreen;
    public float danceTime = 5f;
    public float fadeSpeed = 0.5f;
    private bool fade;
    private bool decreaseTime;
    private ScreenMovementProperties endGameCameraProperties;
    private GameManager gamemanager;

    protected override void OnAwake()
    {
        base.OnAwake();
        gamemanager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }
    protected override void OnUpdate()
    {
        base.OnUpdate();
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

        if (decreaseTime)
        {
            Time.timeScale = Mathf.MoveTowards(Time.timeScale, 0.5f, Time.deltaTime);
        }
    }

    public void OnEnter(Vector3 topMountainPosition, int victoryFrogPlayer = -1)
    {
        //Set explosionSettings
        movementProperties.cameraLocation.position = topMountainPosition + new Vector3(0, 3, 0);
        endgameMenuScreen.movementProperties.cameraLocation.position = movementProperties.cameraLocation.position;

        endGameCameraProperties = movementProperties;

        movementProperties.cameraLocation.position = topMountainPosition + new Vector3(0, -1, 0);
        movementProperties.zoom = 12.0f;

        if (!koth.enabled)
            Invoke("OnExplosion", 1f);
        else
        {
            Invoke("OnExplosion", 3.0f);
            decreaseTime = true;
            StartCoroutine(LastGripScore(victoryFrogPlayer, 0.5f));
        }
    }
    private IEnumerator LastGripScore(int frogNumber, float time)
    {
        yield return new WaitForSeconds(time);
        koth.IncreaseScore(frogNumber, 200, true);
    }
    private void OnExplosion()
    {
        movementProperties = endGameCameraProperties;
        decreaseTime = false;
        Time.timeScale = 1.0f;

        //Boom
        whiteFade.gameObject.SetActive(true);
        fireWorks.ExplodeBig();


        Invoke("Fade", 0.5f);

        gamemanager.SpawnHangingFrogs();
        koth.DisableKeepers();


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

        gamemanager.SetInactivityState(false, 15f);
    }
}
