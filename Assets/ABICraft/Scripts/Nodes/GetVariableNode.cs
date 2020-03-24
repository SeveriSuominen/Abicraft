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
        public int selectedIndex;

        public override object GetValue(AbicraftNodeExecution e, NodePort port)
        {
            if (e != null)
            {
                var index = selectedIndex - 1;

                if (index >= 0)
                {
                    return e.ae.variables[graph.variableDefinitions[selectedIndex - 1].VARIABLE_NAME];
                }
                else
                    return default;
            }
            return default;
        }
    }
}