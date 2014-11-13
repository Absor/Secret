using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class World : InjectedMonoBehaviour
{
    [Inject]
    private BlockStore blockStore;

    public class BlockUpdateEvent : UnityEvent<int, int, int> { }
    public readonly BlockUpdateEvent OnBlockUpdate = new BlockUpdateEvent();
    public readonly UnityEvent OnWorldCreated = new UnityEvent();

    private int worldSizeX, worldSizeY, worldSizeZ;
    public int WorldSizeX { get { return worldSizeX; } }
    public int WorldSizeY { get { return worldSizeY; } }
    public int WorldSizeZ { get { return worldSizeZ; } }

    private Block[][][] blocks;

    public void SetupNewWorld(int worldSizeX, int worldSizeY, int worldSizeZ)
    {
        this.worldSizeX = worldSizeX;
        this.worldSizeY = worldSizeY;
        this.worldSizeZ = worldSizeZ;

        blocks = new Block[worldSizeX][][];
        for (int x = 0; x < worldSizeX; x++)
        {
            blocks[x] = new Block[worldSizeY][];
            for (int y = 0; y < worldSizeY; y++)
            {
                blocks[x][y] = new Block[worldSizeZ];
            }
        }
    }

    public void SetBlock(int x, int y, int z, Block block)
    {
        if (indicesInvalid(x, y, z))
        {
            return;
        }
        blocks[x][y][z] = block;
        OnBlockUpdate.Invoke(x, y, z);
    }

    public Block GetBlock(int x, int y, int z)
    {
        if (indicesInvalid(x, y, z))
        {
            return blockStore.GetBlock(BlockType.Air);
        }
        return blocks[x][y][z];
    }

    private bool indicesInvalid(int x, int y, int z)
    {
        return x < 0 || x >= worldSizeX || y < 0 || y >= worldSizeY || z < 0 || z >= WorldSizeZ;
    }
}
