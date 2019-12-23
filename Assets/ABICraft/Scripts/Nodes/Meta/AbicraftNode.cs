using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;
using AbicraftCore;
using AbicraftMonos;

namespace AbicraftNodes.Meta
{
    public abstract class AbicraftNode : Node
    {
        protected Dictionary<string, List<string>> loopKeys = new Dictionary<string, List<string>>();

        [HideInInspector]
        public Abicraft abicraft;

        public void AddLoopKey(AbicraftNodeExecution e)
        {
            if (e.loopKeys.Count != 0)
            {
                if (!loopKeys.ContainsKey(e.ae.guid) || loopKeys[e.ae.guid] == null)
                {
                    loopKeys.Add(e.ae.guid, new List<string>());
                }

                string key = e.loopKeys[e.loopKeys.Count - 1];
                if (!loopKeys[e.ae.guid].Contains(key))
                    loopKeys[e.ae.guid].Add(key);
            }
        }

        public void AddLoopKey(string ae_guid, string key)
        {
            if (!loopKeys.ContainsKey(ae_guid) || loopKeys[ae_guid] == null)
            {
                loopKeys.Add(ae_guid, new List<string>());
            }

            if (!loopKeys[ae_guid].Contains(key))
                loopKeys[ae_guid].Add(key);
        }


        public virtual void Initialize(AbicraftNodeExecution execution)
        {
            return;
        }

        public virtual void Evaluate(AbicraftNodeExecution execution)
        {
            return;
        }


        public void CleanAbilityExecutionCache(AbicraftAbilityExecution ae)
        {
            if (loopKeys.ContainsKey(ae.guid))
                loopKeys.Remove(ae.guid);
        }

        public virtual IEnumerator ExecuteNode(AbicraftNodeExecution e)
        {
            yield return null;
        }
    }
}


