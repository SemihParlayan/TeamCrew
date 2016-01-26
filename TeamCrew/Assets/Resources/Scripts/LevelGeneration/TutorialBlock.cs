using UnityEngine;
using System.Collections;

public enum TutorialFrogCount
{
    Invalid,
    One,
    Two,
    Three,
    Four
}
public class TutorialBlock : Block 
{
    public Transform[] playerStartPosition;

    public SpriteRenderer green;
    public SpriteRenderer yellow;
    public SpriteRenderer red;

    public TutorialFrogCount frogCount;
}
