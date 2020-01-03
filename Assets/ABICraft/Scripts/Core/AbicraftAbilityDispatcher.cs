using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using AbicraftNodeEditor;
using System;
using AbicraftNodes.Meta;
using AbicraftMonos;

namespace AbicraftCore
{
    /// <summary> Abicraft component to translate and execute AbicraftAbility 's</summary>
    public class AbicraftAbilityDispatcher : MonoBehaviour
    {
        /// <summary> Is AbicraftAbilityExecution buffer updated in FixedUpdate() instead of Update() </summary>
        public bool UpdateAbilityExecutionsInFixedUpdate;

        /// <summary> AbicraftAbilityExecution buffer to hold all active executions that are updated per frame </summary>
        List<AbicraftAbilityExecution> AbilityExecutionBuffer = new List<AbicraftAbilityExecution>();

        public void Start()
        {
            if(AbicraftGlobalContext.abicraft)
                AbicraftGlobalContext.abicraft.dispatcher = this;
        }

        /// <summary> Executes AbicraftAbility </summary>
        public void Dispatch(AbicraftObject senderObject, AbicraftAbility ability, float passiveLifetime = -1)
        {
            if (!ability)
            {
                Debug.LogError("Abicraft: Trying to dispatch ability that is null");
                return;
            }

            if (!senderObject)
            {
                Debug.LogError("Abicraft: Cannot dispatch ability with null senderObject");
                return;
            }

            if (AbilityOnCooldown(senderObject, ability))
                return;

            float usePassiveLifetime = passiveLifetime == -1 ? ability.DefaultLifetime : passiveLifetime;

            AbicraftNode exe = getSortForExecuteNodes(ability)[0];
            AbilityExecutionBuffer.Add
            (
                new AbicraftAbilityExecution(
                        this,
                        ability,
                        senderObject,
                        exe,
                        usePassiveLifetime
                    )
            );
        }

        public List<AbicraftAbilityExecution> GetActiveExecutionsBySenderObject(AbicraftAbility ability, AbicraftObject senderObject)
        {
            List<AbicraftAbilityExecution> executions = new List<AbicraftAbilityExecution>();

            for (int i = 0; i < AbilityExecutionBuffer.Count; i++)
            {
                AbicraftAbilityExecution execution = AbilityExecutionBuffer[i];

                if (execution.Ability.Equals(ability) && execution.senderObject.Equals(senderObject))
                {
                    executions.Add(execution);
                }
            }
            return executions;
        }

        public void EndAbicraftAbilityExecution(AbicraftAbilityExecution ae)
        {
            if (ae.Ability.Passive)
            {
                //Force finish passive ability 
                ae.passiveLifetime = 0;
            }
            else
            {
                for (int i = 0; i < ae.current_node_executions.Count; i++)
                {
                    //End all AbilityExecution's branches
                    ae.current_node_executions[i].EndExecutionBranch();
                }
            }
        }

        /// <summary>Check if AbicraftAbility is on cooldown </summary>
        public bool AbilityOnCooldown(AbicraftObject senderObject, AbicraftAbility ability)
        {
            for (int i = 0; i < AbilityExecutionBuffer.Count; i++)
            {
                if (AbilityExecutionBuffer[i].Ability == ability && AbilityExecutionBuffer[i].senderObject == senderObject)
                {
                    if (AbilityExecutionBuffer[i].elapsedCooldown < AbilityExecutionBuffer[i].Ability.Cooldown)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>Check if any AbicraftAbility is executing </summary>
        public bool AnyAbilityIsExecuting()
        {
            return AbilityExecutionBuffer.Count > 0;
        }

        /// <summary>Check if AbicraftAbility is executing </summary>
        public bool AbilityIsExecuting(AbicraftAbility ability)
        {
            for (int i = 0; i < AbilityExecutionBuffer.Count; i++)
            {
                if (AbilityExecutionBuffer[i].Ability == ability)
                    return true;
            }
            return false;
        }

        /// <summary> Does all AbicraftAbilityExecution checks and per frame updates </summary>
        void UpdateAbilityExecutions()
        {
            if (AbicraftGlobalContext.abicraft && !AbicraftGlobalContext.abicraft.dispatcher)
                AbicraftGlobalContext.abicraft.dispatcher = this;

            // Per execution routine
            for (int i = 0; i < AbilityExecutionBuffer.Count; i++)
            {
                AbicraftAbilityExecution ae = AbilityExecutionBuffer[i];

                RecurseAbilityExecutionNodeTree(ae);
                TickCooldown(ae, Time.deltaTime /*delta*/ );
                TickPassiveLifetime(ae, Time.deltaTime);
                CleanDoneAbilityExecution(ae);
            }
            // ------------
        }

        /// <summary> Call UpdateAbilityExecutions() every frame in FixedUpdate(), if UpdateAbilityExecutionsInFixedUpdate == true </summary>
        private void FixedUpdate()
        {
            if (UpdateAbilityExecutionsInFixedUpdate)
                UpdateAbilityExecutions();
        }

        /// <summary> Iterate over all AbicraftAbilityExecution in current buffer, and sort executions to right handlers or end execution if execution dont continue </summary>
        void RecurseAbilityExecutionNodeTree(AbicraftAbilityExecution execution)
        {
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

        /// <summary> Executes next node in AbicraftAbilityExecution. And set it as execution's current node, if node execution in not currently blocked </summary>
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

        /// <summary> Executes next node in AbicraftAbilityExecution. IF node is type of loop node, works as ExecuteNextNode() method, but handles branching also throught iterating and generate loopkey GUID for loop execution and add it to node. </summary>
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

        /// <summary> Finalizing method called after moved to next node, to branch node output type of AbicraftLifeLine to seperate executions to allow branching </summary>
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

        /// <summary> Tick AbicraftAbilityExecution between frames delta </summary>
        void TickCooldown(AbicraftAbilityExecution execution, float delta)
        {
            execution.elapsedCooldown += delta;
        }

        /// <summary> Tick AbicraftAbilityExecution between frames delta </summary>
        void TickPassiveLifetime(AbicraftAbilityExecution execution, float delta)
        {
            execution.elapsedPassiveLifetime += delta;
        }

        /// <summary> Clean and clears AbicraftAbilityExecution, when its done, meaning Ability's cooldown is elapsed </summary>
        void CleanDoneAbilityExecution(AbicraftAbilityExecution execution)
        {
            if (execution.elapsedCooldown >= execution.Ability.Cooldown)
            {
                bool allLifelineBranchesEnded = true;

                for (int j = 0; j < execution.current_node_executions.Count; j++)
                {
                    if (execution.current_node_executions[j].current_node != null)
                        allLifelineBranchesEnded = false;
                }

                if (allLifelineBranchesEnded)
                {
                    if (execution.Ability.Passive && execution.passiveLifetime >= execution.elapsedPassiveLifetime)
                    {
                        execution.Reset();
                        execution.elapsedCooldown = 0;

                        return;
                    }

                    //CLEAR LOOPING CACHES
                    for (int j = 0; j < execution.Ability.nodes.Count; j++)
                    {
                        AbicraftNode node = execution.Ability.nodes[j];
                        if (node != null)
                            node.CleanAbilityExecutionLoopCache(execution);
                    }
                    AbilityExecutionBuffer.Remove(execution);
                }
            }
        }

        /// <summary>Gets AbicraftAbility's execution node as startpoint for AbicraftNodeExecution </summary>
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

        /// <summary> Call UpdateAbilityExecutions() every frame in Update(), if UpdateAbilityExecutionsInFixedUpdate == false </summary>
        void Update()
        {
            if (!UpdateAbilityExecutionsInFixedUpdate)
                UpdateAbilityExecutions();
        }
    }
}


