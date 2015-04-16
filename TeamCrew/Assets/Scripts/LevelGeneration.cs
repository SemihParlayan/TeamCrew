using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelGeneration : MonoBehaviour 
{
    public Block tutorialBlock;
    public List<Block> easyBlocks = new List<Block>();
    public List<Block> mediumBlocks = new List<Block>();
    public List<Block> hardBlocks = new List<Block>();
    public List<Block> topBlocks = new List<Block>();

    public Vector3 blockHeight;

	void Start () 
    {
        SpriteRenderer renderer = tutorialBlock.transform.GetComponent<SpriteRenderer>();
        blockHeight = new Vector3(0, renderer.sprite.rect.height / renderer.sprite.pixelsPerUnit);
        blockHeight -= new Vector3(0, 0.15f);
        Generate();
	}

    void Generate()
    {
        //Easy
        Block b = GetBlock(tutorialBlock, ref easyBlocks);
        if (b == null)
            return;
        Transform block = Instantiate(b.transform, tutorialBlock.transform.position + blockHeight, Quaternion.identity) as Transform;
        block.name = "Easy";
        block.parent = transform;

        b = GetBlock(b, ref mediumBlocks);
        if (b == null)
            return;
        block = Instantiate(b.transform, block.transform.position + blockHeight, Quaternion.identity) as Transform;
        block.name = "Medium";
        block.parent = transform;

        b = GetBlock(b, ref hardBlocks);
        if (b == null)
            return;
        block = Instantiate(b.transform, block.transform.position + blockHeight, Quaternion.identity) as Transform;
        block.name = "Hard";
        block.parent = transform;

        b = GetBlock(b, ref topBlocks);
        if (b == null)
            return;
        block = Instantiate(b.transform, block.transform.position + blockHeight, Quaternion.identity) as Transform;
        block.name = "Top";
        block.parent = transform;
    }

    Block GetBlock(Block previousBlock, ref List<Block> list)
    {
        List<Block> foundBlocks = new List<Block>();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].start == previousBlock.end)
            {
                if (list[i].start == previousBlock.start && list[i].end == previousBlock.end)
                    continue;

                foundBlocks.Add(list[i]);
            }
        }

        if (foundBlocks.Count > 0)
        {
            return foundBlocks[Random.Range(0, foundBlocks.Count)];
        }
        return null;
    }
}
