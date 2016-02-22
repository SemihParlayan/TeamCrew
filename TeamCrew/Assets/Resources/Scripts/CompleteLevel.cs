using UnityEngine;
using System.Collections;

public class CompleteLevel : MonoBehaviour 
{
    public Transform completeLevelPrefab = null;

    void Start()
    {
        SetLevel();
    }
    void OnDrawGizmos()
    {
        SetLevel();
    }

    void SetLevel()
    {
        if (completeLevelPrefab == null)
        {
            completeLevelPrefab = Resources.Load("Prefabs/Complete level", typeof(Transform)) as Transform;
        }

        if (transform.position != completeLevelPrefab.position)
            transform.position = completeLevelPrefab.position;
    }
}
