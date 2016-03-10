using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(SpriteRenderer))]
public class M_Button : MonoBehaviour
{
    //Data
    public bool animated;
    public bool Disabled { get { return disabled; } set { disabled = value; OnDisabled(value); } }
    private bool disabled;
    public Sprite pressedSprite;
    public Sprite selectedSprite;
    private Sprite defaultSprite;
    protected Animator anim;

    //Static
    public static float pressDelay = 0.2f;

    //Components
    private SpriteRenderer renderer;
    

    //References
    private M_Sounds soundManager;

    void Awake()
    {
        soundManager = GameObject.FindWithTag("SoundManager").GetComponent<M_Sounds>();
        renderer = GetComponent<SpriteRenderer>();
        defaultSprite = renderer.sprite;

        if (animated)
        {
            anim = GetComponent<Animator>();
        }
    }

    void Start()
    {
        OnStart();
    }
    protected virtual void OnStart()
    {

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
    protected IEnumerator UnPress()
    {
        yield return new WaitForSeconds(pressDelay);

        OnSelect();
    }

    //Events
    public virtual void OnPress()
    {
        soundManager.buttonClick.Play();
        if (animated)
        {
            anim.SetTrigger("OnPress");
            return;
        }
        else
        {
            StartCoroutine(UnPress());
            renderer.sprite = pressedSprite;
        }
    }
    public virtual void OnSelect()
    {
        if (animated)
        {
            anim.SetBool("Selected", true);
            return;
        }
        else
            renderer.sprite = selectedSprite;
    }
    public virtual void OnDeSelect()
    {
        soundManager.buttonSwitch.Play();
        if (animated)
        {
            anim.SetBool("Selected", false);
            return;
        }
        else
        {
            StopAllCoroutines();
            renderer.sprite = defaultSprite;
        }
        
    }
    private void OnDisabled(bool value)
    {
        if (animated)
        {
            anim.SetBool("Disabled", value);
        }
    }
}
