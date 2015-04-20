using UnityEngine;
using System.Collections;

public enum BlockEnding 
{ 
    A,
    B,
    AB,
    None
}
public enum BlockDifficulty
{
    Easy,
    Medium,
    Hard,
    Top
}
public class Block : MonoBehaviour 
{
    public BlockEnding end;
    public BlockEnding start;
    public BlockDifficulty difficulty;

    private Bounds bounds;

    void Start()
    {
        bounds = GetComponent<SpriteRenderer>().sprite.bounds;
    }

    public Vector3 StartPosition 
    { 
        get
        {
            return transform.position + new Vector3(bounds.max.x, bounds.min.y);
        }
    }
}
