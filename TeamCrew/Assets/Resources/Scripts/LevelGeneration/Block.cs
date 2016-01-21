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
    Tutorial_1player,
    Tutorial_2player,
    Tutorial_3player,
    Tutorial_4player,
    Converter
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
            Vector3 pos = Vector3.zero;
            if (difficulty == BlockDifficulty.Top)
            {
                if (renderer != null)
                {
                    pos = transform.position;
                    pos.x -= size.x / 2;
                    pos.x += pixelsFromLeftEnd / 100.0f;
                    pos.y += (size.y / 2) - 8;
                }
                else
                {
                    pos = transform.position;
                    pos.x -= size.x / 2;
                    pos.x += pixelsFromLeftEnd / 100.0f;
                    pos.y += blockheightTop;
                }
            }
            else
            {
                if (renderer != null)
                {
                    pos = transform.position;
                    pos.x -= size.x / 2;
                    pos.x += pixelsFromLeftEnd / 100.0f;
                    pos.y += size.y / 2;
                }
                else
                {
                    pos = transform.position;
                    pos.x -= size.x / 2;
                    pos.x += pixelsFromLeftEnd / 100.0f;
                    pos.y += blockheightTop;
                }
            }

            return pos;
        }
    }
    public Vector3 GetEndCenterPosition
    {
        get
        {
            Vector3 pos = GetEndPosition;

            float pixelWidth = 913;

            if (end == BlockEnding.Thin || end == BlockEnding.None)
                pixelWidth = 613;

            pos.x += pixelWidth / 100.0f;
            return pos;
        }
    }

    public Vector3 GetStartPosition
    {
        get
        { 
            Vector3 pos = Vector3.zero;
            if (renderer != null)
            {
                pos = transform.position;
                pos.x -= size.x / 2;
                pos.x += pixelsFromLeftStart / 100.0f;
                pos.y -= size.y / 2;
            }
            else
            {
                pos = transform.position;
                pos.x += pixelsFromLeftStart / 100.0f;
                pos.y -= blockheightBottom;
            }
            return pos;
        }
    }
    public Vector3 GetStartCenterPosition
    {
        get
        {
            Vector3 pos = GetStartPosition;

            float pixelWidth = 913;

            if (start == BlockEnding.Thin)
                pixelWidth = 613;

            pos.x += pixelWidth / 100.0f;
            return pos;
        }
    }

    public float blockheightTop;
    public float blockheightBottom;
    public  float gizmoSize = 0.2f;
    private new SpriteRenderer renderer;

    public Vector3 size
    {
        get
        {
            if(renderer != null)
            {
                return renderer.sprite.bounds.size;
            }
            else
            {
                return Vector3.zero;
            }
        }
    }

    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
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
        Gizmos.DrawCube(GetEndPosition, new Vector3(gizmoSize, gizmoSize));

        Gizmos.color = new Color(0f, 1f, 0f, 1f);
        Gizmos.DrawCube(GetStartPosition, new Vector3(gizmoSize, gizmoSize));

        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(GetStartCenterPosition, new Vector3(gizmoSize, gizmoSize));
        Gizmos.DrawCube(GetEndCenterPosition, new Vector3(gizmoSize, gizmoSize));
    }
}

