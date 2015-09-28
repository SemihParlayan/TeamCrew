using UnityEngine;
using System.Collections;

public class TopFrogSpawner : MonoBehaviour 
{
    public Vector3 spawnPosition;
    public Transform playerOneTopPrefab;
    public Transform playerTwoTopPrefab;

    private Transform currentTopFrog;
    private int frogNumberToSpawn;
    //private bool accessories; //Never used
	public int accessoriesCount = -1;

    private Respawn respawnScript;

    void Start()
    {
        respawnScript = GetComponent<Respawn>();
    }
    private void CreateFrog()
    {
        Transform spawnFrog = null;
        float bandageCount = 0;
        if (frogNumberToSpawn == 1)
        {
            spawnFrog = playerOneTopPrefab; ;
            bandageCount = respawnScript.respawnScripts[0].deathCount;
        }
        else if (frogNumberToSpawn == 2)
        {
            spawnFrog = playerTwoTopPrefab;
            bandageCount = respawnScript.respawnScripts[1].deathCount;
        }

        if (spawnFrog != null)
            currentTopFrog = Instantiate(spawnFrog, spawnPosition, Quaternion.identity) as Transform;

        if (currentTopFrog)
        {
            Bandage b = currentTopFrog.FindChild("body").GetComponent<Bandage>();

            for (int i = 0; i < bandageCount; i++)
            {
                b.AddBandage(2);
            }

                currentTopFrog.FindChild("body").GetComponent<TopFrog>().AddAccessories(accessoriesCount);
        }
    }
    public void SpawnFrog(int frogNumber, float timeUntilSpawn, bool accessories = false)
    {
        //this.accessories = accessories; //Never used
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
