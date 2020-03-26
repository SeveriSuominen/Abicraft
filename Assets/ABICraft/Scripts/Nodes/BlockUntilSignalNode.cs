using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AbicraftCore;
using AbicraftNodes.Meta;
using AbicraftMonos;
using AbicraftMonos.Action;

namespace AbicraftNodes.Action
{
    public class BlockUntilSignalNode : AbicraftExecutionNode
    {
        //public KeyCode KeyCode;
        //public string Key;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AbicraftSignal ActiveSignal;

        public override void Initialize(AbicraftNodeExecution e)
        {
            e.Block();
        }

        public override IEnumerator ExecuteNode(AbicraftNodeExecution e)
        {
            AbicraftObject abj = e.ae.senderObject;

            if(abj != null)
            {
                while (e.IsBlocked())
                {
                    AbicraftSignal signal = GetInputValue<AbicraftSignal>(e, "ActiveSignal", null);

                    if ((signal != null && signal.Active))
                    {
                        e.ReleaseBlock();
                        break;
                    }

                    if (signal == null || !e.ae.OnCooldown())
                    {
                        e.EndExecutionBranch();
                        break;
                    }
                    yield return new WaitForFixedUpdate();
                }  
                yield return null;
            }
            else
            {
                yield return null;
            }
        }
    }
}
