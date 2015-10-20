using UnityEngine;
using System.Collections;

[RequireComponent(typeof(M_Screen))]
public class M_FadeOnScreenSwitch : MonoBehaviour 
{
    //Components
    private M_Screen screen;

	void Start () 
    {
        screen = GetComponent<M_Screen>();
	}

	void Update () 
    {
        if (!screen.active)
        {
            //Spriterenderers
            SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                SpriteRenderer r = renderers[i];
                Color c = r.color;

                if (c.a > 0)
                {
                    c.a = Mathf.MoveTowards(c.a, 0, Time.deltaTime);
                    r.color = c;
                }
            }

            //Textmeshes
            TextMesh[] meshes = GetComponentsInChildren<TextMesh>();
            for (int i = 0; i < meshes.Length; i++)
            {
                TextMesh t = meshes[i];
                Color c = t.color;

                if (c.a > 0)
                {
                    c.a = Mathf.MoveTowards(c.a, 0, Time.deltaTime);
                    t.color = c;
                }
            }
        }
        else
        {
            //Spriterenderers
            SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                SpriteRenderer r = renderers[i];
                Color c = r.color;

                if (c.a < 1)
                {
                    c.a = Mathf.MoveTowards(c.a, 1, Time.deltaTime);
                    r.color = c;
                }
            }

            //Textmeshes
            TextMesh[] meshes = GetComponentsInChildren<TextMesh>();
            for (int i = 0; i < meshes.Length; i++)
            {
                TextMesh t = meshes[i];
                Color c = t.color;

                if (c.a < 1)
                {
                    c.a = Mathf.MoveTowards(c.a, 1, Time.deltaTime);
                    t.color = c;
                }
            }
        }
	}
}
