using UnityEngine;
using System.Collections;

public class TopNumbers : MonoBehaviour 
{
    public TopNumberBubble[] numbers = new TopNumberBubble[4];

    void Start()
    {
        DeActivateNumbers();
    }

    public void ActivateNumbers(Transform[] frogOrder)
    {
        for (int i = 0; i < frogOrder.Length; i++)
        {
            if (frogOrder[i] == null)
                continue;

            numbers[i].target = frogOrder[i];
            numbers[i].gameObject.SetActive(true);
        }
    }
    public void DeActivateNumbers()
    {
        foreach (TopNumberBubble n in numbers)
        {
            n.gameObject.SetActive(false);
        }
    }
}
