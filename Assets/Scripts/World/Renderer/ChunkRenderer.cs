using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Chunk))]
public class ChunkRenderer : MonoBehaviour
{
    private Chunk chunk;

    public void Init(Chunk chunk)
    {
        this.chunk = chunk;
        transform.position = new Vector3(chunk.x, chunk.y, chunk.z);
        chunkUpdated();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (int x = 0; x < chunk.sizeX; x++)
        {
            for (int y = 0; y < chunk.sizeY; y++)
            {
                for (int z = 0; z < chunk.sizeZ; z++)
                {
                    if (chunk.GetBlock(x, y, z) != null && chunk.GetBlock(x, y, z).render)
                    {
                        Gizmos.DrawWireCube(transform.position + new Vector3(x, y, z), Vector3.one);
                    }
                }
            }
        }
    }

    private void chunkUpdated()
    {

    }

    internal void Initialize(int worldX, int worldY, int worldZ, World world)
    {
        throw new System.NotImplementedException();
    }
}
