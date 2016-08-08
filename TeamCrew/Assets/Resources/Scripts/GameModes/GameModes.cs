using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public enum BlockTag
{
    Easy, Medium, Hard, 
    Tutorial, Converter, Climb, Top, Wheel, Daily
}
public class GameModes : MonoBehaviour 
{
    public List<GameMode> gameModes = new List<GameMode>();
    public List<GameMode> dailyGameModes = new List<GameMode>();

    void Start()
    {
        GameManager.CurrentGameMode = gameModes.First();

        foreach(GameMode mode in gameModes)
        {
            mode.Initialize();
        }
        foreach(GameMode mode in dailyGameModes)
        {
            mode.Initialize();
        }
    }

    public GameMode GetRandomDailyGameMode()
    {
        if (dailyGameModes.Count == 0)
            return null;

        return dailyGameModes[Random.Range(0, dailyGameModes.Count)];
    }
}

[System.Serializable]
public class GameMode
{
    public string name;
    public Sprite picture;
    [TextArea(1, 10)]
    public string description;
    public TagCollection topBlock;
    public List<TagCollection> climbingBlocks = new List<TagCollection>();
    public TagCollection tutorialBlock;

    public void Initialize()
    {
        if (!topBlock.ContainsTag(BlockTag.Top))
        {
            topBlock.tags.Add(BlockTag.Top);
        }

        if (!tutorialBlock.ContainsTag(BlockTag.Tutorial))
        {
            tutorialBlock.tags.Add(BlockTag.Tutorial);
        }

        foreach(TagCollection climbingBlock in climbingBlocks)
        {
            if (!climbingBlock.ContainsTag(BlockTag.Climb))
            {
                climbingBlock.tags.Add(BlockTag.Climb);
            }
        }
    }
}

[System.Serializable]
public class TagCollection
{
    public List<BlockTag> tags = new List<BlockTag>();

    public bool ContainsTag(BlockTag tag)
    {
        foreach (BlockTag t in tags)
        {
            if (t == tag)
                return true;
        }

        return false;
    }
    public bool ContainsTags(TagCollection tags)
    {
        foreach (BlockTag tag in tags.tags)
        {
            if (!ContainsTag(tag))
            {
                return false;
            }
        }
        return true;
    }
}
