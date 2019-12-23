using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;
using AbicraftCore;

namespace AbicraftNodes.Action
{
    public class RotateToDirectionNode : AbicraftExecutionNode
    {

        [Input(typeConstraint = TypeConstraint.Strict, backingValue = ShowBackingValue.Never)]
        public AbicraftObject Obj;

        [Input(typeConstraint = TypeConstraint.Strict, backingValue = ShowBackingValue.Never)]
        public Vector3 direction;

      
        public override void Initialize(AbicraftNodeExecution execution)
        {

        }

        public override IEnumerator ExecuteNode(AbicraftNodeExecution e)
        {
            AbicraftGameStateSnapshot context = AbicraftGameStateSnapshot.TakeSnapshot;

            var lookPos = GetInputValue<Vector3>(e, "direction", Vector3.zero);
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);

            AbicraftObject obj = GetInputValue<AbicraftObject>(e, "Obj");

            if (obj)
                obj.transform.rotation = rotation;

            yield return null;
        }
    }
}

