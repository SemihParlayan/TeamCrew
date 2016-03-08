using UnityEngine;
using System.Collections;

public class ChangeModifierText : MonoBehaviour 
{
    public TextMesh modifierNameText;
    public TextMesh modifierDescriptionText;

    public string modifierName;
    [TextArea(1, 10)]
    public string description;

    public void OnSelect()
    {
        modifierNameText.text = modifierName;
        modifierDescriptionText.text = description;
    }
}
