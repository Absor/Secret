using System;
using UnityEngine;

[RequireComponent(typeof(ChunkRenderer))]
public class Chunk : MonoBehaviour
{
    public readonly int x;
    public readonly int y;
    public readonly int z;

    public readonly int sizeX;
    public readonly int sizeY;
    public readonly int sizeZ;

    private Block[][][] blocks;

    #region Setters and getters for surrounding chunks

    private Chunk nextX, prevX;
    private Chunk nextY, prevY;
    private Chunk nextZ, prevZ;

    public Chunk NextX
    {
        set
        {
            nextX = value;
            if (value.prevX == null)
            {
                value.prevX = this;
            }
        }
        get
        {
            return nextX;
        }
    }

    public Chunk PrevX
    {
        set
        {
            prevX = value;
            if (value.nextX == null)
            {
                value.nextX = this;
            }
        }
        get
        {
            return prevX;
        }
    }

    public Chunk NextY
    {
        set
        {
            nextY = value;
            if (value.prevY == null)
            {
                value.prevY = this;
            }
        }
        get
        {
            return nextY;
        }
    }

    public Chunk PrevY
    {
        set
        {
            prevY = value;
            if (value.nextY == null)
            {
                value.nextY = this;
            }
        }
        get
        {
            return prevY;
        }
    }

    public Chunk NextZ
    {
        set
        {
            nextZ = value;
            if (value.prevZ == null)
            {
                value.prevZ = this;
            }
        }
        get
        {
            return nextZ;
        }
    }

    public Chunk PrevZ
    {
        set
        {
            prevZ = value;
            if (value.nextZ == null)
            {
                value.nextZ = this;
            }
        }
        get
        {
            return prevZ;
        }
    }

    #endregion

    //public BlockChunk(int x, int y, int z, int sizeX, int sizeY, int sizeZ)
    //{
    //    this.x = x;
    //    this.y = y;
    //    this.z = z;
    //    this.sizeX = sizeX;
    //    this.sizeY = sizeY;
    //    this.sizeZ = sizeZ;

    //    setupBlockChunk();
    //}

    private void setupBlockChunk() {
        blocks = new Block[sizeX][][];
        for (int i = 0; i < sizeX; i++)
        {
            blocks[i] = new Block[sizeY][];
            for (int j = 0; j < sizeY; j++)
            {
                blocks[i][j] = new Block[sizeZ];
            }
        }
    }

    public void SetBlock(Block block, int x, int y, int z)
    {
        blocks[x][y][z] = block;
    }

    public Block GetBlock(int x, int y, int z) {
        return blocks[x][y][z];
    }

    internal void Initialize(World world, int worldX, int worldY, int worldZ, int chunkSizeX, int chunkSizeY, int chunkSizeZ)
    {
    }
}
