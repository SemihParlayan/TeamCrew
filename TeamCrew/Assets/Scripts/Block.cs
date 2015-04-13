using UnityEngine;
using System.Collections;

public enum BlockEnding 
{ 
    A,
    B,
    AB,
    None
}
public class Block : MonoBehaviour 
{
    public BlockEnding end;
    public BlockEnding start;
}
