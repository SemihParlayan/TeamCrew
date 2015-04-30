using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static Transform playerOne, playerTwo;
    public static float LevelHeight;

    public LevelGeneration generatorScript;
    public Respawn respawnScript;
    public CameraFollow cameraFollowScript;
    public CameraFollowTerrain terrainScript;
    public CameraPan cameraPanScript;
    public MainMenu mainMenuScript;

    private Vector3 cameraDefaultPosition;
    private Transform cameraTransform;
    public bool gameActive;

    public LayerMask mask;

    private Animator tutorialAnimator;
    private bool tutorialSwitched;

	void Start ()
    {
        if (generatorScript == null)
            Debug.LogError("Attach a generator script to GameManager.cs!");

        if (respawnScript == null)
            Debug.LogError("Attach a respawn script to GameManager.cs!");

        if (cameraFollowScript == null)
            Debug.LogError("Attach a camera follow script to GameManager.cs!");

        if (cameraPanScript == null)
            Debug.LogError("Attach a camera pan script to GameManager.cs!");

        if (terrainScript == null)
            Debug.LogError("Attach a camera terrain script to GameManager.cs!");

        if (mainMenuScript == null)
            Debug.LogError("Attach a main menu script to GameManager.cs!");

        cameraTransform = Camera.main.transform;
        cameraDefaultPosition = cameraTransform.transform.position;
	}

    void Update()
    {
        //Check for camera pan complete
        if (cameraPanScript.enabled)
        {
            if (cameraPanScript.Halfway())
            {
                CreateNewFrogs();
            }
            if (cameraPanScript.Complete())
            {
                StartGame();
            }
        }
        else if (!gameActive)
        {
            //Move camera to default
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, cameraDefaultPosition, Time.deltaTime);
        }

        if (!tutorialSwitched && gameActive)
        {
            if (playerOne.GetComponent<FrogPrototype>().ready && playerTwo.GetComponent<FrogPrototype>().ready)
            {
                SwitchTutorial();
            }
        }
    }
    private void SwitchTutorial()
    {
        tutorialSwitched = true;
        tutorialAnimator.SetBool("Switch", true);
    }
    void StartGame()
    {
        gameActive = true;
        cameraFollowScript.enabled = true;
        respawnScript.enabled = true;
        terrainScript.enabled = false;
        playerOne = GameObject.FindWithTag("PlayerOne").transform;
        playerTwo = GameObject.FindWithTag("PlayerTwo").transform;
        tutorialAnimator = GameObject.FindWithTag("TutorialAnimation").GetComponent<Animator>();
    }

    public void ActivateCameraPan()
    {
        cameraPanScript.enabled = true;
    }

    public void Win()
    {
        mainMenuScript.EnableUI();

        cameraFollowScript.enabled = false;
        respawnScript.enabled = false;
        terrainScript.enabled = true;
        gameActive = false;

        //respawnScript.RemoveFrogs();
    }

    private void CreateNewFrogs()
    {
        if (playerOne != null)
            Destroy(playerOne.parent.gameObject);

        if (playerTwo != null)
            Destroy(playerTwo.parent.gameObject);

        playerOne = (Instantiate(respawnScript.playerOnePrefab, generatorScript.GetPlayerOneSpawnPosition(), Quaternion.identity) as Transform).FindChild("body");
        playerTwo = (Instantiate(respawnScript.playerTwoPrefab, generatorScript.GetPlayerTwoSpawnPosition(), Quaternion.identity) as Transform).FindChild("body");

        playerOne.GetComponent<Rigidbody2D>().isKinematic = true;
        playerTwo.GetComponent<Rigidbody2D>().isKinematic = true;
    }
}
