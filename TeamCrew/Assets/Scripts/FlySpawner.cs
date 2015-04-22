using UnityEngine;
using System.Collections;

public class FlySpawner : MonoBehaviour 
{
    public Transform FlyPrefab;
    public Transform playerOne;
    public Transform playerTwo;



	void Start () 
    {
	}
	
	void Update () 
    {
       if(Mathf.Abs(playerOne.position.y - playerTwo.position.y) > .8)
       {
           Debug.Log("fluger!");
           /*if Random spawn time
            * {
            * if random right left
            */

           Instantiate(FlyPrefab);
          
        }
	}
}
