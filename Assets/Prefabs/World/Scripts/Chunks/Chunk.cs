using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    private World world;
    private int worldStartX, worldStartY, worldStartZ, chunkSizeX, chunkSizeY, chunkSizeZ;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private ChunkBlock[][][] chunkBlocks;

    private List<Vector3> newVertices = new List<Vector3>();
    private List<int> newTriangles = new List<int>();

    public void Initialize(World world, int chunkX, int chunkY, int chunkZ, int maxChunkSizeX, int maxChunkSizeY, int maxChunkSizeZ)
    {
        gameObject.name = string.Format("Chunk X: {0,-4} Y: {1,-4} Z: {2,-4}", chunkX, chunkY, chunkZ);
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();

        this.world = world;
        worldStartX = chunkX * maxChunkSizeX;
        worldStartY = chunkY * maxChunkSizeY;
        worldStartZ = chunkZ * maxChunkSizeZ;
        transform.position = new Vector3(worldStartX, worldStartY, worldStartZ);
        chunkSizeX = Mathf.Min(world.WorldSizeX - worldStartX, maxChunkSizeX);
        chunkSizeY = Mathf.Min(world.WorldSizeY - worldStartY, maxChunkSizeY);
        chunkSizeZ = Mathf.Min(world.WorldSizeZ - worldStartZ, maxChunkSizeZ);

        chunkBlocks = new ChunkBlock[chunkSizeX][][];
        for (int x = 0; x < chunkSizeX; x++)
        {
            chunkBlocks[x] = new ChunkBlock[chunkSizeY][];
            for (int y = 0; y < chunkSizeY; y++)
            {
                chunkBlocks[x][y] = new ChunkBlock[chunkSizeZ];
                for (int z = 0; z < chunkSizeZ; z++)
                {
                    chunkBlocks[x][y][z] = new ChunkBlock();
                }
            }
        }

        updateChunkBlocks();
        updateMesh();
    }

    private void updateChunkBlocks()
    {
        for (int x = 0; x < chunkSizeX; x++)
        {
            for (int y = 0; y < chunkSizeY; y++)
            {
                for (int z = 0; z < chunkSizeZ; z++)
                {
                    updateChunkBlock(x, y, z);
                }
            }
        }
    }

    private void updateChunkBlock(int x, int y, int z)
    {
        ChunkBlock chunkBlock = chunkBlocks[x][y][z];
        Block block = world.GetBlock(worldStartX + x, worldStartY + y, worldStartZ + z);
        if (block.Transparent)
        {
            chunkBlock.renderHighY = false;
            chunkBlock.renderLowX = false;
            chunkBlock.renderHighX = false;
            chunkBlock.renderLowZ = false;
            chunkBlock.renderHighZ = false;
        }
        else
        {
            chunkBlock.renderHighY = renderBlockHighY(x, y, z);
            chunkBlock.renderLowX = renderBlockLowX(x, y, z);
            chunkBlock.renderHighX = renderBlockHighX(x, y, z);
            chunkBlock.renderLowZ = renderBlockLowZ(x, y, z);
            chunkBlock.renderHighZ = renderBlockHighZ(x, y, z);
        }
    }

    private bool renderBlockHighZ(int x, int y, int z)
    {
        Block nextZBlock = world.GetBlock(worldStartX + x, worldStartY + y, worldStartZ + z + 1);
        return nextZBlock.Transparent;
    }

    private bool renderBlockLowZ(int x, int y, int z)
    {
        Block prevZBlock = world.GetBlock(worldStartX + x, worldStartY + y, worldStartZ + z - 1);
        return prevZBlock.Transparent;
    }

    private bool renderBlockHighX(int x, int y, int z)
    {
        Block nextXBlock = world.GetBlock(worldStartX + x + 1, worldStartY + y, worldStartZ + z);
        return nextXBlock.Transparent;
    }

    private bool renderBlockLowX(int x, int y, int z)
    {
        Block prevXBlock = world.GetBlock(worldStartX + x - 1, worldStartY + y, worldStartZ + z);
        return prevXBlock.Transparent;
    }

    private bool renderBlockHighY(int x, int y, int z)
    {
        // Always render topmost
        if (y == chunkBlocks[x].Length - 1)
        {
            return true;
        }
        else
        {
            // Render block high y side if the next is transparent
            Block nextYBlock = world.GetBlock(worldStartX + x, worldStartY + y + 1, worldStartZ + z);
            return nextYBlock.Transparent;
        }
    }  

    private void updateMesh()
    {
        for (int x = 0; x < chunkSizeX; x++)
        {
            for (int y = 0; y < chunkSizeY; y++)
            {
                for (int z = 0; z < chunkSizeZ; z++)
                {
                    ChunkBlock chunkBlock = chunkBlocks[x][y][z];
                    if (chunkBlock.renderHighY)
                    {
                        addBlockHighYSide(x, y, z);
                    }
                    if (chunkBlock.renderLowX)
                    {
                        addBlockLowXSide(x, y, z);
                    }
                    if (chunkBlock.renderHighX)
                    {
                        addBlockHighXSide(x, y, z);
                    }
                    if (chunkBlock.renderLowZ)
                    {
                        addBlockLowZSide(x, y, z);
                    }
                    if (chunkBlock.renderHighZ)
                    {
                        addBlockHighZSide(x, y, z);
                    }
                }
            }
        }

        Mesh mesh = meshFilter.sharedMesh;
        mesh.Clear();
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();

        meshRenderer.material.color = Color.green;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();

        newVertices.Clear();
        newTriangles.Clear();
    }

    private void addBlockHighZSide(int x, int y, int z)
    {
        newVertices.Add(new Vector3(0.5f + x, y + 1, 0.5f + z));    // 1
        newVertices.Add(new Vector3(-0.5f + x, y + 1, 0.5f + z));   // 2
        newVertices.Add(new Vector3(-0.5f + x, y, 0.5f + z));       // 3
        newVertices.Add(new Vector3(0.5f + x, y, 0.5f + z));        // 4

        int count = newVertices.Count;
        newTriangles.Add(count - 4);    //1
        newTriangles.Add(count - 3);    //2
        newTriangles.Add(count - 2);    //3
        newTriangles.Add(count - 4);    //1
        newTriangles.Add(count - 2);    //3
        newTriangles.Add(count - 1);    //4
    }

    private void addBlockLowZSide(int x, int y, int z)
    {
        newVertices.Add(new Vector3(-0.5f + x, y + 1, -0.5f + z));   // 1
        newVertices.Add(new Vector3(0.5f + x, y + 1, -0.5f + z));    // 2
        newVertices.Add(new Vector3(0.5f + x, y, -0.5f + z));        // 3
        newVertices.Add(new Vector3(-0.5f + x, y, -0.5f + z));       // 4

        int count = newVertices.Count;
        newTriangles.Add(count - 4);    //1
        newTriangles.Add(count - 3);    //2
        newTriangles.Add(count - 2);    //3
        newTriangles.Add(count - 4);    //1
        newTriangles.Add(count - 2);    //3
        newTriangles.Add(count - 1);    //4
    }

    private void addBlockHighXSide(int x, int y, int z)
    {
        newVertices.Add(new Vector3(0.5f + x, y + 1, -0.5f + z));   // 1
        newVertices.Add(new Vector3(0.5f + x, y + 1, 0.5f + z));    // 2
        newVertices.Add(new Vector3(0.5f + x, y, 0.5f + z));        // 3
        newVertices.Add(new Vector3(0.5f + x, y, -0.5f + z));       // 4

        int count = newVertices.Count;
        newTriangles.Add(count - 4);    //1
        newTriangles.Add(count - 3);    //2
        newTriangles.Add(count - 2);    //3
        newTriangles.Add(count - 4);    //1
        newTriangles.Add(count - 2);    //3
        newTriangles.Add(count - 1);    //4
    }

    private void addBlockLowXSide(int x, int y, int z)
    {
        newVertices.Add(new Vector3(-0.5f + x, y + 1, 0.5f + z));   // 1
        newVertices.Add(new Vector3(-0.5f + x, y + 1, -0.5f + z));  // 2
        newVertices.Add(new Vector3(-0.5f + x, y, -0.5f + z));      // 3
        newVertices.Add(new Vector3(-0.5f + x, y, 0.5f + z));       // 4

        int count = newVertices.Count;
        newTriangles.Add(count - 4);    //1
        newTriangles.Add(count - 3);    //2
        newTriangles.Add(count - 2);    //3
        newTriangles.Add(count - 4);    //1
        newTriangles.Add(count - 2);    //3
        newTriangles.Add(count - 1);    //4
    }

    private void addBlockHighYSide(int x, int y, int z)
    {
        newVertices.Add(new Vector3(-0.5f + x, y + 1, 0.5f + z));   // 1
        newVertices.Add(new Vector3(0.5f + x, y + 1, 0.5f + z));    // 2
        newVertices.Add(new Vector3(0.5f + x, y + 1, -0.5f + z));   // 3
        newVertices.Add(new Vector3(-0.5f + x, y + 1, -0.5f + z));  // 4

        int count = newVertices.Count;
        newTriangles.Add(count - 4);    //1
        newTriangles.Add(count - 3);    //2
        newTriangles.Add(count - 2);    //3
        newTriangles.Add(count - 4);    //1
        newTriangles.Add(count - 2);    //3
        newTriangles.Add(count - 1);    //4
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(
            new Vector3(worldStartX + 1.0f * chunkSizeX / 2 - 0.5f, worldStartY + 1.0f * chunkSizeY / 2, worldStartZ + 1.0f * chunkSizeZ / 2 - 0.5f),
            new Vector3(chunkSizeX, chunkSizeY, chunkSizeZ));
    }
}

