using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;

namespace AbicraftNodes.Math
{
    public class TowardsCursorNode : AbicraftValueNode
    {

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AbicraftObject Obj;
        [Output] public Vector3 direction;
        [Output] public float distance;

        public bool onlyYAxis;

        private Vector3 mouseposition;

        public override void Evaluate(AbicraftNodeExecution e)
        {
            AbicraftObject obj = GetInputValue<AbicraftObject>(e, "Obj");

            if(obj != null)
            {
                AbiCraftStateSnapshot snapshot = e.ae.initial_snapshot;
                distance = Vector3.Distance(obj.transform.position, snapshot.mousePosition3D);
                direction = (snapshot.mousePosition3D - obj.transform.position).normalized;

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