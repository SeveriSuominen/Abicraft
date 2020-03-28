using AbicraftCore.Variables;
using AbicraftMonos;
using AbicraftNodes.Meta;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AbicraftCore
{
    public static class AbicraftGlobalContext
    {
        public static Abicraft abicraft { get { return GetAbicraftInstance(); } private set { } }
        static Abicraft abicraftInstance;

        public static readonly List<AbicraftObject> AllObjects = new List<AbicraftObject>();
        public static AbicraftAbilityVariableMap GlobalVariables = new AbicraftAbilityVariableMap();

        static List<AbicraftAbility> LoadAllAbilityGraphs()
        {
            List<AbicraftAbility> abilityGraphs = new List<AbicraftAbility>();

            #if UNITY_EDITOR
            string[] temp = AssetDatabase.GetAllAssetPaths();
            List<string> resultPaths = new List<string>();

            foreach (string s in temp)
            {
                if (s.Contains(".asset"))
                {
                    AbicraftAbility abilityGraph = AssetDatabase.LoadAssetAtPath<AbicraftAbility>(s);

                    if (abilityGraph)
                    {
                        abilityGraphs.Add(abilityGraph);
                    }
                }
            }
            #endif

            return abilityGraphs;
        }

        public static void UpdateGlobalVariableDefinitions()
        {
            var graphs = LoadAllAbilityGraphs();

            if (graphs.Count == 0)
                return;

            abicraft.dataFile.GlobalVariableDefinitions.Clear();

            for (int i = 0; i < graphs.Count; i++)
            {
                var graph = graphs[i];

                for (int j = 0; j < graph.nodes.Count; j++)
                {
                    AbicraftNode node = graph.nodes[j];

                    if (node && node.GetType() == typeof(AbicraftNodes.Action.SetVariableNode))
                    {
                        AbicraftNodes.Action.SetVariableNode variable = node as AbicraftNodes.Action.SetVariableNode;
                        var varDef = new AbicraftCore.Variables.AbicraftAbilityVariableDefinition(variable.GetVariableName(), variable.GetDefitionType());
                       
                        if (variable.SetGlobalVariable)
                        {
                            abicraft.dataFile.GlobalVariableDefinitions.Add(varDef);
                        }
                    }
                }
            }
        }

        static Abicraft GetAbicraftInstance()
        {
            return abicraftInstance;
        }

        public static void AddAbicraftInstance(Abicraft abicraft)
        {
            abicraftInstance = abicraft;
        }

        public static AbicraftObject FindObject(string name)
        {
            foreach (var obj in AllObjects)
            {
                if (obj.InstantiateObjectToPool)
                {
                    if (obj.ActivePool && obj.name == name)
                        return obj;
                }
                else
                {
                    if (obj.gameObject.activeSelf && obj.name == name)
                        return obj;
                }
            }

            GameObject obj_find_editor = GameObject.Find(name);

            if (obj_find_editor)
                return obj_find_editor.GetComponent<AbicraftObject>();

            return null;
        }

        public static AbicraftObject FindObject(GameObject gameObj)
        {
            return FindObject(gameObj ? gameObj.name : "");
        }
    }
}

