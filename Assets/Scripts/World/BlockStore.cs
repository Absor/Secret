using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockStore : MonoBehaviour {

    private Dictionary<BlockType, Block> blocks;

    void Awake()
    {
        blocks = new Dictionary<BlockType, Block>();

        blocks.Add(BlockType.Air, new Block());
        blocks[BlockType.Air].render = false;
        blocks.Add(BlockType.Grass, new Block());
        blocks[BlockType.Grass].render = true;
    }

    public Block GetBlock(BlockType blockType) {
        return blocks[blockType];
    }
}
