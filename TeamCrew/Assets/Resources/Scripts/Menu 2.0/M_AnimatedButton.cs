using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class M_AnimatedButton : M_Button
{
    //Components
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public override void OnPress()
    {
        anim.SetTrigger("OnPress");
    }
    public override void OnSelect()
    {
        anim.SetTrigger("OnSelect");
    }
    public override void OnDeSelect()
    {
        anim.SetTrigger("OnDeSelect");
    }
    public void Deselect()
    {
        OnDeSelect();
    }
    public void Select()
    {
        OnSelect();
    }
}
