using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;
using AbicraftCore;

namespace AbicraftNodes.Math
{
    public class MultiplyNode : AbicraftValueNode
    {
        [Output] public float value;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public float Value;
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public float   Multiplier;

        public override object GetValue(AbicraftNodeExecution e, NodePort port)
        {
            return GetInputValue<float>(e, "Value", Value) * GetInputValue<float>(e, "Multiplier", Multiplier);
        }
    }
}