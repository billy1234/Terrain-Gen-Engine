using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphTerrain;

public class Gterrain : MonoBehaviour {
    public graph g;

 
	
	void Start () {

        List<worldNode> nodes = new List<worldNode>(10);
        for (int i = 0; i < 10; i++) {
            nodes.Add(new worldNode(new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f))));
            nodes[i].spawn();
            if (i > 0) {
                nodes[i - 1].connect(nodes[i]);
            }
            if (i > 1) {
                nodes[i - 2].connect(nodes[i]);
            }
        }
        g = new graph(nodes.ToArray());
    }
	
	// Update is called once per frame
	void OnDrawGizmos () {
        if (Application.isPlaying) {
            for (int i = 0; i < g.nodes.Count; i++) {
                ((worldNode)g.nodes[i]).draw();
            }
        }
	}

    public class worldNode : node {
        public Vector3 position;
        GameObject worldInstance;

        public worldNode(Vector3 position) : base() {
            this.position = position;
        }

        public void spawn() {
            if (worldInstance == null) {
                worldInstance = GameObject.CreatePrimitive(PrimitiveType.Cube);
                worldInstance.transform.position = position;
            }
        }

        public void draw() {
            if (Application.isEditor) {
                for (int i = 0; i < connnections.Count; i++)
                    Debug.DrawLine(position, ((worldNode)connnections[i]).position);
            }
        }
    }
}
