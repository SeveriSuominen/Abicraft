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
        [Input(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public Vector3 VectorIn;
        [Output]
        public Vector3 VectorOut;

        [Input(backingValue = ShowBackingValue.Unconnected, connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public float XIn, YIn, ZIn;
        [Output]
        public float XOut, YOut, ZOut;

        [HideInInspector]
        public bool lastConnectionStatus;

        public override object GetValue(AbicraftNodeExecution e, NodePort port)
        {
            switch (port.fieldName)
            {
                case "VectorOut":
                    if (GetInputPort("VectorIn").IsConnected)
                    {
                        return GetInputValue<Vector3>(e, "VectorIn", VectorIn);
                    }
                    else
                    {
                       return new Vector3
                       (
                           GetInputValue<float>(e, "XIn", XIn),
                           GetInputValue<float>(e, "YIn", YIn),
                           GetInputValue<float>(e, "ZIn", ZIn)
                       );
                    }
                case "XOut":
                    return GetInputValue<Vector3>(e, "VectorIn", VectorIn).x;
                case "YOut":
                    return GetInputValue<Vector3>(e, "VectorIn", VectorIn).y;
                case "ZOut":
                    return GetInputValue<Vector3>(e, "VectorIn", VectorIn).z;
            }
            return default;
        }
    }
}