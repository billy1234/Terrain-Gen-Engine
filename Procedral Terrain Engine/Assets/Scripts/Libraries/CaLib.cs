using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace cellularAutomataLib
{
	public delegate T1 genericDelegate<T1,T2>(T2 item);

	public static class neighborAcces
	{

		public enum direction : int
		{
			UP =0,DOWN =1,LEFT =2,RIGHT =3,UP_LEFT=4,UP_RIGHT=5,DOWN_LEFT=6,DOWN_RIGHT=7
		}

		static readonly Vector2[] directionVectors = new Vector2[8]{
		Vector2.up,Vector2.down,Vector2.left,Vector2.right,
		Vector3.up+Vector3.left,Vector3.up+Vector3.right,
		Vector2.down + Vector2.left, Vector2.down + Vector2.left
		};

		static public  T[]  getNeigborsNESW<T>(ref T[,] array, Vector2 cell) //no flattened array implemenation as of now
		{
			return getNeigbors(ref array,cell,new direction[4]{direction.UP,direction.RIGHT,direction.DOWN,direction.LEFT});
		}
		static public  T[]  getNeigborsNESWDiag<T>(ref T[,] array, Vector2 cell) //no flattened array implemenation as of now
		{
			return getNeigbors(ref array,cell,new direction[8]{direction.UP,direction.UP_RIGHT,direction.RIGHT,direction.DOWN_RIGHT,direction.DOWN,direction.DOWN_LEFT,direction.LEFT,direction.UP_LEFT});
		}

		static public  T[]  getNeigbors<T>(ref T[,] array, Vector2 cell,direction[] sidesToCheck) //no flattened array implemenation as of now
		{
			T[] cells = new T[sidesToCheck.Length];
			bool[] isNull = new bool[sidesToCheck.Length];
			int legalCells =0;
			int lengthX = array.GetLength(0);
			int lengthY = array.GetLength(1);
			int cellX = (int)cell.x;
			int cellY = (int)cell.y;

			# region checkNulls
			for(int i=0; i < sidesToCheck.Length; i++)
			{
				cellX = (int)cell.x + (int)neighborAcces.directionVectors[(int)sidesToCheck[i]].x;
				cellY = (int)cell.y + (int)neighborAcces.directionVectors[(int)sidesToCheck[i]].y;
				if(cellX >= 0 && cellY >= 0 && cellX < lengthX && cellY < lengthY)
				{
					isNull[i] = false;
					cells[i] = array[cellX,cellY];
					legalCells ++;
				}
				else
				{
					isNull[i] = true;
				}
			}
			#endregion

			#region buildArray
			T[] finalCells = new T[legalCells];
			int cellsIndex =0;
			for(int i=0; i < sidesToCheck.Length; i++)
			{
				if(isNull[i] == false)
				{
					finalCells[cellsIndex] = cells[i];
					cellsIndex ++;
				}
			}
			#endregion
			return finalCells;
		}
	}
	
		//t1 is output t2 is input 
		public class ruleMatrix<T1,T2>
		{
			//public  int[,] ruleMatrixArray;//indexes

			public ruleMatrix(genericDelegate<T1,T2>[] rules,Dictionary<int,int[]> ruleDictionary)
			{
				this.rules = rules;
				if(ruleDictionary.Count != rules.Length)
				{
					Debug.Log("Error rule count does not match rule definition count " + this);
				}
				foreach(int[] ruleIndexes in ruleDictionary.Values)
				{
					if(ruleDictionary.Count != rules.Length)
					{
						Debug.Log("Error rule count does not match rule definition count " + this);
					}
				}
				this.ruleDictionary = ruleDictionary;

			}

			protected genericDelegate<T1,T2>[] rules;

			protected Dictionary<int,int[]> ruleDictionary;

			protected int getRuleNumber(int state,int neighbor)
			{

				if(state < 0 || neighbor < 0 || state > ruleDictionary.Count ||neighbor > ruleDictionary.Count)
				{
					Debug.LogError(string.Format("Invalid state for either cell: {0} or {1} in rule matrix {2}",state,neighbor,this));
				}

				return(ruleDictionary[state][neighbor]);
			}

			public genericDelegate<T1,T2> getRule(int state,int neighbor)
			{
				return rules[getRuleNumber(state,neighbor)];
			}
		}	
}
