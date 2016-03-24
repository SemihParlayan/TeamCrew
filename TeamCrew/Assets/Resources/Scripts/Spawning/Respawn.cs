using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;


public class Respawn : MonoBehaviour 
{
    //Respawn scripts
    public bool drawGizmos;
    public AudioSource bounceAudioSource;
    public List<PlayerRespawn> respawnScripts = new List<PlayerRespawn>();
    [HideInInspector]
    public ConnectFrogs connectFrogs;

    //Components
    private Camera cam;

    //private CameraFollow follow; // never used
    private AudioSource screamSource;
    [HideInInspector]
    public BandageManager bandageManager;
    public InactivityController inactivityController;
    private Grip[] mapGrips;

    //Camera minHeight
    float minHeight;

    //Data
    public float respawnTime = 3f;

	void Start () 
    {
        //Get componenets
        cam = Camera.main;
        screamSource = GetComponent<AudioSource>();
        bandageManager = GetComponent<BandageManager>();
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

            if (script.Respawning)
            {
                Vector3 gripPosition = GetSpawnGripPosition(script);
                script.UpdateRespawnArrowPosition(gripPosition.x);

                if (i == 0)
                    spawnGrip = gripPosition;
            }

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
                    if (script.deathPositionX > (cam.transform.position.x + cam.orthographicSize * camEdgeMultiplier) || script.deathPositionX < (cam.transform.position.x - cam.orthographicSize * camEdgeMultiplier))
                    {
                        script.deathPositionX = cam.transform.position.x;
                    }

                    script.deathCount++;
                    Destroy(GameManager.players[i].parent.gameObject);
                    Invoke("PlayBounce", respawnTime - 0.2f);
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

    public void GameStarting()
    {
        mapGrips = transform.GetComponentsInChildren<Grip>(true);
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
        FrogPrototype frog = body.GetComponent<FrogPrototype>();
        frog.ForceArmsUp();
        Rigidbody2D b = body.GetComponent<Rigidbody2D>();
        b.isKinematic = false;
        b.AddForce(Vector2.up * 650000);
        b.freezeRotation = true;
        StartCoroutine(ResetBodyFreeze(b, 0.5f));

        body.GetComponent<FrogColliderDisabler>().DisableRockColliders();
        return body;
    }

    private IEnumerator ResetBodyFreeze(Rigidbody2D body, float time)
    {
        yield return new WaitForSeconds(time);

        body.freezeRotation = false;
    }
    private void PlayBounce()
    {
        AudioSource canon = GameManager.PlayClipAtPoint(bounceAudioSource.clip, Camera.main.transform.position);
        canon.outputAudioMixerGroup = bounceAudioSource.outputAudioMixerGroup;
        canon.pitch = Random.Range(0.6f, 2.2f);
    }
    public void ResetDeathcount()
    {
        for (int i = 0; i < respawnScripts.Count; i++)
        {
            respawnScripts[i].deathCount = 0;
        }
    }

    Vector3 spawnGrip;
    Vector3 cameraBottom;
    Vector3 deathPositionX;
    private float camEdgeMultiplier = 2.28f;
    void OnDrawGizmos()
    {
        if (!drawGizmos)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(spawnGrip, 0.7f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(cameraBottom, 0.3f);

        if (cam != null)
        {
            Vector3 camEdge = cameraBottom;
            camEdge.x += cam.orthographicSize * camEdgeMultiplier;
            Gizmos.DrawWireSphere(camEdge, 0.3f);

            camEdge = cameraBottom;
            camEdge.x -= cam.orthographicSize * camEdgeMultiplier;
            Gizmos.DrawWireSphere(camEdge, 0.3f);
        }
        

        if (respawnScripts[0] != null)
        {
            deathPositionX = cameraBottom;
            deathPositionX.x = respawnScripts[0].deathPositionX;
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(deathPositionX, 0.5f);
        }

        if (mapGrips == null)
            return;
        for (int i = 0; i < mapGrips.Length; i++)
        {
            if (mapGrips[i] == null)
                continue;

            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(mapGrips[i].transform.position, 0.3f);
        }
    }
    Vector3 GetSpawnGripPosition(PlayerRespawn player)
    {
        cameraBottom = new Vector3(cam.transform.position.x, cam.transform.position.y - cam.orthographicSize);

        if (mapGrips.Length <= 0)
            return cameraBottom;

        int closestIndex = -1;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < mapGrips.Length; i++)
        {
            if (mapGrips[i] == null)
                continue;

            Vector3 targetSpawnPosition = cam.transform.position + new Vector3(0, -2.5f);
            targetSpawnPosition.x = player.deathPositionX;

            float yDistance = mapGrips[i].transform.position.y - cameraBottom.y;
            float xDistance = Mathf.Abs(mapGrips[i].transform.position.x - player.deathPositionX);

            if (yDistance > 0 && yDistance < 10)
            {
                if (xDistance < closestDistance)
                {
                    closestDistance = xDistance;
                    closestIndex = i;
                }
            }
        }

        if (closestIndex != -1 && mapGrips[closestIndex] != null)
        {
            return mapGrips[closestIndex].transform.position;
        }

        return cameraBottom;

        //if (grips.Length > 0)
        //{
        //    int minIndex = 0;

        //    for (int i = 1; i < grips.Length; i++)
        //    {
        //        Vector3 targetSpawnPosition = cam.transform.position + new Vector3(0, -2.5f);
        //        targetSpawnPosition.x = player.deathPositionX;

        //        float minDistance = Vector2.Distance(grips[minIndex].transform.position, targetSpawnPosition);
        //        float distance = Vector2.Distance(grips[i].transform.position, targetSpawnPosition);

        //        if (distance < minDistance)
        //        {
        //            minIndex = i;
        //        }
        //    }


        //    Vector3 pos = grips[minIndex].transform.position;
        //    pos.y = minHeight - 6;
        //    return pos;
        //}

        //return cam.transform.position;
    }
    float GetSpawnOffset(PlayerRespawn player)
    {
        return 7.5f;

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
}
