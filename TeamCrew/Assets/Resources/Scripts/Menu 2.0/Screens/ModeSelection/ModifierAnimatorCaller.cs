using UnityEngine;
using System.Collections;

public class ModifierAnimatorCaller : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();    
    }

    public void IconSwitch()
    {
        anim.SetTrigger("IconSwitch");
    }
}
