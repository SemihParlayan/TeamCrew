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

    private AudioSource p1ScreamSource;
    private AudioSource p2ScreamSource;

	void Start () 
    {
        cam = Camera.main;
        follow = cam.transform.GetComponent<CameraFollow>();
        playerOne = GameObject.FindWithTag("PlayerOne").transform;
        playerTwo = GameObject.FindWithTag("PlayerTwo").transform;


        SoundMaster playerSoundMaster;

        playerSoundMaster= playerOne.GetComponent<SoundMaster>();
        p1ScreamSource = playerSoundMaster.GetSource(0);

        playerSoundMaster = playerTwo.GetComponent<SoundMaster>();
        p2ScreamSource = playerSoundMaster.GetSource(0);
	}
	
	void Update () 
    {
        float minHeight = cam.transform.position.y - cam.orthographicSize;

        if (playerOne.position.y < minHeight)
        {
            follow.SetAbsoluteZoom(true);
            if (!IsInvoking())
            {
                Invoke("RespawnPlayerOne", respawnTime);
                if (!p1ScreamSource.isPlaying) p1ScreamSource.Play();
            }
                
        }
        else
        {
            CancelInvoke("RespawnPlayerOne");
        }

        if (playerTwo.position.y < minHeight)
        {
            follow.SetAbsoluteZoom(true);
            if (!IsInvoking())
            {
                Invoke("RespawnPlayerTwo", respawnTime);
                if (!p2ScreamSource.isPlaying) p2ScreamSource.Play();
            }
        }
        else
        {
            CancelInvoke("RespawnPlayerTwo");
        }
	}

    void RespawnPlayerOne()
    {
        Rigidbody2D body = playerOne.GetComponent<Rigidbody2D>();

        body.isKinematic = true;

        playerOne.parent.position = GetSpawnPosition();
        playerOne.localPosition = GetRandomOffset();
        playerOne.localRotation = Quaternion.Euler(Vector3.zero);
        playerOne.GetComponent<FrogPrototype>().leftGripScript.ResetGrip();
        playerOne.GetComponent<FrogPrototype>().rightGripScript.ResetGrip();

        Transform leg = playerOne.parent.FindChild("left_leg_lower");
        leg.localPosition = playerOnePrefab.FindChild("left_leg_lower").localPosition;

        Transform upperLeg = playerOne.parent.FindChild("left_leg_upper");
        upperLeg.localPosition = playerOnePrefab.FindChild("left_leg_upper").localPosition;

        leg = playerOne.parent.FindChild("right_leg_lower");
        leg.localPosition = playerOnePrefab.FindChild("right_leg_lower").localPosition;

        upperLeg = playerOne.parent.FindChild("right_leg_upper");
        upperLeg.localPosition = playerOnePrefab.FindChild("right_leg_upper").localPosition;

        follow.SetAbsoluteZoom(false);
    }
    void RespawnPlayerTwo()
    {
        Rigidbody2D body = playerTwo.GetComponent<Rigidbody2D>();

        body.isKinematic = true;

        playerTwo.parent.position = GetSpawnPosition();
        playerTwo.localPosition = GetRandomOffset();
        playerTwo.localRotation = Quaternion.Euler(Vector3.zero);
        playerTwo.GetComponent<FrogPrototype>().leftGripScript.ResetGrip();
        playerTwo.GetComponent<FrogPrototype>().rightGripScript.ResetGrip();

        Transform leg = playerTwo.parent.FindChild("left_leg_lower");
        leg.localPosition = playerTwoPrefab.FindChild("left_leg_lower").localPosition;

        Transform upperLeg = playerTwo.parent.FindChild("left_leg_upper");
        upperLeg.localPosition = playerTwoPrefab.FindChild("left_leg_upper").localPosition;

        leg = playerTwo.parent.FindChild("right_leg_lower");
        leg.localPosition = playerTwoPrefab.FindChild("right_leg_lower").localPosition;

        upperLeg = playerTwo.parent.FindChild("right_leg_upper");
        upperLeg.localPosition = playerTwoPrefab.FindChild("right_leg_upper").localPosition;

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
}
