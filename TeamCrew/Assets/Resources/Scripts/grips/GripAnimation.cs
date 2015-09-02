using UnityEngine;
using System.Collections;

public class GripAnimation : MonoBehaviour 
{
    //Component
    private Animator anim;

    private Vector3 spawnRotation;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        transform.eulerAngles = spawnRotation;
    }

    public void Activate(string animationName)
    {
        gameObject.SetActive(true);
        spawnRotation = transform.parent.eulerAngles;
        anim.SetTrigger(animationName);
    }
    public void DeActivate()
    {
        anim.SetTrigger("exit");
        gameObject.SetActive(false);
    }
}
