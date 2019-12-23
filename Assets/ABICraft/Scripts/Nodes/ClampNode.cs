using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;
using AbicraftCore;
using AbicraftNodes.Meta;

namespace AbicraftNodes.Math
{
    public class ClampNode : AbicraftValueNode
    {
        [Output] public float value;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public float Value;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public float Min;
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public float Max;

        public override object GetValue(AbicraftNodeExecution e, NodePort port)
        {
            return Mathf.Clamp(
                GetInputValue<float>(e, "Value",   Value),
                GetInputValue<float>(e, "Min", Min),
                GetInputValue<float>(e, "Max", Max)
            );
        }
    }
}