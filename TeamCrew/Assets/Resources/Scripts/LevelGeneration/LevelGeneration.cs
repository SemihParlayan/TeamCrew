using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelGeneration : MonoBehaviour 
{
    public Transform startTop;
    public Block thickConverter;
    private List<Block> blockList = new List<Block>();
    private List<Block> level = new List<Block>();
    private List<Block> lastLevel = new List<Block>();

    private Transform currentTop;
    private Transform previousTop;
    private GameObject easyBlock;
    private float movementWidth;

    public int numberOfHardBlocks = 1;
    public int numberOfMediumBlocks = 1;
    public int numberOfEasyBlocks = 1;

    public float LevelHeight 
    { 
        get 
        { 
            Block tut = level.Last();
            if (!tut)
                return 0;
            return (tut.transform.position.y - tut.size.y / 2) + 7.2f;
        } 
    }

	void Start () 
    {
        LoadLevelBlocks();

        if (startTop)
            level.Add(startTop.GetComponent<Block>());
        else
            Debug.LogError("Assign a START-TOP on GameMaster!");

        if (!thickConverter)
            Debug.LogError("Assign a thickConverter to LevelGenerator");

        for (int i = 0; i < blockList.Count; i++)
        {
            blockList[i].blockIndex = i;
        }
	}

    private float timer = 0;
    void Update()
    {
        if (Input.GetKey(KeyCode.G))
        {
            timer += Time.deltaTime;
            if (timer >= 0.2f)
            {
                timer = 0;
                Generate();
            }
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
            block.transform.parent = transform; level.Add(block);

            if (i == numberOfEasyBlocks - 1)
            {
                if (block.start == BlockEnding.Thin)
                {
                    //Add converter
                    block = GetBlock(block, BlockDifficulty.Converter);
                    block.transform.parent = transform; level.Add(block);
                }
            }
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
        GameManager.LevelHeight = LevelHeight;
    }
    private void FixSigns()
    {
        GameObject[] signs = GameObject.FindGameObjectsWithTag("Sign");

        for (int i = 0; i < signs.Length; i++)
        {
            float y = Mathf.RoundToInt(Mathf.Abs(signs[i].transform.position.y));

            if (signs[i].transform.childCount == 0)
                Debug.LogError("Sign is missing a textmesh at: " + signs[i].transform.parent.name);

            TextMesh text = signs[i].transform.GetChild(0).GetComponent<TextMesh>();
            if (text)
            {
                text.text = y.ToString() + "m";
            }
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
                    Block found = null;

                    if (previousBlock.start == BlockEnding.Thick)
                    {
                        if (blockList[i].end == BlockEnding.Thick)
                        {
                            //foundBlocks.Add(blockList[i]);
                            found = blockList[i];
                        }
                    }
                    else if (blockList[i].end != BlockEnding.Thick)
                    {
                        //foundBlocks.Add(blockList[i]);
                        found = blockList[i];
                    }

                    if (found)
                    {
                        bool add = true;

                        for (int j = 0; j < level.Count; j++)
                        {
                            if (level[j].blockIndex == found.blockIndex)
                            {
                                add = false;
                                break;
                            }
                        }
                        if (add)
                        {
                            foundBlocks.Add(found);
                        }
                    }
                }
            }
        }




        if (foundBlocks.Count > 0)
        {
            //Remove blocks that was in previous level
            for (int j = 0; j < foundBlocks.Count; j++)
            {
                if (foundBlocks.Count < 2)
                {
                    Debug.Log("Using same block as before");
                    break;
                }

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

            Block blockToSpawn = foundBlocks[Random.Range(0, foundBlocks.Count)];
            if (blockToSpawn)
            {
                Transform t = Instantiate(foundBlocks[Random.Range(0, foundBlocks.Count)].transform, previousBlock.transform.position, Quaternion.identity) as Transform;
                Block b = t.GetComponent<Block>();

                //Change position of new block here!
                Vector3 diff = b.GetEndPosition - previousBlock.GetStartPosition;
                b.transform.position -= diff;
                return b;
            }
            else
            {
                Debug.LogError("Could not find any block to spawn in foundblocks.count");
            }
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

    private void LoadLevelBlocks()
    {
        Block[] blocks = null;

        //Load easy blocks
        if (numberOfEasyBlocks > 0)
        {
           blocks = Resources.LoadAll<Block>("Prefabs/Designed Blocks/Easy blocks");
           for (int i = 0; i < blocks.Length; i++)
           {
               blockList.Add(blocks[i]);
           }
        }

        //Load medium blocks
        if (numberOfMediumBlocks > 0)
        {
            blocks = Resources.LoadAll<Block>("Prefabs/Designed Blocks/Medium blocks");
            for (int i = 0; i < blocks.Length; i++)
            {
                blockList.Add(blocks[i]);
            }
        }

        //Load hard blocks
        if (numberOfHardBlocks > 0)
        {
            blocks = Resources.LoadAll<Block>("Prefabs/Designed Blocks/Hard blocks");
            for (int i = 0; i < blocks.Length; i++)
            {
                blockList.Add(blocks[i]);
            }
        }

        //Load top blocks
        blocks = Resources.LoadAll<Block>("Prefabs/Designed Blocks/Top blocks");
        for (int i = 0; i < blocks.Length; i++)
        {
            blockList.Add(blocks[i]);
        }

        //Load tutorial blocks
        blocks = Resources.LoadAll<Block>("Prefabs/Designed Blocks/Tutorial blocks");
        for (int i = 0; i < blocks.Length; i++)
        {
            blockList.Add(blocks[i]);
        }

        //Load converter blocks
        blocks = Resources.LoadAll<Block>("Prefabs/Designed Blocks/Converter blocks");
        for (int i = 0; i < blocks.Length; i++)
        {
            blockList.Add(blocks[i]);
        }
    }
}
