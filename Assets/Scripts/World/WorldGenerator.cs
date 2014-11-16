using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class WorldGenerator : InjectedMonoBehaviour
{
    [SerializeField]
    private int worldSizeX = 160;
    [SerializeField]
    private int worldSizeY = 32;
    [SerializeField]
    private int worldSizeZ = 160;

    [Inject]
    private World world;
    [Inject]
    private BlockStore blockStore;

    [ContextMenu("Create World")]
	public void CreateWorld () {
        SimplexNoise simplexNoise = new SimplexNoise();

        world.SetupNewWorld(worldSizeX, worldSizeY, worldSizeZ);

        for (int x = 0; x < world.WorldSizeX; x++)
        {
            for (int y = 0; y < world.WorldSizeY; y++)
            {
                for (int z = 0; z < world.WorldSizeZ; z++)
                {
                    world.SetBlock(x, y, z, blockStore.GetBlock(BlockType.Air));
                }
            }
        }

        for (int x = 0; x < world.WorldSizeX; x++)
        {
            for (int z = 0; z < world.WorldSizeZ; z++)
            {
                int noise = (int) (simplexNoise.Noise(x, z) + 1);

                for (int y = 0; y <= noise; y++)
                {
                    world.SetBlock(x, y, z, blockStore.GetBlock(BlockType.Grass));
                }
            }
        }        

        world.OnWorldCreated.Invoke();
	}

    private Block generateBlock(int x, int y, int z, int worldSizeX, int worldSizeY, int worldSizeZ)
    {

            return blockStore.GetBlock(BlockType.Air);
    }
}
