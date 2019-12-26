using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;
using AbicraftCore;
using AbicraftMonos;
using AbicraftNodes.Meta;

namespace AbicraftNodes.Object
{
    public class SenderObjectNode : AbicraftValueNode
    {
        //[Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        //public string Name;

        [Output]
        public AbicraftObject Obj;

        [Output]
        public Vector3 Position, Rotation;

        GameObject obj;

        public override void Evaluate(AbicraftNodeExecution execution)
        {
            base.Evaluate(execution);
        }

        public override object GetValue(AbicraftNodeExecution e, NodePort port)
        {
            AbicraftObject obj = e.ae.senderObject;

            if (obj)
            {
                //AddDynamicOutput(typeof(Vector3), ConnectionType.Override, TypeConstraint.Strict, "Position");
                //AddDynamicOutput(typeof(Quaternion), ConnectionType.Override, TypeConstraint.Strict, "Rotation");
            }
            else
            {
               //RemoveDynamicPort("Position");
               //RemoveDynamicPort("Rotation");
               //RemoveDynamicPort("Testing");
            }

            if (obj != null)
            {
                switch (port.fieldName)
                {
                    case "Rotation":
                        return obj.transform.rotation;
                    case "Position":
                        return obj.transform.position;
                    default:
                        return obj;
                }
            }
            return obj;
        }
    }
}