using UnityEngine;
using System.Collections;

public class Respawn : MonoBehaviour 
{
    public float respawnTime = 2f;

    private Transform playerOne;
    private Transform playerTwo;
    private Camera cam;
    private CameraFollow follow;

	void Start () 
    {
        cam = Camera.main;
        follow = cam.transform.GetComponent<CameraFollow>();
        playerOne = GameObject.FindWithTag("PlayerOne").transform;
        playerTwo = GameObject.FindWithTag("PlayerTwo").transform;
	}
	
	void Update () 
    {
        float minHeight = cam.transform.position.y - cam.orthographicSize;

        if (playerOne.position.y < minHeight)
        {
            follow.SetAbsoluteZoom(true);
            Invoke("RespawnPlayerOne", respawnTime);
        }

        if (playerTwo.position.y < minHeight)
        {
            follow.SetAbsoluteZoom(true);
            Invoke("RespawnPlayerTwo", respawnTime);
        }
	}

    void RespawnPlayerOne()
    {
        Rigidbody2D body = playerOne.GetComponent<Rigidbody2D>();
        if (body.isKinematic)
            return;

        body.isKinematic = true;

        playerOne.position = GetSpawnPosition();

        follow.SetAbsoluteZoom(false);
    }
    void RespawnPlayerTwo()
    {
        Rigidbody2D body = playerTwo.GetComponent<Rigidbody2D>();
        if (body.isKinematic)
            return;

        body.isKinematic = true;

        playerTwo.position = GetSpawnPosition();

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
                float minDistance = Vector2.Distance(grips[minIndex].transform.position, cam.transform.position);
                float distance = Vector2.Distance(grips[i].transform.position, cam.transform.position);

                if (distance < minDistance)
                {
                    minIndex = i;
                }
            }

            return grips[minIndex].transform.position;
        }

        return cam.transform.position;
    }
}
