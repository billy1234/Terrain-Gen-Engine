using UnityEngine;
using System.Collections.Generic;

namespace GraphTerrain {

    public class graph {
        public List<node> nodes;

        public graph(node[] nodes) {
            this.nodes = new List<node>(nodes);
        }
    }

    public class node {

        public List<node> connnections;

        public node() {
            connnections = new List<node>();
        }

        public node(List<node> conenctions) {
            connnections = new List<node>(connnections);
        }

        public void connect(params node[] nodes) {

            for (int i = 0; i < nodes.Length; i++) {
                connnections.Add(nodes[i]);
            }
        }
    }
}