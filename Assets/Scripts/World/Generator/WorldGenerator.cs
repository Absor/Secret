using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class WorldGenerator : AutowiredMonoBehaviour
{
    public class WorldGenerateEvent : UnityEvent { }
    public readonly WorldGenerateEvent OnWorldGenerate = new WorldGenerateEvent();

    [SerializeField]
    private int worldSizeX = 160;
    [SerializeField]
    private int worldSizeY = 32;
    [SerializeField]
    private int worldSizeZ = 160;

    [Autowired]
    private World world;
    [Autowired]
    private BlockStore blockStore;

	void Start () {
        world.SetupNewWorld(worldSizeX, worldSizeY, worldSizeZ);

        for (int x = 0; x < world.WorldSizeX; x++)
        {
            for (int y = 0; y < world.WorldSizeY; y++)
            {
                for (int z = 0; z < world.WorldSizeZ; z++)
                {
                    Block block = generateBlock(x, y, z, worldSizeX, worldSizeY, worldSizeZ);
                    world.SetBlock(x, y, z, block);
                }
            }
        }
        OnWorldGenerate.Invoke();
	}

    private Block generateBlock(int x, int y, int z, int worldSizeX, int worldSizeY, int worldSizeZ)
    {
        if (y == 0)
        {
            return blockStore.GetBlock(BlockType.Grass);
        }
        else
        {
            return blockStore.GetBlock(BlockType.Air);
        }
    }
}
