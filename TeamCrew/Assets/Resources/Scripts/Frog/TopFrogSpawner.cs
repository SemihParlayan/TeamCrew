using UnityEngine;
using System.Collections;

public class TopFrogSpawner : MonoBehaviour 
{
    public Vector3 spawnPosition;
    private Transform currentTopFrog;
    private Respawn respawnScript;

    void Start()
    {
        respawnScript = GetComponent<Respawn>();
    }

    
    public void SpawnFrog(Transform topfrogPrefab, int victoryFrogNumber, float timeUntilSpawn)
    {
        if (currentTopFrog == null)
            StartCoroutine(CreateFrog(topfrogPrefab, victoryFrogNumber, timeUntilSpawn));
    }
    private IEnumerator CreateFrog(Transform topfrogPrefab, int victoryFrogNumber, float timeUntilSpawn)
    {
        yield return new WaitForSeconds(timeUntilSpawn);
        if (currentTopFrog == null)
        {
            currentTopFrog = Instantiate(topfrogPrefab, spawnPosition, Quaternion.identity) as Transform;

            if (currentTopFrog)
            {
                currentTopFrog.position += Vector3.up;

                int bandageCount = respawnScript.bandageManager.GetBandage(victoryFrogNumber).bandageCount;
                Debug.Log("VictoryFrogNumber: " + victoryFrogNumber);
                Debug.Log("BandageCount: " + bandageCount);

                Bandage b = currentTopFrog.FindChild("body").GetComponent<Bandage>();
                for (int i = 0; i < bandageCount; i++)
                {
                    b.AddBandage(1);
                }
            }
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
