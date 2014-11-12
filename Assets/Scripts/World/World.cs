using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class World : MonoBehaviour
{
    public class BlockUpdateEvent : UnityEvent<int, int, int> { }
    public readonly BlockUpdateEvent OnBlockUpdate = new BlockUpdateEvent();

    private int worldSizeX, worldSizeY, worldSizeZ;
    public int WorldSizeX { get { return worldSizeX; } }
    public int WorldSizeY { get { return worldSizeY; } }
    public int WorldSizeZ { get { return worldSizeZ; } }

    private Block[] blocks;

    public void SetupNewWorld(int worldSizeX, int worldSizeY, int worldSizeZ)
    {
        this.worldSizeX = worldSizeX;
        this.worldSizeY = worldSizeY;
        this.worldSizeZ = worldSizeZ;
        blocks = new Block[worldSizeX * worldSizeY * worldSizeZ];
    }

    public void SetBlock(int x, int y, int z, Block block)
    {
        if (isInvalidPosition(x, y, z))
        {
            return;
        }
        blocks[getArrayIndex(x, y, z)] = block;
        OnBlockUpdate.Invoke(x, y, z);
    }

    public Block GetBlock(int x, int y, int z)
    {
        if (isInvalidPosition(x, y, z))
        {
            return null;
        }
        return blocks[getArrayIndex(x, y, z)];
    }

    private int getArrayIndex(int x, int y, int z)
    {
        return (x * worldSizeX + y) * worldSizeY + z;
    }

    private bool isInvalidPosition(int x, int y, int z)
    {
        return x < 0 || x >= worldSizeX || y < 0 || y >= worldSizeY || z < 0 || z >= worldSizeZ;
    }
}
