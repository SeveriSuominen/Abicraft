using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using AbicraftNodeEditor;
using System;
using AbicraftNodes.Meta;

namespace AbicraftCore
{
    public class AbilityDispatcher : MonoBehaviour
    {
        public AbicraftAbility test_Ability, test_Ability2, test_Ability3;

        List<AbicraftAbilityExecution> AbilityExecutionBuffer = new List<AbicraftAbilityExecution>();

        public void Dispatch(AbicraftAbility ability)
        {
            if (AbilityOnCooldown(ability))
                return;

            AbicraftNode exe = getSortForExecuteNodes(ability)[0];
            AbilityExecutionBuffer.Add
            (
                new AbicraftAbilityExecution(
                        this,
                        ability,
                        exe
                    )
            );
        }

        public bool AbilityOnCooldown(AbicraftAbility ability)
        {
            for (int i = 0; i < AbilityExecutionBuffer.Count; i++)
            {
                if (AbilityExecutionBuffer[i].Ability == ability)
                {
                    if (AbilityExecutionBuffer[i].elapsed < AbilityExecutionBuffer[i].Ability.Cooldown)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool AnyAbilityIsExecuting()
        {
            return AbilityExecutionBuffer.Count > 0;
        }

        public bool AbilityIsExecuting(AbicraftAbility ability)
        {
            for (int i = 0; i < AbilityExecutionBuffer.Count; i++)
            {
                if (AbilityExecutionBuffer[i].Ability == ability)
                    return true;
            }
            return false;
        }

        private void FixedUpdate()
        {
            RecurseNodes();
            TickCooldowns(Time.deltaTime);
        }

        void RecurseNodes()
        {
            for (int i = 0; i < AbilityExecutionBuffer.Count; i++)
            {
                AbicraftAbilityExecution execution = AbilityExecutionBuffer[i];

                for (int j = 0; j < execution.current_node_executions.Count; j++)
                {
                    AbicraftNodeExecution nodeExecution = execution.current_node_executions[j];

                    if (!nodeExecution.finished && nodeExecution.current_node != null)
                    {
                        if (nodeExecution.current_node != null && nodeExecution.current_node.GetType().IsSubclassOf(typeof(AbicraftExecutionLoopNode)))
                        {
                            ExecuteLoop(nodeExecution);
                        }
                        else
                        {
                            ExecuteNextNode(nodeExecution);
                        }
                    }
                    else
                    {
                        nodeExecution.current_node = null;
                    }
                }
            }
        }

        void ExecuteNextNode(AbicraftNodeExecution nodeExecution)
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

            if (!nodeExecution.localBlock)
            {
                nodeExecution.executed = false;
                BranchExecute(nodeExecution, nodeExecution.current_node.GetOutputPort("Out").GetConnections());
            }
        }

        void ExecuteLoop(AbicraftNodeExecution nodeExecution)
        {
            AbicraftExecutionLoopNode loopnode = nodeExecution.current_node as AbicraftExecutionLoopNode;

            List<NodePort> loopPorts = nodeExecution.current_node.GetOutputPort("Loop").GetConnections();
            List<NodePort> portsContinue = nodeExecution.current_node.GetOutputPort("Continue").GetConnections();

            loopnode.Initialize(nodeExecution);

            loopnode.iterations.Clear();

            int iterationCount = loopnode.IterationCount(nodeExecution);

            string loopKey = Guid.NewGuid().ToString();
            loopnode.AddLoopKey(nodeExecution.ae.guid, loopKey);

            if (loopnode.Parallel)
            {
                for (int i = 0; i < iterationCount; i++)
                {
                    for (int k = 0; k < loopPorts.Count; k++)
                    {
                        NodePort port;

                        if ((port = loopPorts[k]) != null)
                        {
                            nodeExecution.ae.current_node_executions.Add
                            (
                                new AbicraftNodeExecution(
                                    nodeExecution.ae,
                                    port.node,
                                    nodeExecution.iterationIndices, //Inherite all iterationIndices
                                    i + 1,  // IterarionIndex,
                                    loopKey // Loop key
                                )
                            );
                        }
                        else
                            nodeExecution.current_node = null;
                    }
                }
            }
            BranchExecute(nodeExecution, portsContinue);
        }

        void BranchExecute(AbicraftNodeExecution nodeExecution, List<NodePort> ports)
        {
            AbicraftNode zeroBranchNextNode = null;

            for (int k = 0; k < ports.Count; k++)
            {
                NodePort port = ports[k];

                if (port != null)
                {
                    if (nodeExecution.branchIndex == k)
                    {
                        zeroBranchNextNode = port.node;
                    }
                    else
                    {
                        nodeExecution.ae.current_node_executions.Add
                        (
                            new AbicraftNodeExecution(
                                nodeExecution.ae,
                                port.node,
                                nodeExecution.iterationIndices //Inherite all iterationIndices
                            )
                        );
                    }
                }
            }
            nodeExecution.current_node = zeroBranchNextNode;

            if (ports.Count == 0)
                nodeExecution.current_node = null;
        }

        void TickCooldowns(float delta)
        {
            for (int i = 0; i < AbilityExecutionBuffer.Count; i++)
            {
                AbicraftAbilityExecution cooldown = AbilityExecutionBuffer[i];
                cooldown.elapsed += delta;

                if (cooldown.elapsed >= cooldown.Ability.Cooldown)
                {
                    bool allLifelineBranchesEnded = true;

                    for (int j = 0; j < cooldown.current_node_executions.Count; j++)
                    {
                        if (cooldown.current_node_executions[j].current_node != null)
                            allLifelineBranchesEnded = false;
                    }

                    if (allLifelineBranchesEnded)
                    {
                        //CLEAR LOOPING CACHES
                        for (int j = 0; j < AbilityExecutionBuffer[i].Ability.nodes.Count; j++)
                        {
                            AbicraftNode node = AbilityExecutionBuffer[i].Ability.nodes[j];
                            if (node != null)
                                node.CleanAbilityExecutionCache(AbilityExecutionBuffer[i]);
                        }
                        AbilityExecutionBuffer.RemoveAt(i);
                    }
                }
            }
        }

        List<AbicraftNode> getSortForExecuteNodes(AbicraftAbility Ability)
        {
            List<AbicraftNode> exec_nodes = new List<AbicraftNode>();

            for (int i = 0; i < Ability.nodes.Count; i++)
            {
                if (Ability.nodes[i] != null && Ability.nodes[i].name == "On Execute")
                    exec_nodes.Add(Ability.nodes[i]);
            }
            return new List<AbicraftNode>(exec_nodes);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Dispatch(test_Ability);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Dispatch(test_Ability2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Dispatch(test_Ability3);
            }
        }
    }
}


