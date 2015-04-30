using UnityEngine;
using System.Collections;

public class Respawn : MonoBehaviour 
{
    public float respawnTime = 2f;
    public Transform playerOnePrefab;
    public Transform playerTwoPrefab;

    private Camera cam;
    private CameraFollow follow;

    private AudioSource screamSource;
    float minHeight;

	void Start () 
    {
        cam = Camera.main;
        follow = cam.transform.GetComponent<CameraFollow>();

        screamSource = GetComponent<AudioSource>();
	}
	
	void Update () 
    {
        if (GameManager.playerOne == null || GameManager.playerTwo == null)
            return;

        minHeight = cam.transform.position.y - cam.orthographicSize;

        if (GameManager.playerOne.position.y < minHeight)
        {
            if (!IsInvoking("RespawnPlayerOne"))
            {
                Invoke("RespawnPlayerOne", respawnTime);

                if (!screamSource.isPlaying)
                {
                    screamSource.pitch = Random.Range(.8f, 1.0f);
                    screamSource.Play();
                }
            }
                
        }
        else
        {
            CancelInvoke("RespawnPlayerOne");
        }

        if (GameManager.playerTwo.position.y < minHeight)
        {
            if (!IsInvoking("RespawnPlayerTwo"))
            {
                Invoke("RespawnPlayerTwo", respawnTime);
                if (!screamSource.isPlaying)
                {
                    screamSource.pitch = Random.Range(.4f, .7f);
                    screamSource.Play();
                }
            }
        }
        else
        {
            CancelInvoke("RespawnPlayerTwo");
        }

        if (GameManager.playerOne.position.y < minHeight || GameManager.playerTwo.position.y < minHeight)
        {
            follow.SetAbsoluteZoom(true);
        }
        else
        {
            follow.SetAbsoluteZoom(false);
        }
	}

    void RespawnPlayerOne()
    {
        //Destroy player
        Destroy(GameManager.playerOne.parent.gameObject);

        Transform t = Instantiate(playerOnePrefab, GetSpawnPosition(), Quaternion.identity) as Transform;

        GameManager.playerOne = t.FindChild("body");
        Rigidbody2D b = GameManager.playerOne.GetComponent<Rigidbody2D>();
        b.isKinematic = false;
        b.AddForce(Vector2.up * 750000);
    }
    void RespawnPlayerTwo()
    {
        //Destroy player
        Destroy(GameManager.playerTwo.parent.gameObject);

        Transform t = Instantiate(playerTwoPrefab, GetSpawnPosition(), Quaternion.identity) as Transform;

        GameManager.playerTwo = t.FindChild("body");
        Rigidbody2D b = GameManager.playerTwo.GetComponent<Rigidbody2D>();
        b.isKinematic = false;
        b.AddForce(Vector2.up * 750000);
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


            Vector3 pos = grips[minIndex].transform.position;
            pos.y = minHeight - 6;
            return pos;
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
}
