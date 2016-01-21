using UnityEngine;
using System.Collections;

public class TutorialBubbles : MonoBehaviour 
{
    public void Enable(FrogPrototype[] frogs)
    {
        for (int i = 0; i < frogs.Length; i++)
        {
            if (frogs[i] != null)
            {
                frogs[i].EnableTutorial();
            }
        }
    }
    public void Disable(FrogPrototype[] frogs)
    {
        for (int i = 0; i < frogs.Length; i++)
        {
            if (frogs[i] != null)
            {
                frogs[i].DisableTutorial();
            }
        }
    }
}
