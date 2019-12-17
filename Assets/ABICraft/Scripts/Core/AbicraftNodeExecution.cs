using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbicraftNodeExecution
{
    public AbicraftNode current_node;
    public AbicraftAbilityExecution AbilityExecution;

    public AbicraftActionMono activeMono;

    public int branchIndex;
    public int iterationIndex;

    public bool finished;

    public bool executed;
    public bool localBlock;

    public void EndExecutionBranch()
    {
        AbilityExecution.current_node_executions[branchIndex].current_node = null;
        this.finished = true;
    }

    public void Block()
    {
        this.localBlock = true;
    }

    public void ReleaseBlock()
    {
        this.localBlock = false;
    }

    public bool IsBlocked()
    {
        return this.localBlock;
    }

    public void SetBranchIndex()
    {
        AbilityExecution.current_node_executions[AbilityExecution.current_node_executions.Count - 1].branchIndex = AbilityExecution.current_node_executions.Count - 1;
    }

    public AbicraftNodeExecution(AbicraftAbilityExecution execution, AbicraftNode current_node, int iterationIndex = 0)
    {
        this.current_node = current_node;
        this.AbilityExecution = execution;
        this.iterationIndex = iterationIndex;

        executed = false;
        localBlock = false;
    }
}
