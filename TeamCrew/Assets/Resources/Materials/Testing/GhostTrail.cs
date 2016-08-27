using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ghost
{
    public SpriteRenderer spriteRenderer;
    public Transform transform;

    public void Fade(float fadeTime)
    {
        if (AlphaAtZero())
            return;

        Color color = spriteRenderer.color;
        color.a = Mathf.Lerp(color.a, 0, Time.deltaTime * fadeTime);
        spriteRenderer.color = color;
    }
    public bool AlphaAtZero()
    {
        return spriteRenderer.color.a <= 0;
    }
    public void Activate(Transform parent)
    {
        transform.position = parent.position;
        transform.localScale = parent.localScale;
        transform.rotation = parent.rotation;

        spriteRenderer.color = Color.white;
    }
}
public class GhostTrail : MonoBehaviour 
{
	//publics
    public Transform copyParent;
    public SpriteRenderer spriteToCopy;
    public int numberOfCopies;

    public bool flipY;
    public bool flipX;
    public float copyInterval;
    public float fadeTime;

	//privates
    private List<Ghost> ghosts = new List<Ghost>();
    private float copyTimer;

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
            ghost.spriteRenderer.sortingOrder = spriteToCopy.sortingOrder;
            ghost.spriteRenderer.flipX = flipX;
            ghost.spriteRenderer.flipY = flipY;
            ghost.transform = obj.transform;

            ghosts.Add(ghost); 
        }
	}
	void Update () 
	{
        copyTimer += Time.deltaTime;

        if (copyTimer >= copyInterval)
        {
            copyTimer -= copyInterval;

            FindGhost().Activate(transform);
        }

        foreach(Ghost ghost in ghosts)
        {
            ghost.Fade(fadeTime);
        }
	}

	//public methods

	//private methods
    private Ghost FindGhost()
    {
        Ghost minGhost = ghosts[0];
        float minAlpha = minGhost.spriteRenderer.color.a;

        foreach(Ghost g in ghosts)
        {
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
