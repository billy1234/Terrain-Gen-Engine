using UnityEngine;
using System.Collections;
using cellularAutomataLib;
using System.Collections.Generic;

public class CATest : MonoBehaviour 
{
	public int size;
	public Texture displayTexture;
	cellularAutomotaTile CA;

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

	public void EmptyRules(ref tile me,ref tile[] neighbours)
	{
		return;//do nothing
	}
	public void DomesticRules(ref tile me,ref tile[] neighbours)
	{
		/*
		if(otherTile.zone == ZONE.EMPTY)
		{
			if(Random.Range(0,10) == 0)//1 in 10 chance
			{
				otherTile.zone = ZONE.DOMESTIC;
				otherTile.strength = 0.1f;
			}
		}
		else if(otherTile.zone == ZONE.DOMESTIC)
		{

			otherTile.strength *= me.strength;
			otherTile.strength = Mathf.Clamp(otherTile.strength,0f,1f);

		}
		*/
	}
	public void IndustrialRules(ref tile me,ref tile[] otherTile)
	{
		/*
		if(otherTile.zone == ZONE.EMPTY)
		{
			if(Random.Range(0,10) == 0)//1 in 10 chance
			{
				otherTile.zone = ZONE.INDUSTRIAL;
			}
		}
		*/
	}
	public void ComercialRules(ref tile me,ref tile[] neighbours)
	{
		/*
		if(otherTile.zone == ZONE.EMPTY || otherTile.zone == ZONE.INDUSTRIAL)
		{
			me.strength *= 0.9f;
		}
		if(otherTile.zone == ZONE.DOMESTIC)
		{

			me.favorableNeighborCount ++;
			me.strength = 0.25f * me.favorableNeighborCount;
			me.strength = Mathf.Clamp(me.strength,0f,1f);
		}
		*/
	}

	void Start()
	{
		cellRule<tile>[] rules; //= new cellRule<tile>[2]; //= new Dictionary<int,int[2]>(2);
		rules = new cellRule<tile>[4]
		{
			EmptyRules,DomesticRules,ComercialRules,IndustrialRules
		};
		Dictionary<ZONE,int> ruleMatrix = new Dictionary<ZONE, int>(){{ZONE.EMPTY,(int)ZONE.EMPTY},{ZONE.DOMESTIC,(int)ZONE.DOMESTIC},{ZONE.INDUSTRIAL,(int)ZONE.INDUSTRIAL},{ZONE.COMERCIAL,(int)ZONE.COMERCIAL}};
		tile[,] cells = new tile[size,size];
		displayTexture = new Texture2D(size,size);
		for(int x =0; x < size; x++)
		{
			for(int y =0; y < size; y++)
			{
				cells[x,y] = new tile();
				ZONE cellType = (ZONE)Random.Range(-5,4);
				if((int)cellType < 0)
				{				
					cellType = ZONE.EMPTY;
				}

				cells[x,y].zone = cellType;
				cells[x,y].strength = 0.1f;
				//int state
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
						c= Color.blue;
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
