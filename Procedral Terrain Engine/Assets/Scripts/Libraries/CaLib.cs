using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using gridLib;
 namespace cellularAutomataLib
{

	public delegate void cellRule<T>(ref T me, ref T[] myNeighbours);

	public abstract class cellularAutomotaBase<TTile,TQuery>
	{
		public TTile[,] cells;
		public cellRule<TTile>[] rules;
		public Dictionary<TQuery,int> ruleDictionary;

		public cellularAutomotaBase(TTile[,] cells, cellRule<TTile>[] rules, Dictionary<TQuery,int> ruleDictionary)
		{
			this.cells = cells;
			this.rules = rules;
			this.ruleDictionary = ruleDictionary;
		}

		//protected abstract void iterateCell(Vector2[] neighbors,int x,int y);

		protected void iterateCell(Vector2[] neighbourIndex,int x,int y) //ints implemenation sipliy plugs the return back into the cell
		{
			cellRule<TTile> rule;
			TTile[] neighbourCells = new TTile[neighbourIndex.Length];
			rule = ruleMatrix<TTile,TQuery>.getRule(
											getRuleQueryFromTile(cells[x,y])
											,ref rules,ref ruleDictionary);
			for(int i=0; i < neighbourIndex.Length; i++)
			{
				neighbourCells[i] = cells[(int)neighbourIndex[i].x,(int)neighbourIndex[i].y];
			
			}
			rule(ref cells[x,y],ref neighbourCells);

			//Debug.Log(getRuleQueryFromTile(cells[x,y]));
		}

		/// <summary>
		/// asks the tile for the data so the dictionary can be quereyed, so the whole object does not have to be passed, impliment based on rule structure
		/// </summary>
		/// <returns>a queryable object.</returns>
		/// <param name="tile">Tile.</param>
		protected abstract TQuery getRuleQueryFromTile(TTile tile);

		public void passNESW()
		{
			pass(true);
		}
		public void passNESWD()
		{
			pass(false);
		}
		public void passCustom(Vector2[] cellsToCheck)//even more passes can be added as itterate cell is sperate
		{
			for(int y =0; y < cells.GetLength(1); y++)
			{
				for(int x =0; x < cells.GetLength(0); x++)
				{
					iterateCell(cellsToCheck,x,y);
				}
				
			}
		}
		
		private void pass(bool NESW) //if not nesw will asume neswdiag, this is a utility function for pass nesw/neswd
		{
			Vector2[] neighbors = new Vector2[8];
			for(int y =0; y < cells.GetLength(1); y++)
			{
				for(int x =0; x < cells.GetLength(0); x++)
				{
					if(NESW)
					{
						neighbors = neighborAccess.getNeigborsNESW(ref cells,new Vector2(x,y));
					}
					else if(NESW == false)
					{
						neighbors = neighborAccess.getNeigborsNESWDiag(ref cells,new Vector2(x,y));
					}
					iterateCell(neighbors,x,y);
				}
				
			}
		}
	}
	

	public class cellularAutomotaInt : cellularAutomotaBase<int,int>
	{

		public cellularAutomotaInt(int[,] cells, cellRule<int>[] rules, Dictionary<int,int> ruleDictionary):base(cells,rules,ruleDictionary)
		{

		}

		protected override int getRuleQueryFromTile (int tile)
		{
			return tile;
		}
		/*
		protected override void iterateCell(Vector2[] neighbors,int x,int y) //ints implemenation sipliy plugs the return back into the cell
		{
			genericDelegate<int> rule;

			foreach(Vector2 cellCoord in neighbors)
			{
				rule = ruleMatrix<int,int>.getRule(cells[x,y],cells[(int)cellCoord.x,(int)cellCoord.y],ref rules,ref ruleDictionary);
				cells[(int)cellCoord.x,(int)cellCoord.y] = rule(cells[x,y]);
			}
		}
		*/

	}


	//t1 is output t2 query tipe(what will be fed into this class to find the apropriate rule)
	public static class ruleMatrix<TCell,TQuery>
	{
		private static int getRuleNumber(TQuery state,ref cellRule<TCell>[] rules,ref Dictionary<TQuery,int> ruleDictionary)
		{
			return(ruleDictionary[state]);
		}

		public static cellRule<TCell> getRule(TQuery state,ref cellRule<TCell>[] rules,ref Dictionary<TQuery,int> ruleDictionary)
		{
			return rules[getRuleNumber(state,ref rules,ref ruleDictionary)];
		}
	}	
}
