using UnityEngine;
using System.Collections;

/// <summary>
/// used to query array neighbours mainly 2d arrays
/// </summary>
namespace gridLib
{
	public static class neighborAccess
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
		
		//pointers are needed for this aporach to alow the retured values to be editable
		
		static public Vector2[]  getNeigborsNESW<T>(ref T[,] array, Vector2 cell) //no flattened array implemenation as of now
		{
			return getNeigbors(ref array,cell,new direction[4]{direction.UP,direction.RIGHT,direction.DOWN,direction.LEFT});
		}
		static public Vector2[]  getNeigborsNESWDiag<T>(ref T[,] array, Vector2 cell) //no flattened array implemenation as of now
		{
			return getNeigbors(ref array,cell,new direction[8]{direction.UP,direction.UP_RIGHT,direction.RIGHT,direction.DOWN_RIGHT,direction.DOWN,direction.DOWN_LEFT,direction.LEFT,direction.UP_LEFT});
		}
		
		static public Vector2[]  getNeigbors<T>(ref T[,] array, Vector2 cell,direction[] sidesToCheck) //no flattened array implemenation as of now
		{
			Vector2[] cells = new Vector2[sidesToCheck.Length];
			bool[] isNull = new bool[sidesToCheck.Length];
			int legalCells =0;
			int lengthX = array.GetLength(0);
			int lengthY = array.GetLength(1);
			int cellX = (int)cell.x;
			int cellY = (int)cell.y;
			
			# region checkNulls
			for(int i=0; i < sidesToCheck.Length; i++)
			{
				cellX = (int)cell.x + (int)neighborAccess.directionVectors[(int)sidesToCheck[i]].x;
				cellY = (int)cell.y + (int)neighborAccess.directionVectors[(int)sidesToCheck[i]].y;
				if(cellX >= 0 && cellY >= 0 && cellX < lengthX && cellY < lengthY)
				{
					isNull[i] = false;
					cells[i] = new Vector2(cellX,cellY);
					legalCells ++;
				}
				else
				{
					isNull[i] = true;
				}
			}
			#endregion
			
			#region buildArray
			Vector2[] finalCells = new Vector2[legalCells];
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
}
