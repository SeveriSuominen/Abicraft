using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace AbicraftNodes.Math
{
    public class CurveNode : AbicraftValueNode
    {
        [Output] public float Value;

        public AnimationCurve curve;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public float value;
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public float add;

        public override object GetValue(AbicraftNodeExecution e, NodePort port)
        {
            return GetInputValue<float>(e, "value", value) + GetInputValue<float>(e, "add", add);
        }
    }
}