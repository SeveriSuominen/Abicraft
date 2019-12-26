using AbicraftMonos;
using AbicraftNodes.Meta;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AbicraftCore
{
    public class AbicraftAbilityExecution
    {
        public AbicraftAbility Ability;
        public float elapsed;

        public AbicraftObject senderObject;

        //Taking initial snapshot when execution starts, to get original start point data,
        //like player original position.
        public AbicraftGameStateSnapshot initial_snapshot;

        public AbicraftAbilityDispatcher dispatcher;

        public List<AbicraftNodeExecution> current_node_executions;

        public readonly string guid;

        public AbicraftAbilityExecution(AbicraftAbilityDispatcher dispatcher, AbicraftAbility Ability, AbicraftObject senderObject, AbicraftNode startExecNode)
        {
            this.Ability = Ability;
            this.senderObject = senderObject;
            this.elapsed = 0;

            guid = Guid.NewGuid().ToString();

            current_node_executions = new List<AbicraftNodeExecution>();
            current_node_executions.Add(
                new AbicraftNodeExecution(
                    this,
                    startExecNode
                )
            );

            current_node_executions[current_node_executions.Count - 1].SetBranchIndex();
            this.dispatcher = dispatcher;

            initial_snapshot = AbicraftGameStateSnapshot.TakeSnapshot;
        }

        public AbicraftNodeExecution LastNodeExecution()
        {
            return current_node_executions[current_node_executions.Count - 1];
        }

        public bool OnCooldown()
        {
            return elapsed >= Ability.Cooldown;
        }
    }

}
