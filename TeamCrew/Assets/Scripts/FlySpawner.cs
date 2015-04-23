using UnityEngine;
using System.Collections;

public class FlySpawner : MonoBehaviour 
{
    public Transform FlyPrefab;
    public Transform playerOne;
    public Transform playerTwo;
    public float RespawnCheckRate;
    float nextCheck;

	void Start () 
    {
        RespawnCheckRate = 2;
	}
	
	void Update ()
    {
        nextCheck -= Time.deltaTime;
        if(nextCheck <= 0)
        {
            Debug.Log("2 seconds");
            nextCheck = RespawnCheckRate;
            if (Mathf.Abs(playerOne.position.y - playerTwo.position.y) > 5)
            {
                Debug.Log("Right distance");
                if (Random.Range(0, 100) > 60)
                {
                    Debug.Log("right random");
                    Instantiate(FlyPrefab);
                }


            }
        }
       
	}
}
