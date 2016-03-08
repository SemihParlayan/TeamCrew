using UnityEngine;
using System.Collections;

public class ModifierAnimatorCaller : MonoBehaviour
{
    private Animator anim;
    private bool active;

    void Start()
    {
        anim = GetComponent<Animator>();    
    }

    public void Refresh()
    {
        anim.SetBool("ModeOn", active);
    }
    public void ModifierPressed()
    {
        active = !active;
        anim.SetBool("ModeOn", active);
    }
}
