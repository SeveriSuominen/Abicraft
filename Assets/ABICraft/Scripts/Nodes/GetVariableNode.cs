using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;
using AbicraftCore;
using AbicraftMonos;
using AbicraftNodes.Meta;
using AbicraftCore.Variables;
using System;

namespace AbicraftNodes.Action
{

    public class GetVariableNode : AbicraftValueNode
    {
        [HideInInspector]
        public int    selectedIndex, lastVariableCount;

        [HideInInspector]
        public string selectedVariable;

        [HideInInspector]
        public AbicraftAbility getFromAbilityGlobal, lastSelectedAbilityGlobal;

        public bool GetGlobalVariable;

        [HideInInspector]
        public bool lastGetGlobalVariableSetting;

        public override object GetValue(AbicraftNodeExecution e, NodePort port)
        {
            if (e != null)
            {
                var index = selectedIndex - 1;

                if (index >= 0)
                {
                    if (GetGlobalVariable)
                    {                     
                        return AbicraftGlobalContext.GlobalVariables[AbicraftGlobalContext.abicraft.dataFile.GlobalVariableDefinitions[selectedIndex - 1].VARIABLE_NAME];
                    }
                    else
                    {
                        return e.ae.variables[graph.variableDefinitions[selectedIndex - 1].VARIABLE_NAME];
                    }
                }
                else
                    return default;
            }
            return default;
        }
    }
}