using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;


public class Respawn : MonoBehaviour 
{
    //Respawn scripts
    public List<PlayerRespawn> respawnScripts = new List<PlayerRespawn>();
    [HideInInspector]
    public ConnectFrogs connectFrogs;

    //Components
    private Camera cam;
    private StoneDisabler stoneDisabler;

    //private CameraFollow follow; // never used
    private AudioSource screamSource;
    [HideInInspector]
    public BandageManager bandageManager;
    public InactivityController inactivityController;

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
        inactivityController = GetComponent<GameManager>().inactivityController;

        for (int i = 0; i < respawnScripts.Count; i++)
        {
            respawnScripts[i].timer = respawnTime;
        }
	}


	void Update () 
    {
        for (int i = 0; i < respawnScripts.Count; i++)
        {
            PlayerRespawn script = respawnScripts[i];
            script.UpdateRespawn(GetSpawnPosition(script).x);

            if (script.AllowRespawn(respawnTime))
            {
                GameManager.players[i] = RespawnPlayer(script);
            }
        }

        minHeight = cam.transform.position.y - cam.orthographicSize;
        for (int i = 0; i < respawnScripts.Count; i++)
        {
            PlayerRespawn script = respawnScripts[i];

            if (GameManager.players[i] != null)
            {
                if (GameManager.players[i].position.y < minHeight - 2.5f && script.allowRespawn)
                {
                    script.DisableRespawn(5f);
                    script.Respawning = true;
                    script.deathPositionX = GameManager.players[i].position.x;
                    script.deathCount++;
                    Destroy(GameManager.players[i].parent.gameObject);
                    GameManager.players[i] = null;
                    bandageManager.PlayerRespawned(i);

                    if (!screamSource.isPlaying)
                    {
                        screamSource.pitch = Random.Range(0.8f, 1.0f);
                        screamSource.Play();
                    }
                }
            }
            else
            {
                script.Respawning = true;
            }
        }
	}
    public void ResetRespawns()
    {
        for (int i = 0; i < respawnScripts.Count; i++)
        {
            respawnScripts[i].timer = respawnTime;
            respawnScripts[i].Respawning = false;
        }
    }
    Transform RespawnPlayer(PlayerRespawn player)
    {
        Vector3 pos = cam.ScreenToWorldPoint(player.arrow.rectTransform.position); pos.z = 0; pos.y -= 10;
        pos.y += GetSpawnOffset(player);

        Transform t = connectFrogs.SpawnFrog(player.prefab, pos, false);
        Transform body = t.FindChild("body");
        Rigidbody2D b = body.GetComponent<Rigidbody2D>();
        b.isKinematic = false;
        b.AddForce(Vector2.up * 650000);

        stoneDisabler.DisableStoneAt(pos);

        return body;
    }
    public void ResetDeathcount()
    {
        for (int i = 0; i < respawnScripts.Count; i++)
        {
            respawnScripts[i].deathCount = 0;
        }
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
    float GetSpawnOffset(PlayerRespawn player)
    {
        float camY = cam.ScreenToWorldPoint(player.arrow.rectTransform.position).y;
        Transform bottomFrog = GameManager.GetBottomFrog();

        if (bottomFrog == null)
            return 10f;

        float bottomY = bottomFrog.position.y;

        float offset = bottomY - camY;
        offset -= 2f;

        if (offset < 1f)
        {
            offset = 1f;
        }

        return offset;
    }

    void playBoing()
    {
        //play boing sound
    }

}
