using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbicraftAbilityExecution
{
    public Ability Ability;
    public float elapsed;

    //Taking initial snapshot when execution starts, to get original start point data,
    //like player original position.
    public AbiCraftStateSnapshot initial_snapshot;

    public AbilityDispatcher dispatcher;

    public List<AbicraftNodeExecution> current_node_executions;

    public class AbicraftNodeExecution
    {
        public AbicraftNode current_node;
        public AbicraftAbilityExecution AbilityExecution;

        public int  branchIndex = 0;

        public bool finished;

        public bool executed;
        public bool globalBlock;

        public void Block()
        {
            this.globalBlock = true;
        }

        public void ReleaseBlock()
        {
            this.globalBlock = false;
        }

        public bool IsBlocked()
        {
            return this.globalBlock;
        }

        public AbicraftNodeExecution(AbicraftAbilityExecution execution, AbicraftNode current_node)
        {
            this.current_node = current_node;
            this.AbilityExecution = execution;

            executed    = false;
            globalBlock = false;
        }
    }

    public AbicraftAbilityExecution(AbilityDispatcher dispatcher, Ability Ability, AbicraftNode startExecNode)
    {
        this.Ability = Ability;
        this.elapsed = 0;

        current_node_executions = new List<AbicraftNodeExecution>();
        current_node_executions.Add(
            new AbicraftNodeExecution(
                this,
                startExecNode
            )
        );

        current_node_executions[current_node_executions.Count -1].branchIndex = current_node_executions.Count - 1;
        this.dispatcher = dispatcher;

        initial_snapshot = AbiCraftStateSnapshot.TakeSnapshot;
    }

    public static bool OnCooldown(List<AbicraftAbilityExecution> cds, Ability Ability)
    {
        for (int i = 0; i < cds.Count; i++)
        {
            if (cds[i].Ability.Equals(Ability))
                return true;
        }
        return false;
    }
}
