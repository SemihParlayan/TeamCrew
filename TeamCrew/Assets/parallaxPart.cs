using UnityEngine;
using System.Collections;

public class parallaxPart : MonoBehaviour
{
    private SpriteRenderer render;
    public Transform aboveLayer;
	void Start()
    {
        render = transform.GetComponent<SpriteRenderer>();
        //if(transparentParallax)
        //render.color = new Color(1f, 1f, 1f, .5f);
        //Debug.Log(render.color.a);
        if(aboveLayer == null)
        {
            enabled = false;
        }
    }
	
	void Update()
    {
        if (transform.position.y < aboveLayer.position.y)
        {
            //render.enabled = false;
        }
        else
        {
            render.enabled = true;
        }
            
	}
}
