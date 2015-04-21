﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelGeneration : MonoBehaviour 
{
    public Transform startTop;
    public List<Block> blockList = new List<Block>();

    private List<Transform> level = new List<Transform>();

    private float blockSize;

    private Transform currentTop;
    private Transform previousTop;
    private float movementWidth;

    public int numberOfHardBlocks = 1;
    public int numberOfMediumBlocks = 1;
    public int numberOfEasyBlocks = 1;

    public float LevelHeight { get { return (numberOfEasyBlocks + numberOfMediumBlocks + numberOfHardBlocks + 1) * blockSize; } }

    void Awake()
    {
        blockSize = -19.2f;
        GameManager.LevelHeight = LevelHeight - 5;
    }

	void Start () 
    {
        if (startTop)
            level.Add(startTop);
        else
            Debug.LogError("Assign a START-TOP on GameMaster!");
	}

    public void Generate()
    {
        for (int i = 0; i < level.Count; i++)
        {
            Destroy(level[i].gameObject);
        }
        level.Clear();


        //Spawn top block
        Block block = GetBlock(null, BlockDifficulty.Top);
        block.transform.name = "Top"; block.transform.parent = transform;
        previousTop = currentTop;
        currentTop = block.transform;

        float x = 0;
        if (block.start == BlockEnding.A)
            x = 3;
        else if (block.start == BlockEnding.B)
            x = -3;
        //transform.position = new Vector3(x, transform.position.y);
        block.transform.position = new Vector3(x, block.transform.position.y);
        if (previousTop)
        {
            previousTop.parent = null;
            Destroy(previousTop.gameObject);
        }

        //Spawn Hard Blocks
        for (int i = 0; i < numberOfHardBlocks; i++)
        {
            block = GetBlock(block, BlockDifficulty.Hard);                
            block.transform.name = "Hard"; block.transform.parent = transform; level.Add(block.transform);
        }

        //Spawn Medium Blocks
        for (int i = 0; i < numberOfMediumBlocks; i++)
        {
            block = GetBlock(block, BlockDifficulty.Medium);
            block.transform.name = "Medium"; block.transform.parent = transform; level.Add(block.transform);
        }

        //Spawn Easy Blocks
        for (int i = 0; i < numberOfEasyBlocks; i++)
        {
            block = GetBlock(block, BlockDifficulty.Easy);

            if (i == numberOfEasyBlocks - 1)
            {
                while (true)
                {
                    if (block.start == BlockEnding.AB)
                    {
                        break;
                    }
                    else
                    {
                        Destroy(block.transform.gameObject);
                        block = GetBlock(level.Last().GetComponent<Block>(), BlockDifficulty.Easy);
                    }
                }
            }
            block.transform.name = "Easy"; block.transform.parent = transform; level.Add(block.transform);
        }

        //Spawn Tutorial
        block = GetBlock(block, BlockDifficulty.Tutorial);
        block.transform.name = "Tutorial"; block.transform.parent = transform; level.Add(block.transform);
    }

    Block GetBlock(Block previousBlock, BlockDifficulty difficulty)
    {
        List<Block> foundBlocks = new List<Block>();

        for (int i = 0; i < blockList.Count; i++)
        {
            if (previousBlock == null)
            {
                if (blockList[i].difficulty == BlockDifficulty.Top)
                    foundBlocks.Add(blockList[i]);
            }
            else
            {
                if (blockList[i].difficulty != difficulty)
                    continue;

                if (previousBlock.start == BlockEnding.AB)
                {
                    if (blockList[i].end == BlockEnding.AB)
                    {
                        foundBlocks.Add(blockList[i]);
                    }
                }
                else if (blockList[i].end != BlockEnding.AB)
                {
                    foundBlocks.Add(blockList[i]);
                }
            }
        }

        if (foundBlocks.Count > 0)
        {
            if (previousBlock == null)
            {
                Transform top = Instantiate(foundBlocks[Random.Range(0, foundBlocks.Count)].transform, Vector3.zero, Quaternion.identity) as Transform;
                return top.GetComponent<Block>();
            }

            Transform t = Instantiate(foundBlocks[Random.Range(0, foundBlocks.Count)].transform, previousBlock.transform.position, Quaternion.identity) as Transform;
            Block b = t.GetComponent<Block>();

            if (previousBlock.start == BlockEnding.B && b.end == BlockEnding.A)
            {
                b.transform.position += new Vector3(6, 0);
            }
            else if (previousBlock.start == BlockEnding.A && b.end == BlockEnding.B)
            {
                b.transform.position += new Vector3(-6, 0);
            }

            b.transform.position += new Vector3(0, blockSize);
            return b;
        }

        Debug.LogError("Error finding a " + difficulty.ToString() + " Block!");
        return null;
    }
}
