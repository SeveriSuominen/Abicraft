using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace AbicraftNodes.Math
{
    public class MultiplyVectorNode : AbicraftValueNode
    {
        [Output] public Vector3 vector;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public Vector3 Vector;
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public float   Multiplier;

        public override object GetValue(NodePort port)
        {
            return GetInputValue<Vector3>("Vector", Vector) * GetInputValue<float>("Multiplier", Multiplier);
        }
    }
}