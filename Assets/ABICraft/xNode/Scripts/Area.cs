using AbicraftNodes.Meta;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbicraftNodeEditor
{
    public class Area : ScriptableObject
    {
        public string areaName = "DEFAULT";
        public Rect areaRect;
        public Color color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        public NodeGraph graph;

        public readonly List<AbicraftNode> movingNodes = new List<AbicraftNode>();

        public Rect activeResizer;
        public string dir;

        public bool dragginArea  = false;
        public bool resisingArea = false;
    }
}

