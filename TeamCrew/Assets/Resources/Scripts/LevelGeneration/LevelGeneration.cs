﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelGeneration : MonoBehaviour 
{
    public Transform menuTutorialBlock;
    private List<Block> globalBlockCollection = new List<Block>();
    private List<Block> level = new List<Block>();

    private GameManager gameManager;
    private GameObject firstBlock;
    private TutorialBlock tutorialBlock;
    private System.Random rnd;

    public float LevelHeight 
    { 
        get 
        { 
            if (!menuTutorialBlock)
                return 0;

            return (level.Last().GetEndCenterPosition).y;
        } 
    }

	void Start () 
    {
        rnd = new System.Random(50);
        LoadLevelBlocks();

        //Make sure we have a menu block assigned
        if (menuTutorialBlock == null)
        {
            Debug.LogError("Assign menu tutorial block to levelgeneration");
        }

        //Aquire gamemanager
        gameManager = GetComponent<GameManager>();
	}
    void Update()
    {
    }
    public void GenerateFullMountain(bool keepTutorial = false, int seed = -1)
    {
        GameMode mode = GameManager.CurrentGameMode;

        //Remove previouslevel
        DestroyLevel(keepTutorial);

        //Spawn Tutorial
        Block block = null;
        if (!keepTutorial)
        {
            block = FindBlock(mode.tutorialBlock, false, seed);
            level.Add(block);
            tutorialBlock = block as TutorialBlock;
        }
        else
        {
            block = tutorialBlock;
        }
		
        //Spawn climbing blocks
        for (int i = 0; i < mode.climbingBlocks.Count; i++)
        {
            //Find a climbing block
            TagCollection climbingBlock = null;

            //Add Converter if neccesary
            if (i == 0)
            {
                //Find a climbing block
                climbingBlock = mode.climbingBlocks[i];
                block = FindBlock(climbingBlock, true, seed);

                firstBlock = block.gameObject;

                if (block.startSize != tutorialBlock.endSize)
                {
                    TagCollection c = new TagCollection(); c.tags.Add(BlockTag.Converter);
                    Block converter = FindBlock(c, true, seed);
                    firstBlock = converter.gameObject;
                    level.Add(converter);
                    ConnectBlocks(converter, tutorialBlock);
                    ConnectBlocks(block, converter);
                }
            }
            else
            {
                //Find a climbing block
                climbingBlock = mode.climbingBlocks[i];
                block = FindBlock(climbingBlock, false, seed);
            }


            //Add climbing block
            level.Add(block);
        }

        //Spawn top block
        block = FindBlock(mode.topBlock, false, seed);
        level.Add(block);
    }

    private void FixSigns()
    {
        ActivateFirstBlock();
        GameObject[] signs = GameObject.FindGameObjectsWithTag("Sign");
        DeactivateFirstBlock();

        for (int i = 0; i < signs.Length; i++)
        {
            float y = Mathf.Abs(Mathf.RoundToInt(Mathf.Abs(signs[i].transform.position.y) - LevelHeight));

            if (signs[i].transform.childCount == 0)
                Debug.LogError("Sign is missing a textmesh at: " + signs[i].transform.parent.name);

            TextMesh text = signs[i].transform.GetChild(0).GetComponent<TextMesh>();
            if (text)
            {
                text.text = y.ToString() + "m";
            }
        }
    }
    public Block FindBlock(TagCollection minimumRequiredTags, bool ignorBlockWidthMatching = false, int seed = -1)
    {
        if (seed != -1)
            rnd = new System.Random(seed);

        List<Block> availableBlocks = new List<Block>();
        Block previousBlock = (level.Count > 0) ? level.Last() : null;
        bool searchingForTutorial = minimumRequiredTags.ContainsTag(BlockTag.Tutorial);

        //Find blocks that contain the required tags
        foreach(Block block in globalBlockCollection)
        {
            if (block.tagCollection.ContainsTags(minimumRequiredTags))
            {
                //Are we searching for a tutorial block?
                if (searchingForTutorial && block is TutorialBlock)
                {
                    TutorialBlock tutorial = block as TutorialBlock;

                    if (tutorial.frogCount == GetTutorialFrogCount())
                    {
                        availableBlocks.Add(block);
                    }
                }
                else
                {
                    availableBlocks.Add(block);
                }
            }
        }

        //Sort through so that size fits except for tutorial
        if (!searchingForTutorial && !ignorBlockWidthMatching)
        {
            List<Block> tmpBlocks = new List<Block>();
            tmpBlocks.AddRange(availableBlocks);
            availableBlocks.Clear();

            for (int i = 0; i < tmpBlocks.Count; i++)
            {
                Block block = tmpBlocks[i];

                bool existsInLevel = false;
                for (int m = 0; m < level.Count; m++)
                {
                    if (block.blockIndex ==level[m].blockIndex)
                    {
                        existsInLevel = true;
                        break;
                    }
                }
                if (existsInLevel)
                    continue;

                if (block.startSize == previousBlock.endSize)
                {
                    availableBlocks.Add(block);
                }
            }
        }

        if (availableBlocks.Count <= 0)
        {
            Debug.LogError("Available block count is 0");
            return null;
        }

        Block randomBlock = null;
        if (seed != -1)
            randomBlock = availableBlocks[rnd.Next(0, availableBlocks.Count)];
        else
            randomBlock = availableBlocks[Random.Range(0, availableBlocks.Count)];

        Transform newBlock = Instantiate(randomBlock.transform, Vector3.zero, Quaternion.identity) as Transform;
        Block spawnedBlock = newBlock.GetComponent<Block>();

        ConnectBlocks(spawnedBlock, previousBlock);
        spawnedBlock.transform.parent = transform;
        return spawnedBlock;
    }
    private void ConnectBlocks(Block block, Block previousBlock)
    {
        if (previousBlock == null)
        {
            block.transform.position = menuTutorialBlock.position;
        }
        else
        {
            block.transform.position = previousBlock.transform.position;

            Vector3 diff = block.GetStartPosition - previousBlock.GetEndPosition;
            block.transform.position -= diff;
        }
        
    }
    public Vector3 GetPlayerSpawnPosition(int player)
    {
        if (tutorialBlock.frogCount != GetTutorialFrogCount())
        {
            TutorialBlock tmp = tutorialBlock;

            GameMode mode = GameManager.CurrentGameMode;
            Block b = FindBlock(mode.tutorialBlock);
            tutorialBlock = (TutorialBlock)b;
            tutorialBlock.transform.position = tmp.transform.position;

            Destroy(tmp.gameObject);
            level[0] = tutorialBlock;
        }

        if (tutorialBlock)
            return tutorialBlock.playerStartPosition[player - 1].position;
        return Vector3.zero;
    }
    public SpriteRenderer[] GetReadySetGoSpriteRenderes()
    {
        SpriteRenderer[] renderers = new SpriteRenderer[3];

        renderers[0] = tutorialBlock.red;
        renderers[1] = tutorialBlock.yellow;
        renderers[2] = tutorialBlock.green;

        return renderers;
    }
    public void ActivateFirstBlock()
    {
        firstBlock.SetActive(true);
    }
    public void DeactivateFirstBlock()
    {
        firstBlock.SetActive(false);
    }
    private TutorialFrogCount GetTutorialFrogCount()
    {
        switch (gameManager.GetFrogReadyCount())
        {
            case 1:
                return TutorialFrogCount.One;
            case 2:
                return TutorialFrogCount.Two;
            case 3:
                return TutorialFrogCount.Three;
            case 4:
                return TutorialFrogCount.Four;
            default:
                return TutorialFrogCount.Invalid;
        }
    }

    private void LoadLevelBlocks()
    {
        Block[] blocks = null;

        //Load easy blocks
        blocks = Resources.LoadAll<Block>("Prefabs/Designed Blocks/Easy blocks");
        for (int i = 0; i < blocks.Length; i++)
        {
            globalBlockCollection.Add(blocks[i]);
        }

        //Load medium blocks
        blocks = Resources.LoadAll<Block>("Prefabs/Designed Blocks/Medium blocks");
        for (int i = 0; i < blocks.Length; i++)
        {
            globalBlockCollection.Add(blocks[i]);
        }

        //Load hard blocks
        blocks = Resources.LoadAll<Block>("Prefabs/Designed Blocks/Hard blocks");
        for (int i = 0; i < blocks.Length; i++)
        {
            globalBlockCollection.Add(blocks[i]);
        }

        //Load top blocks
        blocks = Resources.LoadAll<Block>("Prefabs/Designed Blocks/Top blocks");
        for (int i = 0; i < blocks.Length; i++)
        {
            globalBlockCollection.Add(blocks[i]);
        }

        //Load tutorial blocks
        blocks = Resources.LoadAll<Block>("Prefabs/Designed Blocks/Tutorial blocks");
        for (int i = 0; i < blocks.Length; i++)
        {
            globalBlockCollection.Add(blocks[i]);
        }

        //Load converter blocks
        blocks = Resources.LoadAll<Block>("Prefabs/Designed Blocks/Converter blocks");
        for (int i = 0; i < blocks.Length; i++)
        {
            globalBlockCollection.Add(blocks[i]);
        }

        //Load converter blocks
        blocks = Resources.LoadAll<Block>("Prefabs/Designed Blocks/Hjulafton");
        for (int i = 0; i < blocks.Length; i++)
        {
            globalBlockCollection.Add(blocks[i]);
        }

        //Assign indexes for each block
        for (int i = 0; i < globalBlockCollection.Count; i++)
        {
            globalBlockCollection[i].blockIndex = i;
        }
    }
    public void SetLevelHeight()
    {
        GameManager.LevelHeight = LevelHeight;
        FixSigns();
    }
    public Vector3 GetTopPosition()
    {
        if (level.Count > 0)
            return level.Last().GetEndCenterPosition;

        return Vector3.zero;
    }
    public void DestroyLevel(bool keepTutorial)
    {
        if (level.Count <= 0)
        {
            if (keepTutorial)
            {
                Block block = FindBlock(GameManager.CurrentGameMode.tutorialBlock);
                block.transform.parent = transform; level.Add(block);
                tutorialBlock = block as TutorialBlock;
            }
            return;
        }

        for (int i = 0; i < level.Count; i++)
        {
            if (keepTutorial && i == 0)
                continue;

            DestroyImmediate(level[i].gameObject);
        }

        Block b = level[0];
        level.Clear();
        if (keepTutorial)
            level.Add(b);
    }
}
