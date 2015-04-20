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
        Generate();
	}

    public void Generate()
    {
        for (int i = 0; i < level.Count; i++)
        {
            Destroy(level[i].gameObject);
        }
        level.Clear();

        Block block = GetBlock(tutorialBlock, BlockDifficulty.Easy);
        block.transform.name = "Easy"; block.transform.parent = transform; level.Add(block.transform);

        block = GetBlock(block, BlockDifficulty.Medium);
        block.transform.name = "Medium"; block.transform.parent = transform; level.Add(block.transform);

        block = GetBlock(block, BlockDifficulty.Hard);
        block.transform.name = "Hard"; block.transform.parent = transform; level.Add(block.transform);

        block = GetBlock(block, BlockDifficulty.Top);
        block.transform.name = "Top"; block.transform.parent = transform;

        previousTop = currentTop;

        currentTop = block.transform;

        if (previousTop)
        {
            previousTop.parent = null;
            //float diffX = previousTop.position.x - transform.position.x;

           // transform.position += new Vector3(diffX, 0);

            //Destroy(previousTop.gameObject);
        }

        Vector3 camPos = Camera.main.transform.position;
        camPos.x = currentTop.position.x;
        Camera.main.transform.position = camPos;
    }

    Block GetBlock(Block previousBlock, BlockDifficulty difficulty)
    {
        List<Block> foundBlocks = new List<Block>();
        for (int i = 0; i < blockList.Count; i++)
        {
            if (blockList[i].difficulty != difficulty)
                continue;

            if (previousBlock.end == BlockEnding.AB)
            {
                if (blockList[i].start == BlockEnding.AB)
                {
                    foundBlocks.Add(blockList[i]);
                }
            }
            else if (blockList[i].start != BlockEnding.AB)
            {
                foundBlocks.Add(blockList[i]);
            }
        }

        if (foundBlocks.Count > 0)
        {
            Transform t = Instantiate(foundBlocks[Random.Range(0, foundBlocks.Count)].transform, previousBlock.transform.position, Quaternion.identity) as Transform;
            Block b = t.GetComponent<Block>();

            if (previousBlock.end == BlockEnding.B && b.start == BlockEnding.A)
            {
                b.transform.position += new Vector3(6, 0);
            }
            else if (previousBlock.end == BlockEnding.A && b.start == BlockEnding.B)
            {
                b.transform.position += new Vector3(-6, 0);
            }

            b.transform.position += new Vector3(0, blockSize);
            return b;
        }
        return null;
    }
}
