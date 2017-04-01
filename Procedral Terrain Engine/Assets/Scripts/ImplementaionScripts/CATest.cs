using UnityEngine;
using System.Collections;
using cellularAutomataLib;
using System.Collections.Generic;
using NoiseExtention;
public class CATest : MonoBehaviour 
{
	private int vertSize;
	public int size;
	public Texture displayTexture;
	cellularAutomotaTile CA;
	const int maxNeighbours =4;
	[Range(0,1)]
	public float domesticPlaceRate;
	[Range(0,1)]
	public float comercialPlaceRate;
	[Range(0,1)]
	public float industrialPlaceRate;

	public int seed =1;
	public float scale =0.4f;
	public int octaves =4;
	public float persistance =0.5f;
	public float lacunarity =1.5f;
	public float watterCutoff =0.15f;
	public float softCutoffRadius;
	public float hardCutoffRadius;
	public Vector3 meshScaleVector;
	#region ca lib utilities
	public enum ZONE: int
	{
		EMPTY =0, DOMESTIC = 1,COMERCIAL =2,INDUSTRIAL=3, OCEAN =4
	}
	public class tile
	{
		public ZONE zone = 0;
		public float strength =0;
		public float landHeight;
		public Vector3 normal;

	}

	public class cellularAutomotaTile:cellularAutomotaBase<tile,ZONE>
	{
		public cellularAutomotaTile(tile[,] cells, cellRule<tile>[] rules, Dictionary<ZONE,int> ruleDictionary):base(cells,rules,ruleDictionary)
		{
			
		}

		protected override ZONE getRuleQueryFromTile (tile tile)
		{
			return tile.zone;
		}
		
	
	}
	#endregion

	#region editorScripting
	public void OnValidate()
	{
		float maxPer =0;
		maxPer = domesticPlaceRate;
		if(maxPer + comercialPlaceRate > 1)
		{
			comercialPlaceRate = 1 - maxPer;
		}
		maxPer += comercialPlaceRate;
		if(maxPer + industrialPlaceRate > 1)
		{
			industrialPlaceRate = 1- maxPer;
		}
	}
	#endregion

	#region ca
	public void EmptyRules(ref tile me,ref tile[] neighbours)
	{

		//do nothing
	}
	public void DomesticRules(ref tile me,ref tile[] neighbours)
	{

		int goodNeighbours =0;
		int indNeighbours =0;
		for(int i=0; i < neighbours.Length; i++)
		{
			if(neighbours[i].zone ==ZONE.DOMESTIC || neighbours[i].zone == ZONE.COMERCIAL)
			{
				goodNeighbours ++;
			}
			else if(neighbours[i].zone == ZONE.INDUSTRIAL)
			{
				indNeighbours ++;
			}
		}

		for(int i=0; i < neighbours.Length; i++)
		{
			if( neighbours[i].zone == ZONE.EMPTY&&goodNeighbours < maxNeighbours&&Random.Range(0,25-goodNeighbours)==0)
			{
				neighbours[i].zone = ZONE.DOMESTIC;
				neighbours[i].strength = 0.1f;
			}
		}

		 me.strength = Mathf.Clamp((goodNeighbours - indNeighbours)/maxNeighbours,0f,1f);
	}
	public void IndustrialRules(ref tile me,ref tile[] neighbours)
	{
		int emptyCount =0;
		int badNeighbourCount =0;
		for(int i=0; i < neighbours.Length; i++)
		{
			if(neighbours[i].zone == ZONE.EMPTY)
			{
				emptyCount++;
			}
			else if(neighbours[i].zone == ZONE.COMERCIAL || neighbours[i].zone == ZONE.DOMESTIC)
			{
				badNeighbourCount ++;
			}
		}
		me.strength = 1-(badNeighbourCount/maxNeighbours);
		for(int i=0; i < neighbours.Length; i++)
		{
			if(neighbours[i].zone == ZONE.EMPTY && (Random.Range(0f,me.strength) * emptyCount) > maxNeighbours * 0.8f)
			{
				neighbours[i].zone = ZONE.INDUSTRIAL;
			}
		}
	}
	public void ComercialRules(ref tile me,ref tile[] neighbours)
	{
		int comNeighbours =0;
		int indNeighbours =0;
		for(int i=0; i < neighbours.Length; i++)
		{
			if(neighbours[i].zone == ZONE.DOMESTIC ||neighbours[i].zone == ZONE.COMERCIAL)comNeighbours++;
			if(neighbours[i].zone == ZONE.INDUSTRIAL)indNeighbours++;
		}
		for(int i=0; i < neighbours.Length; i++)
		{
			if(neighbours[i].zone ==ZONE.EMPTY&&Random.Range(0,3-comNeighbours)==2)neighbours[i].zone = ZONE.COMERCIAL;
		}
		me.strength = (comNeighbours - indNeighbours)/ maxNeighbours;
	}
	#endregion

	void Start()
	{
		vertSize = size+1;
		float[,] heightmap = perlinNoiseLayeredSimple.perlinNoise(vertSize,vertSize,seed,scale,octaves,persistance,lacunarity,Vector2.one);
		heightMapUtility.heightMapSmoothing.clampHeightMapAt(ref heightmap,watterCutoff);
		heightMapUtility.heightMapSmoothing.clampEdgesCircular(ref heightmap,softCutoffRadius,hardCutoffRadius,watterCutoff);
		Mesh myMesh = heightMapUtility.heightMapToMesh.meshFromHeightMap(heightmap,meshScaleVector);
		GetComponent<MeshFilter>().mesh = myMesh;
		#region caStart
		cellRule<tile>[] rules; //= new cellRule<tile>[2]; //= new Dictionary<int,int[2]>(2);
		rules = new cellRule<tile>[5]
		{
			EmptyRules,DomesticRules,ComercialRules,IndustrialRules,EmptyRules
		};
		Dictionary<ZONE,int> ruleMatrix = new Dictionary<ZONE, int>(){{ZONE.EMPTY,(int)ZONE.EMPTY},{ZONE.DOMESTIC,(int)ZONE.DOMESTIC},{ZONE.COMERCIAL,(int)ZONE.COMERCIAL},{ZONE.INDUSTRIAL,(int)ZONE.INDUSTRIAL},{ZONE.OCEAN,(int)ZONE.OCEAN}};

		tile[,] cells = new tile[size,size];
		displayTexture = new Texture2D(size ,size );
		for(int x =0; x < size ; x++)
		{
			for(int y =0; y < size ; y++)
			{
				cells[x,y] = new tile();
				//print(x  + y  * QuadSize  +" "+x +1 + y * size +" "+x + (y +1) * QuadSize  +" "+x +1 + (y +1) * size);

				//dealing with the quads 4 points hence the 4 averaged values
				cells[x,y].normal =  (myMesh.normals[x + y * size ] + myMesh.normals[x + 1 + y * vertSize]+myMesh.normals[x + (y +1) * size ]+ myMesh.normals[x + 1 + (y +1) * vertSize]) /4f;
				cells[x,y].landHeight = ((myMesh.vertices[x + y * size ] + myMesh.vertices[x + 1 + y * vertSize]+myMesh.vertices[x + (y +1) * size ]+ myMesh.vertices[x + 1 + (y +1) * vertSize]) /4f).y;
				if(heightmap[x,y] <= watterCutoff)
				{
					cells[x,y].zone = ZONE.OCEAN;
					cells[x,y].strength = 0;
				}
				else
				{
					float chance = Random.Range(0f,1f);
					ZONE cellType;
					if(chance < domesticPlaceRate)
					{
						cellType = ZONE.DOMESTIC;
					}
					else if(chance < domesticPlaceRate + comercialPlaceRate)
					{
						cellType = ZONE.COMERCIAL;
					}
					else if(chance < domesticPlaceRate + comercialPlaceRate + industrialPlaceRate)
					{
						cellType = ZONE.INDUSTRIAL;
					}
					else
					{
						cellType = ZONE.EMPTY;
					}

					cells[x,y].zone = cellType;
					cells[x,y].strength = 0.1f;
				}
			}

		}
		CA = new cellularAutomotaTile(cells,rules,ruleMatrix);
		StartCoroutine(step());
		#endregion
	}

	IEnumerator step()
	{
		while(Application.isPlaying)
		{
			yield return new WaitForSeconds(1f);
			CA.passNESW();
			Color[] pixels = new Color[size * vertSize];
			for(int x =0; x < size ; x++)
			{

				for(int y =0; y < size ; y++)
				{

					Color c = new Color();
					if(CA.cells[x,y].zone == ZONE.EMPTY)
					{
						c= new Color(1,0,0,0);
					}
					else if( CA.cells[x,y].zone == ZONE.DOMESTIC)
					{
						c = new Color(0,1,0,0);
					}
					else if(CA.cells[x,y].zone == ZONE.COMERCIAL)
					{
						c = new Color(0,0,1,0);
					}
					else if(CA.cells[x,y].zone == ZONE.INDUSTRIAL)
					{
						c= new Color(0,0,0,1);
					}
					if(CA.cells[x,y].zone != ZONE.OCEAN)
					{
                        pixels[x + y * size] = c; //* CA.cells[x,y].strength;
					}
					else
					{
						pixels[x + y * size] = Color.red;
						//pixels[x + y * size] = Color.blue * Random.Range(0f,1f);
					}

				}
			}
			displayTexture = heightMapUtility.heightMapToTexture.buildTextureFromPixels(pixels,size,size);
			displayTexture.filterMode = FilterMode.Bilinear;
			gameObject.GetComponent<Renderer>().material.mainTexture = displayTexture;
		}
	}

}
