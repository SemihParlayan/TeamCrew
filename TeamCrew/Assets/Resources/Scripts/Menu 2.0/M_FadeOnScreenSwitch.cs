using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(M_Screen))]
public class M_FadeOnScreenSwitch : MonoBehaviour 
{
    //Components
    public bool overideGameActiveFadeStop;
    public float fadeMultiplier = 1.0f;
    private M_Screen screen;
    public bool JustFadedIn { get { return fadedIn && !lastFadedIn; } }
    public bool fadedIn;
    private bool lastFadedIn;
    private GameManager gamemanager;

    private SpriteRenderer[] renderers;
    private TextMesh[] textMeshes;

	void Awake () 
    {
        screen = GetComponent<M_Screen>();
        gamemanager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        renderers = GetComponentsInChildren<SpriteRenderer>();
        textMeshes = GetComponentsInChildren<TextMesh>();

        List<SpriteRenderer> rendComponents = new List<SpriteRenderer>();
        List<TextMesh> textComponents = new List<TextMesh>();

        SearchChild<SpriteRenderer>(rendComponents, transform);
        SearchChild<TextMesh>(textComponents, transform);

        renderers = rendComponents.ToArray();
        textMeshes = textComponents.ToArray();
	}
    private void SearchChild<Type>(List<Type> components, Transform parent) where Type : Component
    {
        int childCount = parent.childCount;

        for (int i = 0; i < childCount; i++)
        {
            Transform child = parent.GetChild(i);
            Type t = child.GetComponent<Type>();

            if (t != null)
            {
                components.Add(t);
            }

            SearchChild(components, child);
        }
    }
	void Update () 
    {
        if (gamemanager.gameActive && !overideGameActiveFadeStop)
            return;

        float delta = Time.deltaTime;
        if (delta == 0.0f)
            delta = 1 / 60f;
        delta *= fadeMultiplier;
        lastFadedIn = fadedIn;

        if (!screen.active)
        {
            //Spriterenderers
            bool completeFadedOut = true;
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
            for (int i = 0; i < textMeshes.Length; i++)
            {
                TextMesh t = textMeshes[i];
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
            for (int i = 0; i < textMeshes.Length; i++)
            {
                TextMesh t = textMeshes[i];
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
