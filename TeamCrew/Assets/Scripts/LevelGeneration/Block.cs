using UnityEngine;
using System.Collections;

public enum BlockEnding 
{ 
    Thin,
    Thick,
    None
}
public enum BlockDifficulty
{
    Easy,
    Medium,
    Hard,
    Top,
    Tutorial
}
public class Block : MonoBehaviour 
{
    public static float ThinWidth = 12.26f;
    public static float ThickWidth = 18.26f;

    public BlockEnding end;
    public int pixelsFromLeftEnd;

    public BlockEnding start;
    public int pixelsFromLeftStart;

    public BlockDifficulty difficulty;

    [HideInInspector]
    public int blockIndex;

    public Vector3 GetEndPosition
    {
        get
        {
            Vector3 pos = transform.position;
            pos.x += pixelsFromLeftEnd / 100.0f;
            return pos;
        }
    }
    public Vector3 GetStartPosition
    {
        get
        {
            Vector3 pos = transform.position;
            pos.x += pixelsFromLeftStart / 100.0f;
            pos.y -= size.y;
            return pos;
        }
    }

    public Vector3 size
    {
        get
        {
            return GetComponent<SpriteRenderer>().sprite.bounds.size;
        }
    }

    void Start()
    {
    }

    void OnBecameVisible()
    {
        enabled = true;
    }

    void OnBecameInvisible()
    {
        enabled = false;
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(GetEndPosition, 0.25f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(GetStartPosition, 0.25f);
    }
}

