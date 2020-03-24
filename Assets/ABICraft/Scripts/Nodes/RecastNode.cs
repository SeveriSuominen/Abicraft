using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AbicraftCore;
using AbicraftNodes.Meta;
using AbicraftMonos;
using AbicraftMonos.Action;

namespace AbicraftNodes.Action
{
    public class RecastNode : AbicraftExecutionNode
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
                Recast recast = abj.gameObject.AddComponent<Recast>();
                //recast.keyCode = KeyCode;

                //Automatically ending actionmono as FALSE after cooldown elapsed so we dont proceed with ability execution;
                recast.StartActionMono(e.ae.Ability.Cooldown - e.ae.elapsedCooldown, false);

                while (e.IsBlocked())
                {
                    AbicraftSignal signal = GetInputValue<AbicraftSignal>(e, "ActiveSignal", null);

                    if ((recast.ActionIsComplete && recast.ActionWasSuccess) || (signal != null && signal.Active))
                    {
                        e.ReleaseBlock();
                        break;
                    }

                    if (!recast)
                    {
                        e.ae.End();
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
