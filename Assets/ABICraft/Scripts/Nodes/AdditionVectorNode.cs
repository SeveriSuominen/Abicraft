using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;
using AbicraftCore;
using AbicraftNodes.Meta;

namespace AbicraftNodes.Math
{
    public class AdditionVectorNode : AbicraftValueNode
    {
        [Output] public Vector3 vector;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public Vector3 Vector01;
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public Vector3 Vector02;

        public override object GetValue(AbicraftNodeExecution e, NodePort port)
        {
            return GetInputValue<Vector3>(e, "Vector01", Vector01) + GetInputValue<Vector3>(e, "Vector02", Vector02);
        }
    }
}