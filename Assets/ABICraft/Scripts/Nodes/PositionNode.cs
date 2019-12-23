using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;
using AbicraftCore;

namespace AbicraftNodes.Object.Getters
{
    public class GetPositionNode : AbicraftValueNode
    {
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AbicraftObject Obj;

        [Output] public Vector3 ObjPosition;
 
        public override object GetValue(AbicraftNodeExecution e, NodePort port)
        {
            AbicraftObject obj = GetInputValue<AbicraftObject>(e, "Obj");

            if (obj && port.fieldName.Equals("ObjPosition"))
                return obj.transform.position;
            else
                return Vector3.zero;
        }
    }
}