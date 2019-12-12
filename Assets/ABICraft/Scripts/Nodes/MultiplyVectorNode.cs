using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace AbicraftNodes.Math
{
    public class AdditionVectorNode : AbicraftValueNode
    {
        [Output] public Vector3 vector;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public Vector3 Vector01;
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public Vector3 Vector02;

        public override object GetValue(NodePort port)
        {
            return GetInputValue<Vector3>("Vector01", Vector01) + GetInputValue<Vector3>("Vector02", Vector02);
        }
    }
}