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

    private Transform currentTop;
    private Transform previousTop;
    private GameObject easyBlock;
    private float movementWidth;

    public int numberOfHardBlocks = 1;
    public int numberOfMediumBlocks = 1;
    public int numberOfEasyBlocks = 1;

    public float LevelHeight { get { return (numberOfEasyBlocks + numberOfMediumBlocks + numberOfHardBlocks + 1); } }

    void Awake()
    {
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

        Generate();
	}
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Generate();
        }
    }
    public void Generate()
    {
        //Remove previouslevel
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

        block.transform.position = new Vector3(0, block.transform.position.y);
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
                //while (true)
                //{
                //    ////if (block.start == BlockEnding.AB)
                //    ////{
                //    ////    break;
                //    ////}
                //    ////else
                //    ////{
                //    ////    DestroyImmediate(block.transform.gameObject);
                //    ////    block = GetBlock(level.Last().GetComponent<Block>(), BlockDifficulty.Easy);
                //    ////}
                //}
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
            //Search for a TOP block
            if (previousBlock == null) 
            {
                if (blockList[i].difficulty == BlockDifficulty.Top)
                    foundBlocks.Add(blockList[i]);
            }
            //Search for other block
            else 
            {
                //If the searched difficulty matches
                if (blockList[i].difficulty == difficulty)
                {
                    if (previousBlock.start == BlockEnding.Thick)
                    {
                        if (blockList[i].end == BlockEnding.Thick)
                        {
                            foundBlocks.Add(blockList[i]);
                        }
                    }
                    else if (blockList[i].end != BlockEnding.Thick)
                    {
                        foundBlocks.Add(blockList[i]);
                    }
                }
            }
        }




        if (foundBlocks.Count > 0)
        {
            //Remove blocks that was in previous level
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

            //Return TOP block if TOP was searched for
            if (previousBlock == null)
            {
                Transform top = Instantiate(foundBlocks[Random.Range(0, foundBlocks.Count)].transform, Vector3.zero, Quaternion.identity) as Transform;
                return top.GetComponent<Block>();
            }

            Transform t = Instantiate(foundBlocks[Random.Range(0, foundBlocks.Count)].transform, previousBlock.transform.position, Quaternion.identity) as Transform;
            Block b = t.GetComponent<Block>();

            //Change position of new block here!
            Vector3 diff = b.GetEndPosition - previousBlock.GetStartPosition;
            b.transform.position -= diff;
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
