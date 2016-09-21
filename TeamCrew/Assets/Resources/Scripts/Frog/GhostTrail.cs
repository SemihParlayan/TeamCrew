using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ghost
{
    public SpriteRenderer spriteRenderer;
    public Transform transform;
    public Transform scaleParent;

    private bool show = true;

    public void Fade(float fadeTime)
    {
        if (AlphaAtZero())
            return;

        if (show && !transform.gameObject.activeInHierarchy)
            transform.gameObject.SetActive(true);


        Color color = spriteRenderer.color;
        color.a = Mathf.Lerp(color.a, 0, Time.deltaTime * fadeTime);
        spriteRenderer.color = color;

        if (AlphaAtZero())
        {
            if (!show)
                transform.gameObject.SetActive(false);
        }
    }
    public bool AlphaAtZero()
    {
        return spriteRenderer.color.a <= 0;
    }
    public void Activate(Transform parent)
    {
        transform.position = parent.position;
        transform.rotation = parent.rotation;

        if (scaleParent != null)
        {
            transform.localScale = scaleParent.lossyScale;
        }
        else
        {
            transform.localScale = parent.localScale;
        }

        spriteRenderer.color = new Color(1f, 1f, 1f, 0.7f);
    }
    public void Show()
    {
        show = true;
    }
    public void Hide()
    {
        show = false;
    }
}
public class GhostTrail : MonoBehaviour 
{
	//publics
    public Transform copyParent;
    public Transform scaleParent;
    public SpriteRenderer spriteToCopy;
    public int numberOfCopies;

    public bool flipY;
    public bool flipX;
    public float copyInterval;
    public float fadeTime;

	//privates
    private List<Ghost> ghosts = new List<Ghost>();
    private float copyTimer;
    private bool active = true;

	//Unity methods
	void Start () 
	{
	    for (int i = 0; i < numberOfCopies; i++)
        {
            GameObject obj = new GameObject("Ghost[" + i + "]");
            obj.transform.SetParent(copyParent);
             
            Ghost ghost = new Ghost();
            ghost.spriteRenderer = obj.AddComponent<SpriteRenderer>();
            ghost.spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
            ghost.spriteRenderer.sprite = spriteToCopy.sprite;
            ghost.spriteRenderer.sortingLayerName = spriteToCopy.sortingLayerName;
            ghost.spriteRenderer.sortingOrder = -1000;
            ghost.spriteRenderer.flipX = flipX;
            ghost.spriteRenderer.flipY = flipY;
            ghost.transform = obj.transform;

            if (scaleParent != null)
            {
                ghost.scaleParent = scaleParent;
            }

            ghosts.Add(ghost); 
        }
	}
	void Update () 
	{
        foreach (Ghost ghost in ghosts)
        {
            ghost.Fade(fadeTime);
        }

        if (!active)
            return;

        copyTimer += Time.deltaTime;

        if (copyTimer >= copyInterval)
        {
            copyTimer -= copyInterval;

            Ghost ghost = FindGhost();
            if (ghost != null)
            {
                ghost.Activate(transform);
            }
        }        
	}

	//public methods
    public void Show()
    {
        active = true;
        foreach(Ghost g in ghosts)
        {
            g.Show();
        }
    }
    public void Hide()
    {
        active = false;
        foreach(Ghost g in ghosts)
        {
            g.Hide();
        }
    }
	//private methods
    private Ghost FindGhost()
    {
        Ghost minGhost = null;
        float minAlpha = float.MaxValue;

        foreach(Ghost g in ghosts)
        {
            if (!gameObject.activeInHierarchy)
                continue;
            if (g.AlphaAtZero())
            {
                return g;
            }
            else
            {
                if (g.spriteRenderer.color.a < minAlpha)
                {
                    minAlpha = g.spriteRenderer.color.a;
                    minGhost = g;
                }
            }
        }

        return minGhost;
    }
}
