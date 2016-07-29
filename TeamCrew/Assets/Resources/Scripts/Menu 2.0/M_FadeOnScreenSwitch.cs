using UnityEngine;
using System.Collections;

[RequireComponent(typeof(M_Screen))]
public class M_FadeOnScreenSwitch : MonoBehaviour 
{
    //Components
    public float fadeMultiplier = 1.0f;
    private M_Screen screen;
    public bool JustFadedIn { get { return fadedIn && !lastFadedIn; } }
    public bool fadedIn;
    private bool lastFadedIn;

	void Awake () 
    {
        screen = GetComponent<M_Screen>();
	}
	void Update () 
    {
        float delta = Time.deltaTime;
        if (delta == 0.0f)
            delta = 1 / 60f;
        delta *= fadeMultiplier;
        lastFadedIn = fadedIn;

        if (!screen.active)
        {
            //Spriterenderers
            bool completeFadedOut = true;
            SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                SpriteRenderer r = renderers[i];
                Color c = r.color;


                if (c.a > 0)
                {
                    completeFadedOut = false;
                    c.a = Mathf.MoveTowards(c.a, 0, delta);
                    r.color = c;
                }
            }

            //TextmeshesW
            TextMesh[] meshes = GetComponentsInChildren<TextMesh>();
            for (int i = 0; i < meshes.Length; i++)
            {
                TextMesh t = meshes[i];
                Color c = t.color;

                if (c.a > 0)
                {
                    completeFadedOut = false;
                    c.a = Mathf.MoveTowards(c.a, 0, delta);
                    t.color = c;
                }
            }

            fadedIn = completeFadedOut;
        }
        else
        {
            //Spriterenderers
            bool completeFadedIn = true;
            SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                SpriteRenderer r = renderers[i];
                Color c = r.color;

                if (c.a < 1)
                {
                    completeFadedIn = false;
                    c.a = Mathf.MoveTowards(c.a, 1, delta);
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
                    completeFadedIn = false;
                    c.a = Mathf.MoveTowards(c.a, 1, delta);
                    t.color = c;
                }
            }

            fadedIn = completeFadedIn;
        }
	}
}
