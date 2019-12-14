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

        public override void Evaluate(AbicraftAbilityExecution.AbicraftNodeExecution execution)
        {
            AbicraftObject obj = GetInputValue<AbicraftObject>("Obj");

            if(obj != null)
            {
                AbiCraftStateSnapshot snapshot = execution.AbilityExecution.initial_snapshot;
                direction = (snapshot.mousePosition3D - obj.transform.position).normalized;

                if (onlyYAxis)
                    direction.y = 0;
            }
        }

        public override object GetValue(NodePort port)
        {
            return direction;
        }
    }
}