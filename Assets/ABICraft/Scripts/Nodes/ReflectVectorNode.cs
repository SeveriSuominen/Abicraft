using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;

namespace AbicraftNodes.Math
{
    public class ReflectVectorNode : AbicraftValueNode
    {
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public Vector3 Vector;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public Vector3 Normal;

        [Output]
        public Vector3 ReflectedVector;

        public override object GetValue(AbicraftNodeExecution e, NodePort port)
        {
            Vector3 original = GetInputValue<Vector3>(e, "Vector");
            return Vector3.Reflect(original, GetInputValue<Vector3>(e, "Normal", Normal));
        }
    }
}