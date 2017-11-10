using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshBuilder {

   
    public class meshFactory {

        protected List<Vector3> verts = new List<Vector3>();
        protected List<int>[] tris = new List<int>[1]; //for more submeshes change the array size indicator
        protected List<Vector2> uv = new List<Vector2>();

        public meshFactory()
        {
            tris = new List<int>[1];

            for (int i = 0; i < tris.Length; i++)
            {
                tris[i] = new List<int>();
            }
        }

        public void addQuad(Vector3[] quadVerts, int submesh)
        {
            if (submesh > tris.Length || submesh < 0)
                Debug.Log(this + " Does not have room for that submesh");

            verts.AddRange(quadVerts);
            //210
            tris[submesh].Add(verts.Count - 2);
            tris[submesh].Add(verts.Count - 3);
            tris[submesh].Add(verts.Count - 4);
            //231
            tris[submesh].Add(verts.Count - 2);
            tris[submesh].Add(verts.Count - 1);
            tris[submesh].Add(verts.Count - 3);
        }

        public void uvLast(Vector2[] uvs)
        {
            uv.AddRange(uvs);
        }

        public void addQuad(Vector3[] quadVerts)
        {
            addQuad(quadVerts, 0);
        }

        public Mesh compileMesh()
        {
            Mesh myMesh = new Mesh();
            myMesh.name = "ProcedralMesh";
            myMesh.subMeshCount = tris.Length;

            if (verts.Count == 0)
            {
                Debug.LogError("sorry verts are empty" + this);
            }

            myMesh.vertices = verts.ToArray();

            for (int i = 0; i < tris.Length; i++)
            {
                myMesh.SetTriangles(tris[i].ToArray(), i);
            }

            if (uv.Count > 0)
            {
                myMesh.uv = uv.ToArray();
            }

            myMesh.RecalculateNormals();
            myMesh.RecalculateBounds();
            return myMesh;
        }
    }
}
