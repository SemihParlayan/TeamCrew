using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelGeneration : MonoBehaviour 
{
    public Transform menuTutorialBlock;
    private List<Block> blockList = new List<Block>();
    public List<Block> level = new List<Block>();
    private List<Block> previousLevel = new List<Block>();

    private GameObject easyBlock;
    private TutorialBlock tutorialBlock;
    private GameManager gameManager;

    public int numberOfHardBlocks = 1;
    public int numberOfMediumBlocks = 1;
    public int numberOfEasyBlocks = 1;

    public bool lockComplete;
    private int minimumSlotIndex = 1;
    private int currentSlotIndex = 1;
    public List<Block> tmpBlocks = new List<Block>();

    public float LevelHeight 
    { 
        get 
        { 
            if (!menuTutorialBlock)
                return 0;

            //-16 är 8 på tom top och 8 på tom tutorial
            return (level.Last().GetEndPosition - menuTutorialBlock.GetComponent<Block>().GetStartCenterPosition).y - 16;
            //return (menuTutorialBlock.transform.position.y - tut.size.y / 2) + 7.2f;
        } 
    }

	void Start () 
    {
        LoadLevelBlocks();


        if (menuTutorialBlock == null)
        {
            Debug.LogError("Assign menu tutorial block to levelgeneration");
        }
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
            GenerateFullMountain();
        }
    }
    public void GenerateFullMountain(bool keepTutorial = false)
    {
        //Remove previouslevel
        DestroyLevel(keepTutorial);

        //Spawn Tutorial
        Block block = null;
        if (!keepTutorial)
        {
            block = CreateNewBlock(null, GetTutorialDifficulty());
            block.transform.parent = transform; level.Add(block);
            tutorialBlock = block as TutorialBlock;
        }
        else
        {
            block = tutorialBlock;
        }
		

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
        previousLevel.AddRange(level);
    }
    public void GenerateMountainSlotmachineStyle()
    {
        GenerateFullMountain(true);
        lockComplete = false;
        minimumSlotIndex = 1;
        currentSlotIndex = 1;

        for (int i = 0; i < level.Count; i++)
        {
            tmpBlocks.Add(level[i]);
            level[i].gameObject.SetActive(false);
        }
        level[0].gameObject.SetActive(true);

        SetLevelHeight();
        InvokeRepeating("IncrementSlotMachine", 0, 0.05f);
        InvokeRepeating("LockSlotMachine", 0, 2f);
    }
    private void IncrementSlotMachine()
    {

        Block prev = tmpBlocks[currentSlotIndex - 1];
        Block newBlock = CreateNewBlock(prev, tmpBlocks[currentSlotIndex].difficulty);

        DestroyImmediate(tmpBlocks[currentSlotIndex].gameObject);
        tmpBlocks[currentSlotIndex] = newBlock;

        currentSlotIndex++;
        if (currentSlotIndex >= level.Count)
            currentSlotIndex = minimumSlotIndex;
    }
    private void LockSlotMachine()
    {
        minimumSlotIndex++;

        if (minimumSlotIndex >= level.Count)
        {
            CancelInvoke();
            level.Clear();
            level.AddRange(tmpBlocks);
            tmpBlocks.Clear();

            previousLevel.Clear();
            previousLevel.AddRange(level);
            lockComplete = true;
        }
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

    public Block CreateNewBlock(Block previousBlock, BlockDifficulty difficulty, bool overrideBlockmatch = false)
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
            if (availableBlocks[i].difficulty.ToString().Contains("Tutorial"))
            {
                continue;
            }

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
    public void SetLevelHeight()
    {
        GameManager.LevelHeight = LevelHeight;
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
                Block block = CreateNewBlock(null, GetTutorialDifficulty());
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
