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
        int x = 0, y = 0, z = 0;
        int chunkSize, chunkHeight; //size is for x and z height is for y

        voxelCell[,,] cells;

        public voxelChunk(int x, int y, int z, int chunkSize, int chunkHeight)
        {
            this.x = x;
            this.y = y;
            this.z = z;

            this.chunkHeight = chunkHeight;
            this.chunkSize = chunkSize;

            cells = new voxelCell[chunkSize, chunkHeight, chunkSize];

            initCells();           

        }

        public voxelChunk(int x, int y, int z, int chunkSize, int chunkHeight,doCell initalization)
        {
            this.x = x;
            this.y = y;
            this.z = z;

            this.chunkHeight = chunkHeight;
            this.chunkSize = chunkSize;

            cells = new voxelCell[chunkSize, chunkHeight, chunkSize];

            doToAllCells(initalization);
            
        }

        protected void initCells() {

            doToAllCells((ref voxelCell v,int x,int y,int z) => { v = new voxelCell(this.x + x,this.y + y,this.z + z); });
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

        public static Mesh buildMesh() {
            Mesh m = new Mesh();

            return m;
        }
    }

    /// <summary>
    /// a function interface to easily iterate over all of the cells gives you a reference to the cell and its x,y,z in array space
    /// </summary>
    /// <param name="v"> a refrence to the cell</param>
    /// <param name="x"> the cells x position in the array</param>
    /// <param name="y"> the cells x position in the array</param>
    /// <param name="z"> the cells x position in the array</param>
    public delegate void doCell(ref voxelCell v,int x, int y, int z);

    


}




