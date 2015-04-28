using UnityEngine; 
using System.Collections;

public class FlySpawner : MonoBehaviour 
{
    public Transform FlyPrefab;

    public float RespawnCheckRate;
           float nextCheck;

	void Start () 
    {
        RespawnCheckRate = 2;
	}
	
	void Update ()
    {
        //Semih should I check for new frogs every update??
        if (!(GameManager.playerOne && GameManager.playerTwo))
            return;

        Transform playerOne = GameManager.playerOne;
        Transform playerTwo = GameManager.playerTwo;


        nextCheck -= Time.deltaTime;
        if(nextCheck <= 0)
        {
            Debug.Log("2 seconds");
            nextCheck = RespawnCheckRate;

            float playersDistanceY = Mathf.Abs(playerOne.position.y - playerTwo.position.y);

            if (playersDistanceY > 5 && Random.Range(0, 100) > 60)
            {
                Debug.Log("right random and distance. Distance: " + playersDistanceY);
                Instantiate(FlyPrefab);
            }

        }
	}
}
