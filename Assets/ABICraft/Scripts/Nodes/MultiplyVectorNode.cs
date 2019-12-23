using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;
using AbicraftCore;
using AbicraftNodes.Meta;

namespace AbicraftNodes.Math
{
    public class MultiplyVectorNode : AbicraftValueNode
    {
        [Output] public Vector3 vector;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public Vector3 Vector;
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public float   Multiplier;

        public override object GetValue(AbicraftNodeExecution e, NodePort port)
        {
            return GetInputValue<Vector3>(e, "Vector", Vector) * GetInputValue<float>(e, "Multiplier", Multiplier);
        }
    }
}