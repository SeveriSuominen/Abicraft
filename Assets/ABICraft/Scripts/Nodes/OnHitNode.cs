using AbicraftCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace AbicraftNodes.Action
{
    public class OnHitNode : AbicraftExecutionNode
    {
        [Output]
        public AbicraftObject HitObject;

        [Output]
        public Vector3 HitPoint;

        //public bool ignoreSelfType;
        //public string[] ignoreTags;

        public override void Initialize(AbicraftAbilityExecution.AbicraftNodeExecution execution)
        {
            execution.Block();
        }

        public override IEnumerator ExecuteNode(AbicraftAbilityExecution.AbicraftNodeExecution execution)
        {
            AbicraftCore.AbicraftLifeline lifeline = GetInputValue<AbicraftCore.AbicraftLifeline>("In");

            while (lifeline.mono != null)
            {
                if (lifeline.mono.ActionIsComplete)
                {
                    Debug.Log(lifeline.mono.ActionWasSuccess);

                    if (lifeline.mono.ActionWasSuccess == false)
                    {
                        execution.EndExecutionBranch();
                    }
                    else
                    {
                        HitObject =  lifeline.mono.ReturnData() as AbicraftObject;
                        HitPoint  =  lifeline.mono.transform.position;
                    }

                    yield return new WaitForFixedUpdate();

                    execution.ReleaseBlock();
                    break;
                }
                yield return new WaitForFixedUpdate();
            }
            yield return null;
        }

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "Out")
                return GetInputValue<AbicraftLifeline>("In");
            if (port.fieldName == "HitObject")
                return HitObject;
            else
                return HitPoint;
        }

    }
}

