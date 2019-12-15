using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbicraftAbilityExecution
{
    public AbicraftAbility Ability;
    public float elapsed;

    //Taking initial snapshot when execution starts, to get original start point data,
    //like player original position.
    public AbiCraftStateSnapshot initial_snapshot;

    public AbilityDispatcher dispatcher;

    public List<AbicraftNodeExecution> current_node_executions;

    public AbicraftAbilityExecution(AbilityDispatcher dispatcher, AbicraftAbility Ability, AbicraftNode startExecNode)
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

        current_node_executions[current_node_executions.Count - 1].SetBranchIndex();
        this.dispatcher = dispatcher;

        initial_snapshot = AbiCraftStateSnapshot.TakeSnapshot;
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
