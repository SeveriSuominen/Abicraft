using AbicraftCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

using XNode;

namespace AbicraftNodes.Action
{
    public class LoopNode : AbicraftExecutionLoopNode
    {
        public override void Initialize(AbicraftAbilityExecution.AbicraftNodeExecution execution)
        {
            
        }

        public override IEnumerator ExecuteNode(AbicraftAbilityExecution.AbicraftNodeExecution execution)
        {
            yield return null;
        }
    }
}


