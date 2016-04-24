using UnityEngine;
using System.Collections;
using cellularAutomataLib;
using System.Collections.Generic;

public class CATest : MonoBehaviour 
{
	public genericDelegate<int,int>[] rules = new genericDelegate<int,int>[2]; //= new Dictionary<int,int[2]>(2);
	Dictionary<int,int[]> ruleMatrix;
	ruleMatrix<int,int> ruleDecider;
	int[,] cells = new int[5,5];
	public int rule1(int cell)//do nothing
	{
		return cell;
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
		rules[0] = rule1;
		rules[1] = rule2;
		ruleMatrix[0] = new int[2]{0,1};
		ruleMatrix[2] = new int[2]{0,1};
		ruleDecider = new ruleMatrix<int,int>(ref rules,ref ruleMatrix);
		for(int x =0; x < 5; x++)
		{
			for(int y =0; y < 5; y++)
			{
				cells[x,y] = Random.Range(0,2);
			}

		}
	}

	void FixedUpdate()
	{
		for(int x =0; x < 5; x++)
		{
			for(int y =0; y < 5; y++)
			{
				//cells[x,y] = ruleMatrix<int,int>().getRule();
			}
		}
	}

}
