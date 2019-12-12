using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

using XNode;

namespace AbicraftNodes.Action
{
    public class BlockExecutionNode : AbicraftExecutionNode
    {
        public static uint id = 111;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public float BlockForSeconds;

        public override void Initialize(AbicraftAbilityExecution.AbicraftNodeExecution execution)
        {
            execution.Block();
        }

        public override IEnumerator ExecuteNode(AbicraftAbilityExecution.AbicraftNodeExecution execution)
        {
            if(BlockForSeconds >= 0.05f)
                yield return new WaitForSeconds(BlockForSeconds);
            execution.ReleaseBlock();
        }
    }
}


