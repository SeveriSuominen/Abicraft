using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace AbicraftNodes.Object.Getters
{
    public class GetPositionNode : AbicraftValueNode
    {
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AbicraftObject Obj;

        [Output] public Vector3 ObjPosition;
 
        public override object GetValue(NodePort port)
        {
            AbicraftObject obj = GetInputValue<AbicraftObject>("Obj");

            if (obj && port.fieldName.Equals("ObjPosition"))
                return obj.transform.position;
            else
                return Vector3.zero;
        }
    }
}