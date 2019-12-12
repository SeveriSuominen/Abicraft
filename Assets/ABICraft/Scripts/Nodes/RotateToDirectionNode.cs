using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace AbicraftNodes.Action
{
    public class RotateToDirectionNode : AbicraftExecutionNode
    {
        public static uint id = 115;

        [Input(typeConstraint = TypeConstraint.Strict, backingValue = ShowBackingValue.Never)]
        public AbicraftObject Obj;

        [Input(typeConstraint = TypeConstraint.Strict, backingValue = ShowBackingValue.Never)]
        public Vector3 direction;

      
        public override void Initialize(AbicraftAbilityExecution.AbicraftNodeExecution execution)
        {

        }

        public override IEnumerator ExecuteNode(AbicraftAbilityExecution.AbicraftNodeExecution execution)
        {
            AbiCraftStateSnapshot context = AbiCraftStateSnapshot.TakeSnapshot;

            var lookPos = GetInputValue<Vector3>("direction", Vector3.zero);
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);

            AbicraftObject obj = GetInputValue<AbicraftObject>("Obj");

            if (obj)
                obj.transform.rotation = rotation;

            yield return null;
        }
    }
}

