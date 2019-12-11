using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using XNode;

public class AbilityDispatcher : MonoBehaviour
{
    public Ability test_Ability;

    List<AbicraftAbilityExecution> AbilityExecutionBuffer = new List<AbicraftAbilityExecution>();

    public void Dispatch(Ability Ability)
    {
        if (AbicraftAbilityExecution.OnCooldown(AbilityExecutionBuffer, Ability))
            return;

        AbicraftNode exe = getSortForExecuteNodes(Ability)[0];
        AbilityExecutionBuffer.Add
        (
            new AbicraftAbilityExecution(
                    this,    
                    Ability, 
                    exe
                )
        );
    }

    private void FixedUpdate()
    {
        RecurseNodes  ();
        TickCooldowns (Time.deltaTime);
    }

    void RecurseNodes()
    {
        for (int i = 0; i < AbilityExecutionBuffer.Count; i++)
        {
            AbicraftAbilityExecution execution = AbilityExecutionBuffer[i];

            for (int j = 0; j < execution.current_node_executions.Count; j++)
            {
                AbicraftAbilityExecution.AbicraftNodeExecution nodeExecution = execution.current_node_executions[j];

                if (nodeExecution.current_node != null)
                {
                    if (!nodeExecution.executed)
                    {
                        foreach (var item in nodeExecution.current_node.Inputs)
                        {
                            if (item.Connection != null)
                            {
                                item.Connection.node.Evaluate(nodeExecution);                   
                            }
                        }
                        nodeExecution.current_node.Initialize(nodeExecution);
                        StartCoroutine(nodeExecution.current_node.ExecuteNode(nodeExecution));
                    }

                    nodeExecution.executed = true;

                    if (!nodeExecution.globalBlock)
                    {
                        nodeExecution.executed = false;
                        List<NodePort> ports = nodeExecution.current_node.GetOutputPort("Out").GetConnections();

                        for (int k = 0; k < ports.Count; k++)
                        {
                            Debug.Log(nodeExecution.current_node);
                            NodePort port;

                            if ((port = ports[k]) != null)
                                if (nodeExecution.branchIndex == k)
                                {
                                    nodeExecution.current_node = port.node;
                                }
                                else
                                {
                                    if(k > nodeExecution.AbilityExecution.current_node_executions.Count -1 )
                                    {
                                        nodeExecution.AbilityExecution.current_node_executions.Add
                                        (
                                            new AbicraftAbilityExecution.AbicraftNodeExecution(
                                                nodeExecution.AbilityExecution,
                                                port.node
                                            )
                                        );
                                    }
                                }
                            else
                                nodeExecution.current_node = null;

                        }

                        if(ports.Count == 0)
                            nodeExecution.current_node = null;
                    }
                }
                else
                {
                    execution.current_node_executions.RemoveAt(j);
                }
            }
        }
    }

    void TickCooldowns(float delta)
    {
        for (int i = 0; i < AbilityExecutionBuffer.Count; i++)
        {
            AbicraftAbilityExecution cooldown = AbilityExecutionBuffer[i];
            cooldown.elapsed += delta;

            if (cooldown.elapsed >= cooldown.Ability.Cooldown)
            {
                if(cooldown.current_node_executions.Count == 0)
                    AbilityExecutionBuffer.RemoveAt(i);
            }      
        }
    }

    List<AbicraftNode> getSortForExecuteNodes(Ability Ability)
    {
        List<AbicraftNode> exec_nodes = new List<AbicraftNode>();

        for (int i = 0; i < Ability.nodes.Count; i++)
        {
            if (Ability.nodes[i].name == "On Execute")
                exec_nodes.Add(Ability.nodes[i]);
        }
        return new List<AbicraftNode>(exec_nodes); 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Dispatch(test_Ability);
        }
    }
}
