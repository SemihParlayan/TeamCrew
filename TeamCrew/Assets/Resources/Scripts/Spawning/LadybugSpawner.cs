using UnityEngine; 
using System.Collections;
using System.Collections.Generic;

public class LadybugSpawner : MonoBehaviour
{
    //Data
    public float respawnCheckRate = 2; //in seconds
    private float spawnTimer = 0;

    //References
    public Transform ladybugPrefab;
    public Transform currentLadybug;
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        int activeFrogCount = 0;
        for (int i = 0; i < GameManager.players.Length; i++)
        {
            if (GameManager.players[i] != null)
                activeFrogCount++;
        }

        if (activeFrogCount <= 1)
            return;

        if (!gameManager.tutorialComplete)
            return;

        if (currentLadybug != null)
            return;


        spawnTimer += Time.deltaTime;

        if (spawnTimer >= respawnCheckRate)
        {
            spawnTimer -= respawnCheckRate;

            SpawnFly();
        }
    }

    void SpawnFly()
    {
        float playersDistanceY = Mathf.Abs(GameManager.GetTopFrog().position.y - GameManager.GetBottomFrog().position.y);

        if (playersDistanceY > 5 && Random.Range(0, 100) > 60)
        {
            Vector3 spawnPos = GameManager.GetBottomFrog().position;
            int dir = (Random.Range(0, 2) == 0) ? 1 : -1;

            spawnPos += Vector3.right * dir * 50f;
            currentLadybug = Instantiate(ladybugPrefab, spawnPos, Quaternion.identity) as Transform;
            currentLadybug.GetComponent<Insect>().spawner = this;
        }
    }
    public void RemoveFly()
    {
        if (currentLadybug)
        {
            Destroy(currentLadybug.gameObject);
            currentLadybug = null;
        }
    }
}