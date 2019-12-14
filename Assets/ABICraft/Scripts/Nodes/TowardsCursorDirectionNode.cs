using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace AbicraftNodes.Math
{
    public class TowardsCursorDirectionNode : AbicraftValueNode
    {

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AbicraftObject Obj;
        [Output] public Vector3 direction;

        public bool onlyYAxis;

        private Vector3 mouseposition;

        public override void Evaluate(AbicraftNodeExecution e)
        {
            AbicraftObject obj = GetInputValue<AbicraftObject>(e, "Obj");

            if(obj != null)
            {
                AbiCraftStateSnapshot snapshot = e.AbilityExecution.initial_snapshot;
                direction = (snapshot.mousePosition3D - obj.transform.position).normalized;

                if (onlyYAxis)
                    direction.y = 0;
            }
        }

        public override object GetValue(AbicraftNodeExecution e, NodePort port)
        {
            return direction;
        }
    }
}