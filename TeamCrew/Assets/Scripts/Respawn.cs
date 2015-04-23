using UnityEngine;
using System.Collections;

public class Respawn : MonoBehaviour 
{
    public float respawnTime = 2f;
    public Transform playerOnePrefab;
    public Transform playerTwoPrefab;

    private Transform playerOne;
    private Transform playerTwo;
    private Camera cam;
    private CameraFollow follow;

    private AudioSource screamSource;

	void Start () 
    {
        cam = Camera.main;
        follow = cam.transform.GetComponent<CameraFollow>();
        playerOne = GameObject.FindWithTag("PlayerOne").transform;
        playerTwo = GameObject.FindWithTag("PlayerTwo").transform;

        screamSource = GetComponent<AudioSource>();
	}
	
	void Update () 
    {
        if (playerOne == null || playerTwo == null)
            return;

        float minHeight = cam.transform.position.y - cam.orthographicSize;

        if (playerOne.position.y < minHeight)
        {
            follow.SetAbsoluteZoom(true);
            if (!IsInvoking("RespawnPlayerOne"))
            {
                Invoke("RespawnPlayerOne", respawnTime);

                if (!screamSource.isPlaying)
                    screamSource.Play();
            }
                
        }
        else
        {
            CancelInvoke("RespawnPlayerOne");
        }

        if (playerTwo.position.y < minHeight)
        {
            follow.SetAbsoluteZoom(true);
            if (!IsInvoking("RespawnPlayerTwo"))
            {
                Invoke("RespawnPlayerTwo", respawnTime);
                if (!screamSource.isPlaying) 
                    screamSource.Play();
            }
        }
        else
        {
            CancelInvoke("RespawnPlayerTwo");
        }
	}

    void RespawnPlayerOne()
    {
        //Destroy player
        Destroy(playerOne.parent.gameObject);

        //Instantiate a new player
        Transform t = Instantiate(playerOnePrefab, GetSpawnPosition(), Quaternion.identity) as Transform;

        //Set new player start values
        playerOne = t.FindChild("body");
        playerOne.localPosition = GetRandomOffset();

        //Set camera values
        follow.playerOne = playerOne;
        follow.SetAbsoluteZoom(false);
    }
    void RespawnPlayerTwo()
    {
        //Destroy player
        Destroy(playerTwo.parent.gameObject);

        //Instantiate a new player
        Transform t = Instantiate(playerTwoPrefab, GetSpawnPosition(), Quaternion.identity) as Transform;

        //Set new player start values
        playerTwo = t.FindChild("body");
        playerTwo.localPosition = GetRandomOffset();

        //Set camera values
        follow.playerTwo = playerTwo;
        follow.SetAbsoluteZoom(false);
    }

    Vector2 GetSpawnPosition()
    {
        //Vector2 spawnPosition = cam.transform.position;

        GameObject[] grips = GameObject.FindGameObjectsWithTag("Grip");

        if (grips.Length > 0)
        {
            int minIndex = 0;

            for (int i = 1; i < grips.Length; i++)
            {
                float minDistance = Vector2.Distance(grips[minIndex].transform.position, cam.transform.position + new Vector3(0, -2.5f));
                float distance = Vector2.Distance(grips[i].transform.position, cam.transform.position + new Vector3(0, -2.5f));

                if (distance < minDistance)
                {
                    minIndex = i;
                }
            }


            return grips[minIndex].transform.position;
        }

        return cam.transform.position;
    }
    Vector2 GetRandomOffset()
    {
        Vector2[] offsets = new Vector2[3];
        offsets[0] = new Vector2(1.58f, -1.4f);
        offsets[1] = new Vector2(-1.58f, -1.4f);
        offsets[2] = new Vector2(0, -1.8f);

        int i = Random.Range(0, offsets.Length);

        return offsets[i];
    }

    public void RemoveFrogs()
    {
        Destroy(playerOne.parent.gameObject);
        Destroy(playerTwo.parent.gameObject);
    }
}
