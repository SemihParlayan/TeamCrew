using UnityEngine; 
using System.Collections;
using System.Collections.Generic;

public class FlySpawner : MonoBehaviour
{
    public Transform FlyPrefab;
    public float RespawnCheckRate = 2; //Seconds
    Transform fly;

    private float spawnTimer = 0;
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= RespawnCheckRate)
        {
            spawnTimer -= RespawnCheckRate;

            SpawnFly();
        }
    }
    void SpawnFly()
    {
        Transform playerOne = GameManager.players[0];
        Transform playerTwo = GameManager.players[1];

        if (GameManager.players[0] == null || GameManager.players[1] == null || fly != null || !gameManager.tutorialComplete)// || !gameManager.tutorialComplete)
            return;

        float playersDistanceY = Mathf.Abs(playerOne.position.y - playerTwo.position.y);

        if (playersDistanceY > 5 && Random.Range(0, 100) > 60)
        {
            fly = Instantiate(FlyPrefab) as Transform;
        }
    }

    public void RemoveFly()
    {
        if (fly)
        {
            Destroy(fly.gameObject);
        }
    }
}