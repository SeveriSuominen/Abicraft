using AbicraftMonos.Action;
using AbicraftNodes.Meta;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbicraftCore
{
    public class AbicraftNodeExecution
    {

        public AbicraftNode current_node;
        public AbicraftAbilityExecution ae;

        public AbicraftActionMono activeMono;

        public readonly List<string> loopKeys = new List<string>();

        public int branchIndex;

        //public int iterationIndex;
        public readonly Dictionary<string, int> iterationIndices = new Dictionary<string, int>();

        public bool finished;

        public bool executed;
        public bool localBlock;

        public void EndExecutionBranch()
        {
            ae.current_node_executions[branchIndex].current_node = null;
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
            ae.current_node_executions[ae.current_node_executions.Count - 1].branchIndex = ae.current_node_executions.Count - 1;
        }

        public string GetLoopKey(int index)
        {
            return loopKeys[index];
        }

        public int GetIterationIndex(List<string> loopKeys)
        {
            foreach (var item in loopKeys)
            {
                if (iterationIndices.ContainsKey(item))
                    return iterationIndices[item];
            }
            return -1;
        }

        public AbicraftNodeExecution(AbicraftAbilityExecution execution, AbicraftNode current_node)
        {
            this.current_node = current_node;
            this.ae = execution;

            executed = false;
            localBlock = false;
        }

        public AbicraftNodeExecution(AbicraftAbilityExecution execution, AbicraftNode current_node, Dictionary<string, int> iteratIndices, int iterationIndex, string addLoopKey)
        {
            this.current_node = current_node;
            this.ae = execution;

            this.loopKeys.Clear();

            foreach (KeyValuePair<string, int> item in iteratIndices)
            {
                this.iterationIndices.Add(item.Key, item.Value);
                this.loopKeys.Add(item.Key);
            }

            if (addLoopKey != null)
            {
                if (!this.loopKeys.Contains(addLoopKey))
                    loopKeys.Add(addLoopKey);

                iterationIndices.Add(addLoopKey, iterationIndex);
            }

            executed = false;
            localBlock = false;
        }

        public AbicraftNodeExecution(AbicraftAbilityExecution execution, AbicraftNode current_node, Dictionary<string, int> iteratIndices)
        {
            this.current_node = current_node;
            this.ae = execution;

            this.loopKeys.Clear();

            foreach (KeyValuePair<string, int> item in iteratIndices)
            {
                this.iterationIndices.Add(item.Key, item.Value);
                this.loopKeys.Add(item.Key);
            }

            executed = false;
            localBlock = false;
        }
    }
}


