using UnityEngine; 
using System.Collections;
using System.Collections.Generic;

public class FlySpawner : MonoBehaviour
{
    public Transform FlyPrefab;
    public float RespawnCheckRate = 2; //Seconds
    Transform fly;


    void Start()
    {
        InvokeRepeating("TestFlySpawn", RespawnCheckRate, RespawnCheckRate);
    }

    void TestFlySpawn()
    {
        //Invoke("TestFlySpawn", RespawnCheckRate);

        Transform playerOne = GameManager.playerOne;
        Transform playerTwo = GameManager.playerTwo;

        if (!(GameManager.playerOne && GameManager.playerTwo && fly == null))
        {
            return;
        }
        

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