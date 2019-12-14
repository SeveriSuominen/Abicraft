using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace AbicraftNodes.Object
{
    public class AbicraftObjectFindNode : AbicraftValueNode
    {
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public string Name;

        [Output]
        public AbicraftObject Obj;
        GameObject obj;

        public override void Evaluate(AbicraftAbilityExecution.AbicraftNodeExecution execution)
        {
            base.Evaluate(execution);
        }

        public override object GetValue(NodePort port)
        {
            AbicraftObject obj = AbicraftGlobalContext.FindObject(Name);

            if (obj)
            {
                AddDynamicOutput(typeof(Vector3), ConnectionType.Override, TypeConstraint.Strict, "Position");
                AddDynamicOutput(typeof(Vector3), ConnectionType.Override, TypeConstraint.Strict, "Rotation");
            }
            else
            {
                RemoveDynamicPort("Position");
                RemoveDynamicPort("Rotation");
                RemoveDynamicPort("Testing");
            }

            if(obj != null)
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