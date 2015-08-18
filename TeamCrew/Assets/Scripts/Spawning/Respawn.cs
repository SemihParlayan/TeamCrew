using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class Respawn : MonoBehaviour 
{
    //Respawn scripts
    public PlayerRespawn playerOne;
    public PlayerRespawn playerTwo;

    //Components
    private Camera cam;
    private StoneDisabler stoneDisabler;

    //private CameraFollow follow; // never used
    private AudioSource screamSource;
    private BandageManager bandageManager;

    //Camera minHeight
    float minHeight;

    //Data
    public float respawnTime = 3f;

    //put boingsoundhere

	void Start () 
    {
        //Get componenets
        cam = Camera.main;
        screamSource = GetComponent<AudioSource>();
        bandageManager = GetComponent<BandageManager>();
        stoneDisabler = GetComponent<StoneDisabler>();

        playerOne.timer = respawnTime;
        playerTwo.timer = respawnTime;
	}
	
	void Update () 
    {
        //Update timers and move arrows
        playerOne.UpdateRespawn(GetSpawnPosition(playerOne).x);
        playerTwo.UpdateRespawn(GetSpawnPosition(playerTwo).x);
        

        //Respawn player if possible
        if (playerOne.AllowRespawn(respawnTime))
        {
            GameManager.playerOne = RespawnPlayer(playerOne);
        }
        if (playerTwo.AllowRespawn(respawnTime))
        {
            GameManager.playerTwo = RespawnPlayer(playerTwo);
        }

        //Check if player one and player two exists
        minHeight = cam.transform.position.y - cam.orthographicSize;


        //Check player under screen, remove and start respawn
        if (GameManager.playerOne != null)
        {
            if (GameManager.playerOne.position.y < minHeight - 2.5f)
            {
                playerOne.Respawning = true;
                playerOne.deathPositionX = GameManager.playerOne.position.x;
                playerOne.deathCount++;
                Destroy(GameManager.playerOne.parent.gameObject);
                bandageManager.PlayerOneRespawned();

                if (!screamSource.isPlaying)
                {
                    screamSource.pitch = Random.Range(.8f, 1.0f);
                    screamSource.Play();
                }
            }
        }
        else
        {
            playerOne.Respawning = true;
        }

        if (GameManager.playerTwo != null)
        {
            if (GameManager.playerTwo.position.y < minHeight - 2.5f)
            {
                playerTwo.Respawning = true;
                playerTwo.deathPositionX = GameManager.playerTwo.position.x;
                playerTwo.deathCount++;
                Destroy(GameManager.playerTwo.parent.gameObject);
                bandageManager.PlayerTwoRespawned();

                if (!screamSource.isPlaying)
                {
                    screamSource.pitch = Random.Range(.8f, 1.0f);
                    screamSource.Play();
                }
            }
        }
        else
        {
            playerTwo.Respawning = true;
        }
	}
    public void ResetRespawns()
    {
        playerOne.timer = respawnTime;
        playerTwo.timer = respawnTime;

        playerOne.Respawning = false;
        playerTwo.Respawning = false;
    }
    Transform RespawnPlayer(PlayerRespawn player)
    {
        Vector3 pos = cam.ScreenToWorldPoint(player.arrow.rectTransform.position);
        pos.z = 0;
        pos.y -= 3;
        Transform t = Instantiate(player.prefab, pos, Quaternion.identity) as Transform;

        Transform body = t.FindChild("body");
        body.GetComponent<Line>().Remove();

        Rigidbody2D b = body.GetComponent<Rigidbody2D>();
        b.isKinematic = false;
        b.AddForce(Vector2.up * 650000);

        stoneDisabler.DisableStoneAt(pos);

        return body;
    }
    public void ResetDeathcount()
    {
        playerOne.deathCount = 0;
        playerTwo.deathCount = 0;
    }

    Vector2 GetSpawnPosition(PlayerRespawn player)
    {
        GameObject[] grips = GameObject.FindGameObjectsWithTag("Grip");

        if (grips.Length > 0)
        {
            int minIndex = 0;

            for (int i = 1; i < grips.Length; i++)
            {
                Vector3 targetSpawnPosition = cam.transform.position + new Vector3(0, -2.5f);
                targetSpawnPosition.x = player.deathPositionX;

                float minDistance = Vector2.Distance(grips[minIndex].transform.position, targetSpawnPosition);
                float distance = Vector2.Distance(grips[i].transform.position, targetSpawnPosition);

                if (distance < minDistance)
                {
                    minIndex = i;
                }
            }


            Vector3 pos = grips[minIndex].transform.position;
            pos.y = minHeight - 6;
            return pos;
        }

        return cam.transform.position;
    }

    void playBoing()
    {
        //play boing sound
    }

}
