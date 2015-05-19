using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelGeneration : MonoBehaviour 
{
    public Transform startTop;
    public List<Block> blockList = new List<Block>();
    public List<Sprite> signSprites = new List<Sprite>();
    public List<Block> level = new List<Block>();
    public List<Block> lastLevel = new List<Block>();

    private float blockSize;

    private Transform currentTop;
    private Transform previousTop;
    private GameObject easyBlock;
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
            level.Add(startTop.GetComponent<Block>());
        else
            Debug.LogError("Assign a START-TOP on GameMaster!");


        for (int i = 0; i < blockList.Count; i++)
        {
            blockList[i].blockIndex = i;
        }
	}

    public void Generate()
    {
        for (int i = 0; i < level.Count; i++)
        {
            DestroyImmediate(level[i].gameObject);
        }
        level.Clear();

        //Spawn top block
        Block block = GetBlock(null, BlockDifficulty.Top);
        block.transform.parent = transform;
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
            DestroyImmediate(previousTop.gameObject);
        }

        //Spawn Hard Blocks
        for (int i = 0; i < numberOfHardBlocks; i++)
        {
            block = GetBlock(block, BlockDifficulty.Hard);                
            block.transform.parent = transform; level.Add(block);
        }

        //Spawn Medium Blocks
        for (int i = 0; i < numberOfMediumBlocks; i++)
        {
            block = GetBlock(block, BlockDifficulty.Medium);
            block.transform.parent = transform; level.Add(block);
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
                        DestroyImmediate(block.transform.gameObject);
                        block = GetBlock(level.Last().GetComponent<Block>(), BlockDifficulty.Easy);
                    }
                }
            }
            block.transform.parent = transform; level.Add(block);
        }
        easyBlock = block.gameObject;

        //Spawn Tutorial
        block = GetBlock(block, BlockDifficulty.Tutorial);
        block.transform.parent = transform; level.Add(block);

        FixSigns();

        lastLevel.Clear();

        for (int i = 0; i < level.Count; i++)
        {
            lastLevel.Add(level[i]);
        }
        
    }
    private void FixSigns()
    {
        GameObject[] signs = GameObject.FindGameObjectsWithTag("Sign");

        if (signs.Length != signSprites.Count)
        {
            Debug.LogWarning("Number of signs found is not equal to sign sprites applied to LevelGeneration.cs" +
                " \n Signs found: " + signs.Length + " \n Sprites applied: " + signSprites.Count
                );
            return;
        }
        for (int j = 0; j < signSprites.Count; j++)
        {
            int maxY = int.MaxValue;
            int index = -1;
            for (int i = 0; i < signs.Length; i++)
            {
                float y = signs[i].transform.position.y;
                if (y < maxY)
                {
                    index = i;
                }
            }

            signs[j].GetComponent<SpriteRenderer>().sprite = signSprites[j];
        }
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
            for (int j = 0; j < foundBlocks.Count; j++)
            {
                for (int i = 0; i < lastLevel.Count; i++)
                {
                    if (foundBlocks[j].blockIndex == lastLevel[i].blockIndex)
                    {
                        foundBlocks.Remove(foundBlocks[j]);
                        break;
                    }
                }
            }

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

    public Vector3 GetPlayerOneSpawnPosition()
    {
        return level.Last().GetComponent<TutorialBlock>().playerOneStart.position;
    }
    public Vector3 GetPlayerTwoSpawnPosition()
    {
        return level.Last().GetComponent<TutorialBlock>().playerTwoStart.position;
    }
    public SpriteRenderer[] GetReadySetGoSpriteRenderes()
    {
        SpriteRenderer[] renderers = new SpriteRenderer[3];
        TutorialBlock tut = level.Last().GetComponent<TutorialBlock>();

        renderers[0] = tut.red;
        renderers[1] = tut.yellow;
        renderers[2] = tut.green;

        return renderers;
    }
    public void ActivateEasyBlock()
    {
        easyBlock.SetActive(true);
    }
    public void DeactivateEasyBlock()
    {
        easyBlock.SetActive(false);
    }
}
