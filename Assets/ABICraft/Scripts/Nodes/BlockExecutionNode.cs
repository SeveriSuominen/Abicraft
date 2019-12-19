using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

using AbicraftNodeEditor;

namespace AbicraftNodes.Action
{
    public class BlockExecutionNode : AbicraftExecutionNode
    {
        public static uint id = 111;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public float BlockForSeconds;

        public override void Initialize(AbicraftNodeExecution execution)
        {
            execution.Block();
        }

        public override IEnumerator ExecuteNode(AbicraftNodeExecution e)
        {
            float blockForSeconds = GetInputValue<float>(e, "BlockForSeconds", BlockForSeconds);
            if (blockForSeconds >= 0.01f)
                yield return new WaitForSeconds(blockForSeconds);
            e.ReleaseBlock();
        }
    }
}


