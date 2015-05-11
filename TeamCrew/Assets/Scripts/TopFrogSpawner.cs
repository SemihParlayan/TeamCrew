using UnityEngine;
using System.Collections;

public class TopFrogSpawner : MonoBehaviour 
{
    public Vector3 spawnPosition;
    public Transform playerOneTopPrefab;
    public Transform playerTwoTopPrefab;

    private Transform currentTopFrog;
    private int frogNumberToSpawn;

    private void CreateFrog()
    {
        Transform spawnFrog = null;
        if (frogNumberToSpawn == 1)
        {
            spawnFrog = playerOneTopPrefab; ;
        }
        else if (frogNumberToSpawn == 2)
        {
            spawnFrog = playerTwoTopPrefab;
        }

        if (spawnFrog != null)
            currentTopFrog = Instantiate(spawnFrog, spawnPosition, Quaternion.identity) as Transform;
    }
    public void SpawnFrog(int frogNumber, float timeUntilSpawn)
    {
        frogNumberToSpawn = frogNumber;
        Invoke("CreateFrog", timeUntilSpawn);

    }
    public void RemoveFrog()
    {
        if (currentTopFrog)
        {
            Destroy(currentTopFrog.gameObject);
        }
    }
}
