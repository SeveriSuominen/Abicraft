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
    public class AnimationSetBooleanNode : AbicraftExecutionNode
    {
        public static readonly List<AbicraftObject> IsAnimating = new List<AbicraftObject>();

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AbicraftObject Obj;
        private AbicraftObject obj;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public string BooleanName;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public bool SetTo;

        Animator animator;

        public override void Initialize(AbicraftNodeExecution execution)
        {
           
        }

        public override IEnumerator ExecuteNode(AbicraftNodeExecution e)
        {
            obj = GetInputValue<AbicraftObject>(e, "Obj");
            animator = obj.GetComponent<Animator>();

            if (obj)
            {
                if (animator != null)
                {

                    animator.SetBool(GetInputValue<string>(e, "BooleanName", BooleanName), GetInputValue<bool>(e, "SetTo", SetTo));
                }
            }
            yield return null;
        }
    }
}


