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
    [System.Serializable]
    public class Any
    {
        public dynamic Value;
        public Type Type;
    }

    public class SetVariableNode : AbicraftExecutionNode
    {
        public string VariableName;
        public bool   SetGlobalVariable;

        [HideInInspector]
        public string lastVariableName;

        [HideInInspector]
        public bool   lastGlobalVariableSetting;

        [HideInInspector]
        public bool   definitionsNeedUpdating;

        [Input(connectionType = ConnectionType.Override)]
        public Any Value;

        public string GetVariableName()
        {
            return VariableName;
        }

        public Type GetDefitionType()
        {
            NodePort port = GetInputPort("Value");

            if (port.Connection != null)
            {
                return port.Connection.ValueType;
            }
            return null;
        }

        public override IEnumerator ExecuteNode(AbicraftNodeExecution e)
        {
            object value = GetInputValue<object>(e, "Value");
            Type type    = GetDefitionType();

            if (value != null && type != null)
            {
                if (SetGlobalVariable)
                {
                    AbicraftGlobalContext.GlobalVariables.Set(e.ae, VariableName, value, GetDefitionType());
                }
                else
                {
                    e.ae.variables.Set(e.ae, VariableName, value, GetDefitionType());
                }
            }
            yield return null;
        }
    }
}