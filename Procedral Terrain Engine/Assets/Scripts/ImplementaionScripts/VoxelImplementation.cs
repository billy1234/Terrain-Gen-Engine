using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxel;
using NoiseExtention;

public class VoxelImplementation : MonoBehaviour {

    private static float noiseScale = 5, lacanarity = 0.6f, persitance = 2f;
    private static int octaves = 4;


    int chunkSize = 32, chunkHeight = 25;
    int seaLevel = 5, mountainMax = 25;

    private static string seed = "world1";


    void Start() {

        mountainMax = mountainMax - seaLevel;

        voxelWorld w = new voxelWorld(chunkSize, chunkHeight);
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                w.spawnChunk(x, y, chunkInit);
            }
        }
       

    }



    void chunkInit(ref voxelChunk c, int x, int z)
    {
        GameObject g = new GameObject("Chunk");
        Vector3 chunkWorldPos = new Vector3(x * chunkSize, 0, z * chunkSize);
        g.transform.position = chunkWorldPos;

        float[,] heightMap = perlinNoiseLayeredSimple.perlinNoise(x  * chunkSize, z * chunkSize, c.chunkSize, c.chunkSize, seed.GetHashCode(), noiseScale, octaves, persitance, lacanarity);
        c.doToAllCells((ref voxelCell v, int blockX, int blockY, int blockZ) => { //do the terrain generation to the cells

            basicVoxelCell cell = new basicVoxelCell(blockX, blockY, blockZ, (blockY < seaLevel + heightMap[blockX, blockZ] * mountainMax));

            v = cell;
            GameObject obj = cell.spawn(chunkWorldPos);
            if (obj != null)
            {
                obj.transform.parent = g.transform;
            }
        });

    }
	


    public class basicvchunk : voxelChunk
    {
        public basicvchunk(int x, int z, int chunkSize, int chunkHeight) : base(x, z, chunkSize, chunkHeight)    
        {
               
        }

    }


    public class basicVoxelCell : voxelCell
    {
        public bool full = true;
        public GameObject worldObject;

        public basicVoxelCell(int x, int y, int z, bool full) : base(x, y, z)
        {
            this.full = full;
        }

        public GameObject spawn(Vector3 offset)
        {
            if (full)
            {
                worldObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                worldObject.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                worldObject.GetComponent<Collider>().enabled = false;
                worldObject.transform.position = new Vector3(x, y, z) + offset;
            }

            return worldObject;
        }
    }
}
