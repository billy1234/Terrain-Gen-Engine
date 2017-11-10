using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxel;
using NoiseExtention;

public class VoxelImplementation : MonoBehaviour {

    float noiseScale = 5;
    int chunkWidth = 64, chunkHeight = 50;
    int seaLevel = 20, mountainMax = 45;
    float lacanarity = 0.6f, persitance = 2f;
    int octaves = 4;
    string seed = "world1";


	void Start () {

        mountainMax = mountainMax - seaLevel;

        //wrap this in voxel
        float[,] heightMap = perlinNoiseLayeredSimple.perlinNoise(chunkWidth, chunkWidth, seed.GetHashCode(), noiseScale, octaves, persitance, lacanarity, Vector2.zero);
        voxelChunk c = new voxelChunk(0,0,0,chunkWidth,chunkHeight,
            (ref voxelCell v, int x, int y, int z) => {
                basicVoxelCell cell = new basicVoxelCell(x, y, z, (y < 20 + heightMap[x,z] * mountainMax));
                v = cell;
                cell.spawn();

            });		
	}
	


    public class basicVoxelCell : voxelCell
    {
        public bool full = true;
        public GameObject worldObject;

        public basicVoxelCell(int x, int y, int z, bool full) : base(x, y, z)
        {
            this.full = full;
        }

        public void spawn()
        {
            if (full)
            {
                worldObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                worldObject.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                worldObject.GetComponent<Collider>().enabled = false;
                worldObject.transform.position = new Vector3(x, y, z);
            }
        }
    }
}
