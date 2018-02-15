using UnityEngine;
using System.Collections;

/// <summary>
/// used to query array neighbours mainly 2d arrays
/// </summary>
namespace gridLib
{
    public static class neighborAccess {

        public enum direction : int {
            UP = 0, DOWN = 1, LEFT = 2, RIGHT = 3, UP_LEFT = 4, UP_RIGHT = 5, DOWN_LEFT = 6, DOWN_RIGHT = 7
        }

        static readonly Vector2[] directionVectors = new Vector2[8]{
            Vector2.up,Vector2.down,Vector2.left,Vector2.right,
            Vector3.up+Vector3.left,Vector3.up+Vector3.right,
            Vector2.down + Vector2.left, Vector2.down + Vector2.left
        };

        public static readonly Vector3[] directionVectors3D = getNeigborIndicies(false); //vector 3 offests for indexes for all cells neighbouring a cell
        public static readonly Vector3[] directionVectors3DDiag = getNeigborIndicies(true); //vector 3 offests for indexes for all cells neighbouring a cell indcluding diagonals

        //pointers are needed for this aporach to alow the retured values to be editable

        static public Vector2[] getNeigborsNESW<T>(ref T[,] array, Vector2 cell) //no flattened array implemenation as of now
        {
            return getNeigbors(ref array, cell, new direction[4] { direction.UP, direction.RIGHT, direction.DOWN, direction.LEFT });
        }
        static public Vector2[] getNeigborsNESWDiag<T>(ref T[,] array, Vector2 cell) //no flattened array implemenation as of now
        {
            return getNeigbors(ref array, cell, new direction[8] { direction.UP, direction.UP_RIGHT, direction.RIGHT, direction.DOWN_RIGHT, direction.DOWN, direction.DOWN_LEFT, direction.LEFT, direction.UP_LEFT });
        }
        static public Vector2[] getNeigbors<T>(ref T[,] array, Vector2 cell, direction[] sidesToCheck) //no flattened array implemenation as of now
        {
            Vector2[] cells = new Vector2[sidesToCheck.Length];
            bool[] isNull = new bool[sidesToCheck.Length];
            int legalCells = 0;
            int lengthX = array.GetLength(0);
            int lengthY = array.GetLength(1);
            int cellX = (int)cell.x;
            int cellY = (int)cell.y;

            #region checkNulls
            for (int i = 0; i < sidesToCheck.Length; i++) {
                cellX = (int)cell.x + (int)neighborAccess.directionVectors[(int)sidesToCheck[i]].x;
                cellY = (int)cell.y + (int)neighborAccess.directionVectors[(int)sidesToCheck[i]].y;
                if (cellX >= 0 && cellY >= 0 && cellX < lengthX && cellY < lengthY) {
                    isNull[i] = false;
                    cells[i] = new Vector2(cellX, cellY);
                    legalCells++;
                }
                else {
                    isNull[i] = true;
                }
            }
            #endregion

            #region buildArray
            Vector2[] finalCells = new Vector2[legalCells];
            int cellsIndex = 0;
            for (int i = 0; i < sidesToCheck.Length; i++) {
                if (isNull[i] == false) {
                    finalCells[cellsIndex] = cells[i];
                    cellsIndex++;
                }
            }
            #endregion
            return finalCells;
        }

        static private Vector3[] getNeigborIndicies(bool diag) // use 3 for diagonals 2 for non diagonal neighbours
        {
            int magnatude;
            Vector3[] indexes; 
            if (diag) {
                magnatude = 2;
                indexes = new Vector3[26];
            }
            else {
                magnatude = 1;
                indexes = new Vector3[6];
            }

            int index = 0;
            for (int x = -1; x < 2; x++) {
                for (int y = -1; y < 2; y++) {
                    for (int z = -1; z < 2; z++) {
                        if (Mathf.Abs(x) + Mathf.Abs(y) + Mathf.Abs(z) <= magnatude &&  !(x == 0 && y == 0 && z == 0)) {
                            indexes[index] = new Vector3(x, y, z);
                            index++;
                        }
                    }
                }
            }
            return indexes;
        }
    }

    public static class multiDimUtill<T>
    {
        //cant pass multi dimentional arrays through T[] needs fix

        public static bool inBounds(T[] items,params int[] index)
        {
            int dimentions = items.Rank;
            if (index.Length != dimentions ) { //if the amount of indexes dont match the dimentions throw an error
                System.Console.Error.WriteLine(index +" dosnt match "+items.ToString());
                return false;
            }

            for (int i = 0; i < dimentions; i++)
            {
                if (index[i] < 0 || i >= index[items.GetLength(i)]) 
                {
                    return false;
                }


            }

            return true;
            
        }

        public static bool tryDo(ref T[] items, action passAction, params int[] index)
        {
            if (inBounds(items, index))
            {
                passAction((T)(items.GetValue(index)));
                return true;
            }
            return false;
        }

        public static bool tryDo(ref T[] items, action passAction,action failAction,evaluateItem evaluation, params int[] index)
        {
            if (inBounds(items,index)) //if the item out of bounds dont try do anything to it
            {
                T item = (T)(items.GetValue(index));
                if (evaluation(item))
                {
                    passAction(item); //if it passes do the pass action to it
                    return true;
                }
                else
                {
                    failAction(item); //if it fails do the fail action to it
                    return false;
                }
            }

            return false;
        }   
                
        public delegate bool evaluateItem(T item);
        public delegate void action(T item);
    }
}