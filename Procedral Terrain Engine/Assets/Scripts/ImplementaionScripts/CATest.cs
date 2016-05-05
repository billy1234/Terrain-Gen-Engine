using UnityEngine;
using System.Collections;
using cellularAutomataLib;
using System.Collections.Generic;

public class CATest : MonoBehaviour 
{
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
	#region ca lib utilities
	public enum ZONE: int
	{
		EMPTY =0, DOMESTIC = 1,COMERCIAL =2,INDUSTRIAL=3
	}
	public class tile
	{
		public ZONE zone = 0;
		public float strength =0;
		public int favorableNeighborCount;
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

	void Start()
	{
		cellRule<tile>[] rules; //= new cellRule<tile>[2]; //= new Dictionary<int,int[2]>(2);
		rules = new cellRule<tile>[4]
		{
			EmptyRules,DomesticRules,ComercialRules,IndustrialRules
		};
		Dictionary<ZONE,int> ruleMatrix = new Dictionary<ZONE, int>(){{ZONE.EMPTY,(int)ZONE.EMPTY},{ZONE.DOMESTIC,(int)ZONE.DOMESTIC},{ZONE.COMERCIAL,(int)ZONE.COMERCIAL},{ZONE.INDUSTRIAL,(int)ZONE.INDUSTRIAL}};
		tile[,] cells = new tile[size,size];
		displayTexture = new Texture2D(size,size);
		for(int x =0; x < size; x++)
		{
			for(int y =0; y < size; y++)
			{
				cells[x,y] = new tile();
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
		CA = new cellularAutomotaTile(cells,rules,ruleMatrix);
		StartCoroutine(step());
	}

	IEnumerator step()
	{
		while(Application.isPlaying)
		{
			yield return new WaitForSeconds(1f);
			CA.passNESW();
			Color[] pixels = new Color[size * size];
			for(int x =0; x < size; x++)
			{
				for(int y =0; y < size; y++)
				{
					Color c = new Color();
					if(CA.cells[x,y].zone == ZONE.EMPTY)
					{
						c = Color.white;
					}
					else if( CA.cells[x,y].zone == ZONE.DOMESTIC)
					{
						c = Color.blue;
					}
					else if(CA.cells[x,y].zone == ZONE.COMERCIAL)
					{
						c = Color.red;
					}
					else if(CA.cells[x,y].zone == ZONE.INDUSTRIAL)
					{
						c= Color.green;
					}

					pixels[x + y * size] = c * CA.cells[x,y].strength;

				}
			}
			displayTexture = heightMapUtility.heightMapToTexture.buildTextureFromPixels(pixels,size,size);
			gameObject.GetComponent<Renderer>().material.mainTexture = displayTexture;
		}
	}
	/*
	void OnDrawGizmos()
	{
		if(Application.isPlaying)
		{
			for(int x =0; x < 5; x++)
			{
				for(int y =0; y < 5; y++)
				{
					if((int)CA.cells[x,y].zone == 0)
					{
						Gizmos.color = Color.white;
					}
					if((int)CA.cells[x,y].zone == 1)
					{
						Gizmos.color = Color.blue;
					}
					else if((int)CA.cells[x,y].zone == 2)
					{
						Gizmos.color = Color.yellow;
					}
					else if((int)CA.cells[x,y].zone == 3)
					{
						Gizmos.color = Color.black;
					}
					Gizmos.DrawCube(new Vector3(x,y,0),Vector3.one);
				}
			}
		}
	}
	*/

}
