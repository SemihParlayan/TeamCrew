using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelGeneration : MonoBehaviour 
{
    private List<Block> blockList = new List<Block>();
    private List<Block> level = new List<Block>();
    private List<Block> previousLevel = new List<Block>();

    private GameObject easyBlock;
    private TutorialBlock tutorialBlock;
    private GameManager gameManager;

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

        for (int i = 0; i < blockList.Count; i++)
        {
            blockList[i].blockIndex = i;
        }

        gameManager = GetComponent<GameManager>();
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
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

        //Spawn Tutorial
        Block block = CreateNewBlock(null, GetTutorialDifficulty());
        block.transform.parent = transform; level.Add(block);
        tutorialBlock = block as TutorialBlock;

        //Spawn Easy Blocks
        for (int i = 0; i < numberOfEasyBlocks; i++)
        {
            if (i == 0)
            {
                block = CreateNewBlock(block, BlockDifficulty.Easy, true);
                
                if (block.start == BlockEnding.Thin)
                {
                    block.transform.position = Vector3.zero;

                    Block converter = CreateNewBlock(level[0], BlockDifficulty.Converter, true);
                    level.Add(converter);

                    ConnectBlocks(block, converter);

                    easyBlock = converter.gameObject;
                }
                else
                {
                    easyBlock = block.gameObject;
                }
                level.Add(block);
            }
            else
            {
                block = CreateNewBlock(block, BlockDifficulty.Easy);
                level.Add(block);
            }

        }

        //Spawn Medium Blocks
        for (int i = 0; i < numberOfMediumBlocks; i++)
        {
            block = CreateNewBlock(block, BlockDifficulty.Medium);
            level.Add(block);
        }

        //Spawn Hard Blocks
        for (int i = 0; i < numberOfHardBlocks; i++)
        {
            block = CreateNewBlock(block, BlockDifficulty.Hard);
            level.Add(block);
        }

        //Spawn top block
        block = CreateNewBlock(block, BlockDifficulty.Top);
        level.Add(block);

        FixSigns();

        //Save current level inside previousLevel
        previousLevel.Clear();
        for (int i = 0; i < level.Count; i++)
        {
            previousLevel.Add(level[i]);
        }

        //Set height of map
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

    Block CreateNewBlock(Block previousBlock, BlockDifficulty difficulty, bool overrideBlockmatch = false)
    {
        List<Block> availableBlocks = new List<Block>();

        for (int i = 0; i < blockList.Count; i++)
        {
            //If the searched difficulty matches
            if (blockList[i].difficulty == difficulty)
            {
                Block newBlock = blockList[i];

                //Does it fit with previousblock
                if (!overrideBlockmatch && previousBlock != null)
                {
                    if (newBlock.start != previousBlock.end)
                    {
                        continue;
                    }
                }


                bool add = true;
                for (int j = 0; j < level.Count; j++)
                {
                    if (level[j].blockIndex == newBlock.blockIndex)
                    {
                        add = false;
                        break;
                    }
                }
                if (add)
                {
                    availableBlocks.Add(newBlock);
                }
            }
        }




        if (availableBlocks.Count == 0)
        {
            Debug.LogError("Could not find a " + difficulty.ToString() + " block to generate!");
            return null;
        }

        //Remove blocks that was in previous level
        for (int i = 0; i < availableBlocks.Count; i++)
        {
            for (int j = 0; j < previousLevel.Count; j++)
            {
                if (availableBlocks[i].blockIndex == previousLevel[j].blockIndex)
                {
                    availableBlocks.Remove(availableBlocks[i]);
                    break;
                }
            }
        }

        if (availableBlocks.Count == 0)
        {
            Debug.LogError("Could not find a " + difficulty.ToString() + " block to generate. Reason: All available blocks deleted because they were in previous level!");
            return null;
        }


        Transform t = Instantiate(availableBlocks[Random.Range(0, availableBlocks.Count)].transform, Vector3.zero, Quaternion.identity) as Transform;
        Block b = t.GetComponent<Block>();

        ConnectBlocks(b, previousBlock);
        b.transform.parent = transform;
        return b;
    }
    private void ConnectBlocks(Block block, Block previousBlock)
    {
        if (previousBlock == null)
        {
            block.transform.position = Vector3.zero;
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
        return tutorialBlock.playerStartPosition[player - 1].position;
        //return level.Last().GetComponent<TutorialBlock>().playerStartPosition[player - 1].position;
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
    private BlockDifficulty GetTutorialDifficulty()
    {
        switch(gameManager.GetFrogReadyCount())
        {
            case 1:
                return BlockDifficulty.Tutorial_1player;
            case 2:
                return BlockDifficulty.Tutorial_2player;
            case 3:
                return BlockDifficulty.Tutorial_3player;
            case 4:
                return BlockDifficulty.Tutorial_4player;
            default:
                return BlockDifficulty.Tutorial_1player;
        }
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
