using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;

namespace AbicraftNodes.Math
{
    public class TowardsObjectNode : AbicraftValueNode
    {

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AbicraftObject ObjFrom;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AbicraftObject ObjTo;

        [Output] public Vector3 direction;
        [Output] public float distance;

        public bool onlyYAxis;

        private Vector3 mouseposition;

        public override void Evaluate(AbicraftNodeExecution e)
        {
            AbicraftObject objTo   = GetInputValue<AbicraftObject>(e, "ObjTo");
            AbicraftObject objFrom = GetInputValue<AbicraftObject>(e, "ObjFrom");

            if (objTo != null && objFrom != null)
            {
                distance  = Vector3.Distance(objTo.transform.position, objFrom.transform.position);
                direction = (objTo.transform.position - objFrom.transform.position).normalized;

                if (onlyYAxis)
                    direction.y = 0;
            }
        }

        public override object GetValue(AbicraftNodeExecution e, NodePort port)
        {
            if (port.fieldName.Equals("distance"))
                return distance;
            else
                return direction;
        }
    }
}