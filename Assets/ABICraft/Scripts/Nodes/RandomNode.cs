using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;
using AbicraftCore;
using AbicraftNodes.Meta;

namespace AbicraftNodes.Math
{
    public class RandomNode : AbicraftValueNode
    {
        [Output] public float Value;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public float Min, Max;

        public override object GetValue(AbicraftNodeExecution e, NodePort port)
        {
            return Random.Range(GetInputValue<float>(e, "Min", Min), GetInputValue<float>(e, "Max", Max));
        }
    }
}