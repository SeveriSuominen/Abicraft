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
            Type type = GetDefitionType();

            if (value != null && type != null)
            {
                e.ae.variables.Set(VariableName, value, GetDefitionType());
            }
          
            //Convert.ChangeType(e.ae.variables[VariableName], GetDefitionType()));

            yield return null;
        }
    }
}