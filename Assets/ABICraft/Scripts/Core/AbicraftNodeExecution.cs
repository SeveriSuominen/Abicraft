using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbicraftNodeExecution
{
    public AbicraftNode current_node;
    public AbicraftAbilityExecution AbilityExecution;

    public int branchIndex;
    public int iterationIndex;

    public bool finished;

    public bool executed;
    public bool globalBlock;

    public void EndExecutionBranch()
    {
        AbilityExecution.current_node_executions[branchIndex].current_node = null;
        this.finished = true;
    }

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

        executed = false;
        globalBlock = false;
    }

    public void SetBranchIndex()
    {
        AbilityExecution.current_node_executions[AbilityExecution.current_node_executions.Count - 1].branchIndex = AbilityExecution.current_node_executions.Count - 1;
    }
}
