using UnityEngine;
using System.Collections;

public class TopFrogSpawner : MonoBehaviour 
{
    public Vector3 spawnPosition;
    public Transform playerOneTopPrefab;
    public Transform playerTwoTopPrefab;

    private Transform currentTopFrog;
	public int accessoriesCount = -1;

    private Respawn respawnScript;

    void Start()
    {
        respawnScript = GetComponent<Respawn>();
    }

    
    public void SpawnFrog(Transform topfrogPrefab, int victoryFrogNumber, float timeUntilSpawn)
    {
        StartCoroutine(CreateFrog(topfrogPrefab, victoryFrogNumber, timeUntilSpawn));
    }
    private IEnumerator CreateFrog(Transform topfrogPrefab, int victoryFrogNumber, float timeUntilSpawn)
    {
        yield return new WaitForSeconds(timeUntilSpawn);

        float bandageCount = respawnScript.respawnScripts[victoryFrogNumber - 1].deathCount;
        currentTopFrog = Instantiate(topfrogPrefab, spawnPosition, Quaternion.identity) as Transform;

        if (currentTopFrog)
        {
            currentTopFrog.position += Vector3.up;

            Bandage b = currentTopFrog.FindChild("body").GetComponent<Bandage>();
            for (int i = 0; i < bandageCount; i++)
            {
                b.AddBandage(2);
            }

            currentTopFrog.FindChild("body").GetComponent<TopFrog>().AddAccessories(accessoriesCount);
        }
    }
    public void RemoveFrog()
    {
        if (currentTopFrog)
        {
            Destroy(currentTopFrog.gameObject);
        }
    }
}
