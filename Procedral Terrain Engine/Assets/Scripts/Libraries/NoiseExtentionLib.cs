using UnityEngine;
using System.Collections;
/*
[System.Serializable]
public struct NoiseOctaveInfo
{
	public float Lacunarity;
	public float Persistance;

}
*/

namespace NoiseExtention
{
	public static class cellNoise
	{
		private static System.Random noisePrng;
		public static float[,] getNormalizedCellNoise(int height, int width,int seed,int featurePointCount)
		{
			float[,] noise = getCellNoise(height, width, seed, featurePointCount);
			float maxValue =0;
			for(int y =0; y < height; y++)
			{
				for(int x =0; x < height; x++)
				{
					if(maxValue < noise[x,y])
					{
						maxValue = noise[x,y];
					}
				}
			}

			for(int y =0; y < height; y++)
			{
				for(int x =0; x < height; x++)
				{
					noise[x,y] /= maxValue;
				}
			}
			return noise;
		}
		public static float[,] getCellNoise(int height, int width,int seed,int featurePointCount)
		{
			noisePrng = new System.Random(seed);
			float[,] noise = new float[height,width];
			Vector2[] featurePoints = new Vector2[featurePointCount];
			for(int i =0; i < featurePointCount;i++)
			{
				featurePoints[i] = new Vector2((float)noisePrng.NextDouble(),(float)noisePrng.NextDouble());
			}
			for(int y =0; y < height; y++)
			{
				for(int x =0; x < height; x++)
				{
					noise[x,y] = setCell(featurePoints,x,y,height,width);
				}
			}
			return noise;
		}

		private static float setCell(Vector2[] featurepoints,int x,int y,int height, int width)
		{
			float minDistance = Mathf.Infinity;
			float featureX;
			float featureY;
			Vector2 position = new Vector2(x,y);
			for(int i =0; i < featurepoints.Length; i++)
			{
				featureX = featurepoints[i].x * width;
				featureY = featurepoints[i].y * height;
				float currentDistance =Mathf.Abs(euclideandistance(position,new Vector2(featureX,featureY)));
				if(currentDistance < minDistance)
				{
					minDistance =  currentDistance;
				}
			}
			return minDistance;
		}
		private static float euclideandistance(Vector2 p1, Vector2 p2)
		{
			return((p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y) + (p1.x - p2.x) * (p1.x - p2.x));
		}
	}

	public static class perlinNoiseLayeredSimple
	{
		public static float[,] perlinNoise(int mapWidth, int mapHeight, int seed, float scale, int octaves, float peristance, float lacunarity,Vector2 offset,bool squared)
		{
			System.Random prng = new System.Random(seed);
			float[,] noiseMap = new float[mapWidth,mapHeight];
			Vector2[] octaveOffsets = new Vector2[octaves];
			for (int i=0; i < octaves; i++)
			{
				float offsetX = prng.Next(-100000,100000) + offset.x;
				float offsetY = prng.Next(-100000,100000) + offset.y;
				octaveOffsets[i] = new Vector2(offsetX,offsetY);
			}
			float maxNoise = float.MinValue;
			float minNoise = float.MaxValue;
			for(int y =0; y < mapHeight; y++)
			{
				for(int x =0; x < mapWidth; x++)
				{
					float amplitude =1;
					float frequency =1;
					float noiseHeight =0;
					for(int i=0; i< octaves; i++)
					{
						float sampleX = x/scale*frequency + octaveOffsets[i].x;
						float sampleY = y/scale*frequency + octaveOffsets[i].y;
						float perlinValue = Mathf.PerlinNoise(sampleX,sampleY);
						noiseHeight +=perlinValue * amplitude;

						amplitude *= peristance;
						frequency *= lacunarity;
					}


					if(noiseHeight > maxNoise)
					{
						maxNoise = noiseHeight;
					}
					else if(noiseHeight < minNoise)
					{
						minNoise = noiseHeight;
						//Debug.Log(minNoise);
					}
					if(squared)
					{
						noiseHeight *= noiseHeight * noiseHeight;
					}

						noiseMap[x,y] = noiseHeight;
					
				}
			}
			//normalization
			for(int y =0; y < mapHeight; y++)
			{
				for(int x =0; x < mapWidth; x++)
				{
					noiseMap[x,y] = Mathf.InverseLerp(minNoise,maxNoise,noiseMap[x,y]);
				}
			}
			return noiseMap;
		}
		public static float[,] perlinNoise(int mapWidth, int mapHeight, int seed, float scale, int octaves, float peristance, float lacunarity,Vector2 offset)
		{
			return perlinNoise(mapWidth, mapHeight, seed, scale, octaves, peristance, lacunarity, offset,false);
		}
	}
}
