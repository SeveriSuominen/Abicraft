using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace AbicraftNodes.Math
{
    public class TowardsCursorNode : AbicraftValueNode
    {
        [Output] public Vector3 direction;

        public override void Evaluate(AbicraftAbilityExecution.AbicraftNodeExecution execution)
        {
            AbiCraftStateSnapshot snapshot = execution.AbilityExecution.initial_snapshot;
            direction = (snapshot.mousePosition3D - snapshot.player.transform.position).normalized;

            Debug.Log("from Evaluate: " + direction);
        }

        public override object GetValue(NodePort port)
        {
            return direction;
        }
    }
}