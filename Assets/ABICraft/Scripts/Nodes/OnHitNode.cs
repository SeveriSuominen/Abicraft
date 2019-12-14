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

        public override void Initialize(AbicraftNodeExecution execution)
        {
            execution.Block();
        }

        public override IEnumerator ExecuteNode(AbicraftNodeExecution e)
        {
            AbicraftCore.AbicraftLifeline lifeline = GetInputValue<AbicraftLifeline>(e, "In");

            while (lifeline.mono != null)
            {
                if (lifeline.mono.ActionIsComplete)
                {
                    if (lifeline.mono.ActionWasSuccess == false)
                    {
                        e.EndExecutionBranch();
                    }
                    else
                    {
                        HitObject =  lifeline.mono.ReturnData() as AbicraftObject;
                        HitPoint  =  lifeline.mono.transform.position;
                    }

                    yield return new WaitForFixedUpdate();

                    e.ReleaseBlock();
                    break;
                }
                yield return new WaitForFixedUpdate();
            }
            yield return null;
        }

        public override object GetValue(AbicraftNodeExecution e, NodePort port)
        {
            if (port.fieldName == "Out")
                return GetInputValue<AbicraftLifeline>(e, "In");
            if (port.fieldName == "HitObject")
                return HitObject;
            else
                return HitPoint;
        }

    }
}

