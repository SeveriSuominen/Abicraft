using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace AbicraftNodes.Math
{
    public class TowardsObjectDirectionNode : AbicraftValueNode
    {

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AbicraftObject ObjFrom;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AbicraftObject ObjTo;

        [Output] public Vector3 direction;

        public bool onlyYAxis;

        private Vector3 mouseposition;

        public override void Evaluate(AbicraftAbilityExecution.AbicraftNodeExecution execution)
        {
            AbicraftObject objTo   = GetInputValue<AbicraftObject>("ObjTo");
            AbicraftObject objFrom = GetInputValue<AbicraftObject>("ObjFrom");

            if (objTo != null && objFrom != null)
            {
                direction = (objTo.transform.position - objFrom.transform.position).normalized;

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