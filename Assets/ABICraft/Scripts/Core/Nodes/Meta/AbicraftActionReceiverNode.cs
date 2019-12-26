
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;

using AbicraftCore;

namespace AbicraftNodes.Meta
{
    public abstract class AbicraftActionReceiverNode : AbicraftNode
    {
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict, backingValue = ShowBackingValue.Never)]
        public AbicraftActionLine In;
        [Output]
        public AbicraftLifeline Out;

        protected void AddObjectToIterationIndex<T>(ref Dictionary<string, T> map, AbicraftNodeExecution e, T obj)
        {


            if (e.loopKeys.Count > 0)
            {
                string loopKey = e.loopKeys[e.loopKeys.Count - 1];

                if (loopKey == null)
                {
                    Debug.LogError("Abicraft: Loop key is null!");
                    return;
                }
                map[loopKey + e.GetIterationIndex(loopKeys[e.ae.guid])] = obj;
            }
            else
            {
                map["default_no_loop"] = obj;
            }
        }

        protected T GetObjectByIterationIndex<T>(ref Dictionary<string, T> map, AbicraftNodeExecution e)
        {

            if (e.loopKeys.Count > 0)
            {
                int iterationIndex = e.GetIterationIndex(loopKeys[e.ae.guid]);

                foreach (var item in loopKeys[e.ae.guid])
                {
                    if (map.ContainsKey(item + iterationIndex))
                    {
                        //Debug.Log(item + "_" + iterationIndex);
                        return map[item + iterationIndex];
                    }

                }
                return default(T);
            }
            else
            {
                if (map.ContainsKey("default_no_loop"))
                    return map["default_no_loop"];
                else
                    return default(T);
            }
        }

        public override object GetValue(AbicraftNodeExecution e, NodePort port)
        {
            return GetInputValue<AbicraftLifeline>(e, "In");
        }
    }
}

