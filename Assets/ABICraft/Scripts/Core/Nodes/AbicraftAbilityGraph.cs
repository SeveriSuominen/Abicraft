using AbicraftCore.Variables;
using AbicraftNodes.Meta;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AbicraftNodeEditor {
    /// <summary> Base class for all node graphs </summary>
    [Serializable]
    public abstract class AbicraftAbilityGraph : ScriptableObject {

        /// <summary> All nodes in the graph. <para/>
        /// See: <see cref="AddNode{T}"/> </summary>
        [SerializeField] public List<AbicraftNode> nodes = new List<AbicraftNode>();
        [SerializeField] public List<Area> areas = new List<Area>();

        [SerializeField]
        public List<AbicraftAbilityVariableDefinition> variableDefinitions = new List<AbicraftAbilityVariableDefinition>();

        public Texture2D icon;
        public string AbilityName;
        /// <summary> Ability cooldown, min time between ability casts </summary>
        public float  Cooldown;
        public bool   Passive;
        public float  DefaultLifetime;

        /// <summary> Add a node to the graph by type (convenience method - will call the System.Type version) </summary>
        public T AddNode<T>() where T : AbicraftNode {
            return AddNode(typeof(T)) as T;
        }

        /// <summary> Add a node to the graph by type </summary>
        public virtual AbicraftNode AddNode(Type type) {
            AbicraftNode.graphHotfix = this;
            AbicraftNode node = ScriptableObject.CreateInstance(type) as AbicraftNode;
            node.graph = this;
            nodes.Add(node);
            return node;
        }

        /// <summary> Remove all node graph areas </summary>
        /// <param name="area"> Node graph area to remove</param>
        public virtual void RemoveArea(Area area)
        {
            for (int i = 0; i < areas.Count; i++)
            {
                if(areas[i] == area)
                {
                    areas.RemoveAt(i);
                    if (Application.isPlaying) DestroyImmediate(area, true);
                    break;
                }                
            }
        }

        /*
       /// <summary> Remove all node graph areas </summary>
       /// <param name="node"> Remove all node graph areas</param>
        public virtual void RemoveAllAreas()
        {
           for (int i = 0; i < areas.Count; i++)
           {
               Area area = areas[i];

               areas.RemoveAt(i);
               if (Application.isPlaying) DestroyImmediate(area, true);
           }
        }*/

        /// <summary> Add a node to the graph by type </summary>
        public virtual Area AddArea(Type type)
        {
            Area area = ScriptableObject.CreateInstance(typeof(Area)) as Area;
            area.graph = this;
            areas.Add(area);
            return area;
        }

        /// <summary> Creates a copy of the original node in the graph </summary>
        public virtual AbicraftNode CopyNode(AbicraftNode original) {
            AbicraftNode.graphHotfix = this;
            AbicraftNode node = ScriptableObject.Instantiate(original);
            node.graph = this;
            node.ClearConnections();
            nodes.Add(node);
            return node;
        }

        /// <summary> Safely remove a node and all its connections </summary>
        /// <param name="node"> The node to remove </param>
        public virtual void RemoveNode(AbicraftNode node) {
            node.ClearConnections();
            nodes.Remove(node);
            if (Application.isPlaying) DestroyImmediate(node, true);
        }

        /// <summary> Remove all nodes and connections from the graph </summary>
        public virtual void Clear() {
            if (Application.isPlaying) {
                for (int i = 0; i < nodes.Count; i++) {
                    Destroy(nodes[i]);
                }
            }
            nodes.Clear();
        }

        /// <summary> Create a new deep copy of this graph </summary>
        public virtual AbicraftAbilityGraph Copy() {
            // Instantiate a new nodegraph instance
            AbicraftAbilityGraph graph = Instantiate(this);
            // Instantiate all nodes inside the graph
            for (int i = 0; i < nodes.Count; i++) {
                if (nodes[i] == null) continue;
                AbicraftNode.graphHotfix = graph;
                AbicraftNode node = Instantiate(nodes[i]) as AbicraftNode;
                node.graph = graph;
                graph.nodes[i] = node;
            }

            // Redirect all connections
            for (int i = 0; i < graph.nodes.Count; i++) {
                if (graph.nodes[i] == null) continue;
                foreach (NodePort port in graph.nodes[i].Ports) {
                    port.Redirect(nodes.Cast<AbicraftNode>().ToList(), graph.nodes.Cast<AbicraftNode>().ToList());
                }
            }

            return graph;
        }

        protected virtual void OnDestroy() {
            // Remove all nodes prior to graph destruction
            Clear();
        }
    }
}