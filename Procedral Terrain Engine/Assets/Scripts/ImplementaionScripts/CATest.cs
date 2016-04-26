using UnityEngine;
using System.Collections;
using cellularAutomataLib;
using System.Collections.Generic;

public class CATest : MonoBehaviour 
{

	cellularAutomotaInt CA;
	public int rule1(int cell)//do nothing
	{
		return Random.Range(0,2);
	}
	public int rule2(int cell)//invert cell
	{
		if(cell ==1)
		{
			return 0;
		}
		return 1;
	}

	void Start()
	{
		genericDelegate<int,int>[] rules = new genericDelegate<int,int>[2]; //= new Dictionary<int,int[2]>(2);
		Dictionary<int,int[]> ruleMatrix = new Dictionary<int, int[]>();
		int[,] cells = new int[5,5];
		rules[0] = rule1;
		rules[1] = rule2;
		ruleMatrix[0] = new int[2]{0,1};
		ruleMatrix[1] = new int[2]{0,1};
		for(int x =0; x < 5; x++)
		{
			for(int y =0; y < 5; y++)
			{
				cells[x,y] = Random.Range(0,2);
			}

		}
		CA = new cellularAutomotaInt(cells,rules,ruleMatrix);
		StartCoroutine(step());
	}

	IEnumerator step()
	{
		while(Application.isPlaying)
		{
			CA.passNESW();
			yield return new WaitForSeconds(1f);
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
					Gizmos.color = Color.white;
					if(CA.cells[x,y] == 0)
					{
						Gizmos.color = Color.black;
					}
					Gizmos.DrawCube(new Vector3(x,y,0),Vector3.one);
				}
			}
		}
	}

}
