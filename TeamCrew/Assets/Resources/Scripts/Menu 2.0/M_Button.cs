using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(SpriteRenderer))]
public class M_Button : MonoBehaviour
{
    //Data
    public Sprite pressedSprite;
    public Sprite selectedSprite;
    private Sprite defaultSprite;

    //Static
    public static float pressDelay = 0.2f;

    //Components
    private SpriteRenderer renderer;

    void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        defaultSprite = renderer.sprite;
    }

    //Editor
    public void EditorUpdate()
    {
        //Set sortinglayer
        TextMesh[] texts = transform.GetComponentsInChildren<TextMesh>();
        if (texts.Length > 0)
        {
            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].GetComponent<MeshRenderer>().sortingLayerName = "Text";
                texts[i].GetComponent<MeshRenderer>().sortingOrder = int.MaxValue;
            }
        }
    }

    //Methods
    IEnumerator UnPress()
    {
        yield return new WaitForSeconds(pressDelay);

        OnSelect();
    }

    //Events
    public void OnPress()
    {
        StartCoroutine(UnPress());
        renderer.sprite = pressedSprite;
    }
    public void OnSelect()
    {
        renderer.sprite = selectedSprite;
    }
    public void OnDeSelect()
    {
        StopAllCoroutines();
        renderer.sprite = defaultSprite;
    }
}
