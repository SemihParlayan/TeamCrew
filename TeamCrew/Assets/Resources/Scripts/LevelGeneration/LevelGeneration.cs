using UnityEngine;
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
        if (Input.GetKeyDown(KeyCode.B))
        {
            GenerateFullMountain(false);
        }
    }
    public void GenerateFullMountain(bool keepTutorial = false)
    {
        GameMode mode = GameManager.CurrentGameMode;
        Debug.Log(mode.name);
        //Remove previouslevel
        DestroyLevel(keepTutorial);

        //Spawn Tutorial
        Block block = null;
        if (!keepTutorial)
        {
            block = FindBlock(mode.tutorialBlock);
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
            TagCollection climbingBlock = mode.climbingBlocks[i];
            block = FindBlock(climbingBlock);

            //Add Converter if neccesary
            if (i == 0)
            {
                firstBlock = block.gameObject;

                if (block.startSize != tutorialBlock.endSize)
                {
                    TagCollection c = new TagCollection(); c.tags.Add(BlockTag.Converter);
                    Block converter = FindBlock(c);
                    level.Add(converter);
                    ConnectBlocks(block, converter);
                }
            }

            //Add climbing block
            level.Add(block);
        }

        //Spawn top block
        block = FindBlock(mode.topBlock);
        level.Add(block);
    }

    private void FixSigns()
    {
        GameObject[] signs = GameObject.FindGameObjectsWithTag("Sign");

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
    public Block FindBlock(TagCollection minimumRequiredTags)
    {
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

        //Sort through so that size fits except for tutorial and first block
        if (!searchingForTutorial)
        {
            List<Block> tmpBlocks = new List<Block>();
            tmpBlocks.AddRange(availableBlocks);
            availableBlocks.Clear();

            for (int i = 0; i < tmpBlocks.Count; i++)
            {
                Block block = tmpBlocks[i];
                if (level.Count <= 1)
                {
                    availableBlocks.Add(block);
                }
                else if (block.startSize == previousBlock.endSize)
                {
                    availableBlocks.Add(block);
                }
            }
        }

        Block randomBlock = availableBlocks[Random.Range(0, availableBlocks.Count)];
        Transform newBlock = Instantiate(randomBlock.transform, Vector3.zero, Quaternion.identity) as Transform;
        Block spawnedBlock = newBlock.GetComponent<Block>();

        ConnectBlocks(spawnedBlock, previousBlock);
        spawnedBlock.transform.parent = transform;
        return spawnedBlock;

        //for (int i = 0; i < globalBlockCollection.Count; i++)
        //{
        //    If the searched difficulty matches
        //    if (globalBlockCollection[i].difficulty == difficulty)
        //    {
        //        Block newBlock = globalBlockCollection[i];

        //        Does it fit with previousblock
        //        if (!overrideBlockmatch && previousBlock != null)
        //        {
        //            if (newBlock.start != previousBlock.end)
        //            {
        //                continue;
        //            }
        //        }


        //        bool add = true;
        //        for (int j = 0; j < level.Count; j++)
        //        {
        //            if (level[j].blockIndex == newBlock.blockIndex)
        //            {
        //                add = false;
        //                break;
        //            }
        //        }
        //        if (add)
        //        {
        //            availableBlocks.Add(newBlock);
        //        }
        //    }
        //}




        //if (availableBlocks.Count == 0)
        //{
        //    Debug.LogError("Could not find a " + difficulty.ToString() + " block to generate!");
        //    return null;
        //}


        //Transform t = Instantiate(availableBlocks[Random.Range(0, availableBlocks.Count)].transform, Vector3.zero, Quaternion.identity) as Transform;
        //Block b = t.GetComponent<Block>();

        //ConnectBlocks(b, previousBlock);
        //b.transform.parent = transform;
        return null;
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
