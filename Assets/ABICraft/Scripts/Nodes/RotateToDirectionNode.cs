using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace AbicraftNodes.Action
{
    public class RotateToDirectionNode : AbicraftExecutionNode
    {
        public static uint id = 115;

        [Input] public Vector3 direction;

        public override void Initialize(AbicraftAbilityExecution.AbicraftNodeExecution execution)
        {

        }

        public override IEnumerator ExecuteNode(AbicraftAbilityExecution.AbicraftNodeExecution execution)
        {
            Debug.Log("Rotate To Cursor Node");

            AbiCraftStateSnapshot context = AbiCraftStateSnapshot.TakeSnapshot;

            //var lookPos = context.mousePosition3D - context.player.transform.position;

            var lookPos = GetInputValue<Vector3>("direction");
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            context.player.transform.rotation = rotation;

            yield return null;
        }
    }
}

