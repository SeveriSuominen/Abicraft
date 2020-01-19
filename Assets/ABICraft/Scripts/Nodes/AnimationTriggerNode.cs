using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

using AbicraftNodeEditor;
using AbicraftCore;
using AbicraftMonos;
using AbicraftNodes.Meta;

namespace AbicraftNodes.Action
{
    public class AnimationTriggerNode : AbicraftExecutionNode
    {
        public static readonly List<AbicraftObject> IsAnimating = new List<AbicraftObject>();

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AbicraftObject Obj;
        private AbicraftObject obj;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public string TriggerName;
        public bool ClearNavMeshAgentDestination;
        Animator animator;

        public override void Initialize(AbicraftNodeExecution execution)
        {
           
        }

        public override IEnumerator ExecuteNode(AbicraftNodeExecution e)
        {
            obj = GetInputValue<AbicraftObject>(e, "Obj");
            animator = obj.GetComponent<Animator>();

            if (obj && ClearNavMeshAgentDestination)
            {
                var navAgent = obj.GetComponent<NavMeshAgent>();
                if(navAgent)
                    navAgent.ResetPath();
            }


            if (obj)
            {
                if (animator != null)
                {
                    animator.speed = 1;
                    animator.SetTrigger(GetInputValue<string>(e, "TriggerName", TriggerName));
                }
            }
            yield return null;
        }
    }
}


