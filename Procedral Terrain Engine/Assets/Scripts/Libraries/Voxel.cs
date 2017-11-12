using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxel {

    /// <summary>
    /// derive all objects that want to be "cells from this class"
    /// </summary>
    public class voxelCell
    {

        public voxelCell(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public int x = 0, y = 0, z = 0;

        public virtual bool empty()  //for marching squares mesh gen should it be used
        {
            return false;
        } 

    }
    
    public class voxelChunk
    {
        public int x = 0, z = 0;
        public int chunkSize, chunkHeight; //size is for x and z height is for y

        voxelCell[,,] cells;

        public voxelChunk(int x, int z, int chunkSize, int chunkHeight)
        {
            this.x = x;
            this.z = z;

            this.chunkHeight = chunkHeight;
            this.chunkSize = chunkSize;

            cells = new voxelCell[chunkSize, chunkHeight, chunkSize];

            initCells();           

        }

        public Vector2 getPosVec2()
        {
            return new Vector2(x, z);
        }

        protected void initCells()
        {
            doToAllCells((ref voxelCell v,int x,int y,int z) => { v = new voxelCell(this.x + x, y,this.z + z); });
        }

        /// <summary>
        /// will run the provided "do cell on all cells"
        /// </summary>
        /// <param name="action"> the action to perform on the cell</param>
        public void doToAllCells(doCell action) {
            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkHeight; y++)
                {
                    for (int z = 0; z < chunkSize; z++)
                    {
                        action(ref cells[x, y, z],x,y,z);
                    }
                }
            }
        }

        /// <summary>
        /// will run all of the "do cell" actions on a cell in order,
        /// all of action 0 will be done to EVERY cell then action 1 ect...
        /// </summary>
        /// <param name="actions"></param>
        public void doToAllCells(doCell[] actions)
        {
            for (int i = 0; i < actions.Length; i++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    for (int y = 0; y < chunkHeight; y++)
                    {
                        for (int z = 0; z < chunkSize; z++)
                        {
                            actions[i](ref cells[x, y, z], x, y, z);
                        }
                    }
                }
            }
        }


    }

    public class voxelWorld
    {
        Dictionary<Vector2, voxelChunk> chunks;
        protected int chunkSize =0, chunkHeight = 0;

        public voxelWorld( int chunkSize = 0, int chunkHeight = 0)
        {
            this.chunkSize = chunkSize;
            this.chunkHeight = chunkHeight;

            chunks = new Dictionary<Vector2, voxelChunk>();
        }

        public void spawnChunk(int x,int z, doChunk chunkInit)
        {

            if (chunks.ContainsKey(new Vector2(x,z)))
            {
                System.Console.Error.WriteLine("x: "+x +"z: " + z+ ": Allready in the the chunk list");
                return;
            }

            voxelChunk chunk = new voxelChunk(x, z, chunkSize, chunkHeight);
           
            chunkInit(ref chunk, x, z); //initalize the chunk                  

            chunks.Add(chunk.getPosVec2(), chunk); //ad the chunk to the list with its position as its key
        }

    }

    /// <summary>
    /// a function interface to easily iterate over all of the cells gives you a reference to the cell and its x,y,z in array space
    /// </summary>
    /// <param name="v"> a refrence to the cell</param>
    /// <param name="x"> the cells x position in the array</param>
    /// <param name="y"> the cells x position in the array</param>
    /// <param name="z"> the cells x position in the array</param>
    public delegate void doCell(ref voxelCell v, int x, int y, int z);
    
    public delegate void doChunk(ref voxelChunk c, int x, int z);

}




