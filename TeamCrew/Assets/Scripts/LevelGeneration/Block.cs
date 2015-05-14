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
    Top,
    Tutorial
}
public class Block : MonoBehaviour 
{
    public BlockEnding end;
    public BlockEnding start;
    public BlockDifficulty difficulty;

    //[HideInInspector]
    public int blockIndex;

    void OnBecameVisible()
    {
        enabled = true;
    }

    void OnBecameInvisible()
    {
        enabled = false;
    }

}

