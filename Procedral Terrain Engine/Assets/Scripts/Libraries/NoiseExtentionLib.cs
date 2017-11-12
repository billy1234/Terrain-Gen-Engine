using UnityEngine;
using System.Collections;


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
       
        public static float[,] perlinNoise(int mapWidth, int mapHeight, int seed, float scale, int octaves, float peristance, float lacunarity, Vector2 offset, bool squared)
        {
            System.Random prng = new System.Random(seed);
            float[,] noiseMap = new float[mapWidth, mapHeight];
            Vector2[] octaveOffsets = new Vector2[octaves];
            for (int i = 0; i < octaves; i++)
            {
                float offsetX = prng.Next(-100000, 100000);
                float offsetY = prng.Next(-100000, 100000);
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }
            float maxNoise = float.MinValue;
            float minNoise = float.MaxValue;
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    float amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;
                    for (int i = 0; i < octaves; i++)
                    {
                        float sampleX = (x + offset.x)  / scale * frequency + octaveOffsets[i].x;
                        float sampleY = (y + offset.y)  / scale * frequency + octaveOffsets[i].y;
                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= peristance;
                        frequency *= lacunarity;
                    }

                   


                    if (noiseHeight > maxNoise)
                    {
                        maxNoise = noiseHeight;
                    }
                    else if (noiseHeight < minNoise)
                    {
                        minNoise = noiseHeight;
                    }

                    if (squared)
                    {
                        noiseHeight *= noiseHeight;
                    }

                    noiseMap[x, y] = noiseHeight;

                }
            }
            //normalization
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minNoise, maxNoise, noiseMap[x, y]);
                }
            }
            return noiseMap;
        }

        public static float[,] perlinNoise(int mapWidth, int mapHeight, int seed, float scale, int octaves, float peristance, float lacunarity, Vector2 offset)
        {
            return perlinNoise(mapWidth, mapHeight, seed, scale, octaves, peristance, lacunarity, offset, false);
        }

        public static float[,] perlinNoise(int x, int y, int mapWidth, int mapHeight, int seed, float scale, int octaves, float peristance, float lacunarity)
        {
            return perlinNoise(mapWidth, mapHeight, seed, scale, octaves, peristance, lacunarity, new Vector2(x,y), false);
        }

        public static float[,] perlinNoise(int x, int y, int mapWidth, int mapHeight, int seed, float scale, int octaves, float peristance, float lacunarity, bool squared)
        {
            return perlinNoise(mapWidth, mapHeight, seed, scale, octaves, peristance, lacunarity, new Vector2(x, y), squared);
        }
    }

    public class hillNoise
	{
		public hillNoise(int hillCount,float maxHillSize,float minHillSize,float hillStrength)
		{
			this.hills = new hill[hillCount];
			this.hillStrength = hillStrength;
			for(int i=0; i <hillCount; i++)
			{
				this.hills[i] = new hill(maxHillSize,minHillSize);
			}
		}
		hill[] hills;
		float hillStrength =0.3f;
		public float getHeightAt(float x, float y)
		{
			float height =0;
			foreach(hill h in hills)
			{
				height += h.myInfuenceAt(x,y) * hillStrength;//addative as of now
			}
		return height;
		}
	#region InternalDataStype
		private class hill
		{
			
			public hill(float maxWidth,float minWidth)
			{
				this.hillPosition = new Vector2(Random.Range(0f,1f),Random.Range(0f,1f));
				this.hillSize = new Vector2(Random.Range(minWidth,maxWidth),Random.Range(minWidth,maxWidth));
				
			}
			
			
			//both vectors should be normalized between 0-1
			public Vector2 hillPosition;
			public Vector2 hillSize{set{
										_hillSize = value;
										hillSizeSquared = new Vector2(Mathf.Sqrt(value.x),Mathf.Sqrt(value.y));
										}} //rather than constaly calculate square store it once it is set
			private Vector2 _hillSize;
			private Vector2 hillSizeSquared;
			
			
			
			public float myInfuenceAt(float x, float y)
			{
				Vector2 hillPosition = this.hillPosition;
				float z = ovalHeightFormula(hillSizeSquared,hillPosition,x,y);

				if(z < 0)
				{
					z=0;
				}				
				return z;
			}

			private float ovalHeightFormula(Vector2 hillsizeSquared,Vector2 hillPosition,float x, float y)
			{
				return (hillSizeSquared.x - x1MinusY1Squared(x,hillPosition.x)) +    (hillSizeSquared.y - x1MinusY1Squared(y,hillPosition.y ));
			}
			
			private float x1MinusY1Squared(float x, float y)
			{
				return Mathf.Pow(x-y,2f);
			}
		}
	#endregion

	public static Texture2D testText(int hills, float maxHillSize,float minHillSize,int size,float hillStrength)
	{
		hillNoise d = new hillNoise(hills,maxHillSize,minHillSize,hillStrength);
		Color[] pixels = new Color[size * size];
		for(int y =0; y < size; y++)
		{
			for(int x=0; x < size; x++)
			{
				float height = d.getHeightAt((float)x/(float)size,(float)y/(float)size);
				pixels[x + y * size] = new Color(height,height,height);
			}
		}
		return heightMapUtility.heightMapToTexture.buildTextureFromPixels(pixels,size,size);

	}

}

}
