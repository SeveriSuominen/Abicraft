using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AbicraftNodeEditor;
using AbicraftCore;
using AbicraftMonos;
using AbicraftNodes.Meta;

namespace AbicraftNodes.Action
{
	public class DispatchAbilityNode : AbicraftExecutionNode {

        public AbicraftAbility Ability;
        public bool IgnoreCooldown;

		// Node execution logic comes here
		public override IEnumerator ExecuteNode(AbicraftNodeExecution e)
		{
            if (Ability != null)
            {
                if(IgnoreCooldown)
                    e.ae.dispatcher.DispatchIgnoreCooldown(e.ae.senderObject, Ability);
                else
                    e.ae.dispatcher.Dispatch(e.ae.senderObject, Ability);
            }   
			yield return null;
		}
	}
}