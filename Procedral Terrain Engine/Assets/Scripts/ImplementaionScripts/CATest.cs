using UnityEngine;
using System.Collections;
using cellularAutomataLib;
using System.Collections.Generic;

public class CATest : MonoBehaviour 
{
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

	public tile EmptyRules(tile me,tile otherTile)
	{
		return otherTile;//do nothing
	}
	public tile DomesticRules(tile me,tile otherTile)
	{
		if(otherTile.zone == ZONE.EMPTY)
		{
			if(Random.Range(0,2) ==0)
			{
				otherTile.zone = ZONE.DOMESTIC;
				otherTile.strength = 0.1f;
			}
		}

		if(otherTile.zone == ZONE.DOMESTIC)
		{

			otherTile.strength *= me.strength;
			otherTile.strength = Mathf.Clamp(otherTile.strength,0f,1f);

		}
		return otherTile;
	}
	public tile IndustrialRules(tile me,tile otherTile)
	{
		return otherTile;//Random.Range(0,2);
	}
	public tile ComercialRules(tile me,tile otherTile)
	{
		return me;//Random.Range(0,2);
	}

	void Start()
	{
		cellRule<tile>[] rules; //= new cellRule<tile>[2]; //= new Dictionary<int,int[2]>(2);
		rules = new cellRule<tile>[4]
		{
			EmptyRules,DomesticRules,IndustrialRules,ComercialRules
		};
		Dictionary<ZONE,int> ruleMatrix = new Dictionary<ZONE, int>(){{ZONE.EMPTY,(int)ZONE.EMPTY},{ZONE.DOMESTIC,(int)ZONE.DOMESTIC},{ZONE.INDUSTRIAL,(int)ZONE.INDUSTRIAL},{ZONE.COMERCIAL,(int)ZONE.COMERCIAL}};
		tile[,] cells = new tile[5,5];

		for(int x =0; x < 5; x++)
		{
			for(int y =0; y < 5; y++)
			{
				cells[x,y] = new tile();
				cells[x,y].zone = (ZONE)Random.Range(0,4);

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
			CA.passNESWD();

		}
	}
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

}
