using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;
using AbicraftCore;
using AbicraftNodes.Meta;

namespace AbicraftNodes.Math
{
    public class VectorNode : AbicraftValueNode
    {
        [Output] public Vector3 Vector;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public float X, Y, Z;

        public override object GetValue(AbicraftNodeExecution e, NodePort port)
        {
            return new Vector3
            (
                GetInputValue<float>(e, "X", X),
                GetInputValue<float>(e, "Y", Y),
                GetInputValue<float>(e, "Z", Z)
            );
        }
    }
}