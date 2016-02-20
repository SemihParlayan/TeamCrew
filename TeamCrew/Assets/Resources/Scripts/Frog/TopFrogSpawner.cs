using UnityEngine;
using System.Collections;

public class TopFrogSpawner : MonoBehaviour 
{
    public Vector3 spawnPosition;
    public Transform currentTopFrog;
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
            GameManager gameManager = GetComponent<GameManager>();
            gameManager.transformOrder.Insert(0, currentTopFrog.FindChild("body"));
            gameManager.ActivateTopNumbers();

            if (currentTopFrog)
            {
                currentTopFrog.position += Vector3.up;

                Bandage bandage = respawnScript.bandageManager.GetBandage(victoryFrogNumber);
                int bandageCount = 0;
                if (bandage)
                {
                    bandageCount = bandage.bandageCount;
                }

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
