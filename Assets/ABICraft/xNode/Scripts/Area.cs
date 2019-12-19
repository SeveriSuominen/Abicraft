using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbicraftNodeEditor
{
    public class Area : ScriptableObject
    {
        public string areaName = "DEFAULT";
        public Rect areaRect;
        public Color color;
        public NodeGraph graph;
    }
}

