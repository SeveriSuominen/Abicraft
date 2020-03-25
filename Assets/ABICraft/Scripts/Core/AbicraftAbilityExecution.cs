using AbicraftCore.Variables;
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
        public float elapsedCooldown, elapsedPassiveLifetime, passiveLifetime;

        public AbicraftObject senderObject;
        
        //Taking initial snapshot when execution starts, to get original start point data,
        //like player original position.
        public AbicraftGameStateSnapshot initial_snapshot;

        public AbicraftAbilityDispatcher dispatcher;

        public List<AbicraftNodeExecution> current_node_executions;

        public AbicraftNode startNode;

        public readonly string guid;

        public readonly AbicraftAbilityVariableMap variables = new AbicraftAbilityVariableMap();

        public AbicraftAbilityExecution(AbicraftAbilityDispatcher dispatcher, AbicraftAbility Ability, AbicraftObject senderObject, AbicraftNode startExecNode)
        {
            this.Ability         = Ability;
            this.senderObject    = senderObject;
            this.elapsedCooldown = 0;

            this.guid = Guid.NewGuid().ToString();

            this.startNode = startExecNode;

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

        public AbicraftAbilityExecution(AbicraftAbilityDispatcher dispatcher, AbicraftAbility Ability, AbicraftObject senderObject, AbicraftNode startExecNode, float passiveLifetime)
        {
            this.Ability = Ability;
            this.senderObject = senderObject;
            this.elapsedCooldown = 0;
            this.passiveLifetime = passiveLifetime;

            this.guid = Guid.NewGuid().ToString();

            this.startNode = startExecNode;

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

        public void End()
        {
            dispatcher.EndAbicraftAbilityExecution(this);
        }

        public void Reset()
        {
            current_node_executions.Add(
                 new AbicraftNodeExecution(
                     this,
                     startNode
                 )
             );
        }

        public AbicraftNodeExecution LastNodeExecution()
        {
            return current_node_executions[current_node_executions.Count - 1];
        }

        public bool OnCooldown()
        {
            return elapsedCooldown <= Ability.Cooldown;
        }

        public float GetCooldownLeft()
        {
            return Ability.Cooldown - elapsedCooldown;
        }

        public float GetTimeElapsedSinceExecute()
        {
            return elapsedCooldown;
        }
    }
}
