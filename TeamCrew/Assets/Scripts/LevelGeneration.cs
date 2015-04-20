using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelGeneration : MonoBehaviour 
{
    public Block tutorialBlock;
    public List<Block> blockList = new List<Block>();

    private List<Transform> level = new List<Transform>();

    public float blockSize;

    public Transform currentTop;
    public Transform previousTop;
    private float movementWidth;

	void Start () 
    {
        SpriteRenderer renderer = tutorialBlock.transform.GetComponent<SpriteRenderer>();
        blockSize = renderer.sprite.rect.height / renderer.sprite.pixelsPerUnit;
        blockSize *= -1;
        Generate();
	}

    public void Generate()
    {
        for (int i = 0; i < level.Count; i++)
        {
            Destroy(level[i].gameObject);
        }
        level.Clear();


        Block block = GetBlock(null, BlockDifficulty.Top);
        block.transform.name = "Top"; block.transform.parent = transform;
        previousTop = currentTop;
        currentTop = block.transform;

        if (previousTop)
        {
            previousTop.parent = null;
            float x = 0;
            if (block.start == BlockEnding.A)
                x = 3;
            else if (block.start == BlockEnding.B)
                x = -3;
            //transform.position = new Vector3(x, transform.position.y);
            block.transform.position = new Vector3(x, block.transform.position.y);
            Destroy(previousTop.gameObject);
        }

        block = GetBlock(block, BlockDifficulty.Hard);
        block.transform.name = "Hard"; block.transform.parent = transform; level.Add(block.transform);

        block = GetBlock(block, BlockDifficulty.Medium);
        block.transform.name = "Medium"; block.transform.parent = transform; level.Add(block.transform);

        block = GetBlock(block, BlockDifficulty.Easy);
        block.transform.name = "Easy"; block.transform.parent = transform; level.Add(block.transform);


        Vector3 camPos = Camera.main.transform.position;
        camPos.x = currentTop.position.x;
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
        return null;
    }
}
