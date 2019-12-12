using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace AbicraftNodes.Math
{
    public class FloatNode : AbicraftValueNode
    {
        [Output] public float Value;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public float value;

        public override object GetValue(NodePort port)
        {
            return value;
        }
    }
}