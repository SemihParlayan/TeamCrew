using UnityEngine;
using System.Collections;

public class TopNumberBubble : MonoBehaviour 
{
    private static Vector3 offset = new Vector3(3, 0, 0);
    private static float speed = 3;

    public Transform target;

    void Update()
    {
        if (target == null)
            return;

        Vector3 targetPosition = target.position;
        targetPosition += offset;
        targetPosition.z = 0;


        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);
    }
}
