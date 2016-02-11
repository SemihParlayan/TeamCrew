using UnityEngine;
using System.Collections;

public enum BlockEnding 
{ 
    Thin,
    Thick,
    None
}
public class Block : MonoBehaviour 
{
    public static float ThinWidth = 12.26f;
    public static float ThickWidth = 18.26f;

    public TagCollection tagCollection;
    public int pixelsFromLeftStart;
    public int pixelsFromLeftEnd;
    public BlockEnding startSize;
    public BlockEnding endSize;

    [HideInInspector]
    public int blockIndex;

    public Vector3 noRendererStartOffset;
    public Vector3 noRendererEndOffset;

    public Vector3 GetEndPosition
    {
        get
        {
            Vector3 pos = Vector3.zero;

            if (renderer != null)
            {
                pos = transform.position;
                pos.x -= size.x / 2;
                pos.x += pixelsFromLeftEnd / 100.0f;
                pos.y += (size.y / 2);
                
            }
            else
            {
                pos = transform.position - noRendererEndOffset;
            }

            if (tagCollection.ContainsTag(BlockTag.Top))
            {
                pos.y -= 8;
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

            if (endSize == BlockEnding.Thin || endSize == BlockEnding.None)
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
                pos = transform.position - noRendererStartOffset;
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

            if (startSize == BlockEnding.Thin)
                pixelWidth = 613;

            pos.x += pixelWidth / 100.0f;
            return pos;
        }
    }

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

    void Awake()
    {
        OnAwake();
    }
    protected virtual void OnAwake()
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
    
    public void SetStartPosition(Vector3 worldposition)
    {
        noRendererStartOffset = transform.position - worldposition;
    }
    public void SetEndPosition(Vector3 worldposition)
    {
        noRendererEndOffset = transform.position - worldposition;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(GetEndPosition, gizmoSize);

        Gizmos.color = new Color(0f, 1f, 0f, 1f);
        Gizmos.DrawWireSphere(GetStartPosition, gizmoSize);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(GetStartCenterPosition, gizmoSize);
        Gizmos.DrawWireSphere(GetEndCenterPosition, gizmoSize);
    }
}

