using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;
using AbicraftCore;

namespace AbicraftNodes.Math
{
    public class DivideNode : AbicraftValueNode
    {
        [Output] public float value;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public float Value;
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public float Divider;

        public override object GetValue(AbicraftNodeExecution e, NodePort port)
        {
            if(Divider != 0)
                return GetInputValue<float>(e, "Value", Value) / GetInputValue<float>(e, "Divider", Divider);
            return 0;
        }
    }
}