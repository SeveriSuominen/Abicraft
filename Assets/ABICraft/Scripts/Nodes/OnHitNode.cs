using AbicraftCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;

namespace AbicraftNodes.Action
{
    public class OnHitNode : AbicraftActionReceiverNode
    {
        [Output]
        public AbicraftObject HitObject;
        private Dictionary<string, AbicraftObject> hitObjectsByIteration = new Dictionary<string, AbicraftObject>();

        [Output]
        public Vector3 HitPoint;
        private Dictionary<string, Vector3> hitPointsByIteration = new Dictionary<string, Vector3>();

        [Output]
        public Vector3 Normal;
        private Dictionary<string, RaycastHit> raycastHitIteration = new Dictionary<string, RaycastHit>();

        public override void Initialize(AbicraftNodeExecution execution)
        {
            execution.Block();
        }

        public override IEnumerator ExecuteNode(AbicraftNodeExecution e)
        {
            AddLoopKey(e);
 
            while (e.activeMono != null)
            {
                if (e.activeMono.ActionIsComplete)
                {
                    if (e.activeMono.ActionWasSuccess == false)
                    {
                        e.EndExecutionBranch();
                    }
                    else
                    {
                        AddObjectToIterationIndex<AbicraftObject>(ref hitObjectsByIteration,e,  e.activeMono.ReturnData() as AbicraftObject);
                        AddObjectToIterationIndex<Vector3>(ref hitPointsByIteration,e, e.activeMono.transform.position);
                        AddObjectToIterationIndex<RaycastHit>(ref raycastHitIteration, e, e.activeMono.rayHits[RaycastDirection.Forward]);
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
            if (port.fieldName == "HitObject" && e != null)
                return GetObjectByIterationIndex<AbicraftObject>(ref hitObjectsByIteration, e);
            if (port.fieldName == "Normal" && e != null) {
                return GetObjectByIterationIndex<RaycastHit>(ref raycastHitIteration, e).normal;
            }
            else if(e != null)
                return GetObjectByIterationIndex<Vector3>(ref hitPointsByIteration, e);

            return default;
        }
    }
}

