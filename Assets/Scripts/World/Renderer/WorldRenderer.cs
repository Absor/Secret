using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldRenderer : AutowiredMonoBehaviour
{
    [SerializeField]
    private int chunkSizeX = 16;
    [SerializeField]
    private int chunkSizeY = 16;
    [SerializeField]
    private int chunkSizeZ = 16;

    [Autowired]
    private World world;
    [Autowired]
    private WorldGenerator worldGenerator;

    private Chunk[] chunks;
    private int chunksX, chunksY, chunksZ;

    protected override void Awake()
    {
        base.Awake();
        worldGenerator.OnWorldGenerate.AddListener(onWorldGenerated);
    }

    void OnDestroy()
    {
        worldGenerator.OnWorldGenerate.RemoveListener(onWorldGenerated);
    }

    private void onWorldGenerated()
    {
        destroyOldChunks();

        chunksX = Mathf.CeilToInt(1.0f * world.WorldSizeX / chunkSizeX);
        chunksY = Mathf.CeilToInt(1.0f * world.WorldSizeY / chunkSizeY);
        chunksZ = Mathf.CeilToInt(1.0f * world.WorldSizeZ / chunkSizeZ);

        chunks = new Chunk[chunksX * chunksY * chunksZ];

        for (int x = 0; x < chunksX; x++)
        {
            for (int y = 0; y < chunksY; y++)
            {
                for (int z = 0; z < chunksZ; z++)
                {
                    createNewChunk(x, y, z);
                }
            }
        }
    }

    private void createNewChunk(int chunkX, int chunkY, int chunkZ)
    {
        int worldX = chunkX * chunkSizeX;
        int worldY = chunkY * chunkSizeY;
        int worldZ = chunkZ * chunkSizeZ;

        GameObject empty = new GameObject();
        empty.name = string.Format("Chunk X: {0,-5} Y: {1,-5} Z: {2,-5}", chunkX, chunkY, chunkZ);
        empty.transform.SetParent(transform);
        empty.AddComponent<Chunk>();

        Chunk chunk = empty.GetComponent<Chunk>();
        setChunkConnections(chunkX, chunkY, chunkZ, chunk);
        chunk.Initialize(world, worldX, worldY, worldZ, chunkSizeX, chunkSizeY, chunkSizeZ);
        setChunk(chunkX, chunkY, chunkZ, chunk);
    }

    private void setChunkConnections(int chunkX, int chunkY, int chunkZ, Chunk chunk)
    {
        if (chunkX > 0)
        {
            chunk.PrevX = getChunk(chunkX - 1, chunkY, chunkZ);
        }

        if (chunkY > 0)
        {
            chunk.PrevY = getChunk(chunkX, chunkY - 1, chunkZ);
        }

        if (chunkZ > 0)
        {
            chunk.PrevZ = getChunk(chunkX, chunkY, chunkZ - 1);
        }
    }

    private void destroyOldChunks()
    {
        if (chunks == null)
        {
            return;
        }
        foreach (Chunk chunk in chunks)
        {
            GameObject.Destroy(chunk.gameObject);
        }
        chunks = null;
    }

    private void setChunk(int x, int y, int z, Chunk chunk)
    {
        chunks[getArrayIndex(x, y, z)] = chunk;
    }

    private Chunk getChunk(int x, int y, int z)
    {
        return chunks[getArrayIndex(x, y, z)];
    }

    private int getArrayIndex(int x, int y, int z)
    {
        return (x * chunksX + y) * chunksY + z;
    }
}
