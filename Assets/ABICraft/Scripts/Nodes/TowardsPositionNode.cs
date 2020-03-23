using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;
using AbicraftCore;
using AbicraftMonos;
using AbicraftNodes.Meta;

namespace AbicraftNodes.Math
{
    public class TowardsPositionNode : AbicraftValueNode
    {

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AbicraftObject ObjFrom;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public Vector3 PosTo;

        [Output] public Vector3 direction;
        [Output] public float distance;

        public bool onlyYAxis;

        public override object GetValue(AbicraftNodeExecution e, NodePort port)
        {
            AbicraftObject objFrom = GetInputValue<AbicraftObject>(e, "ObjFrom");
            Vector3 posTo = GetInputValue<Vector3>(e, "PosTo");

            if (objFrom != null)
            {
                distance = Vector3.Distance(posTo, objFrom.transform.position);
                direction = (posTo - objFrom.transform.position).normalized;

                if (onlyYAxis)
                    direction.y = 0;
            }

            if (port.fieldName.Equals("distance"))
                return distance;
            else
                return direction;
        }
    }
}