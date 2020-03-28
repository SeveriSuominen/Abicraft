
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;

using AbicraftCore;
using System;

namespace AbicraftNodes.Meta
{
    public abstract class AbicraftExecutionNode : AbicraftNode
    {
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict, backingValue = ShowBackingValue.Never)]
        public AbicraftLifeline In;
        [Output]
        public AbicraftLifeline Out;

        Dictionary<string, object> iterationIndexValues = new Dictionary<string, object>();

        protected void AddObjectToIterationIndex<T>(AbicraftNodeExecution e, string fieldName, T obj)
        {
            if (e == null)
                return;

            if (e.loopKeys.Count > 0)
            {
                string loopKey = e.loopKeys[e.loopKeys.Count - 1];

                if (loopKey == null)
                {
                    Debug.LogError("Abicraft: Loop key is null!");
                    return;
                }
                iterationIndexValues[loopKey + e.GetIterationIndex(loopKeys[e.ae.guid]) + fieldName] = obj;
            }
            else
            {
                iterationIndexValues["default_no_loop"] = obj;
            }
        }

        protected void AddObjectToIterationIndex<T>(ref Dictionary<string, T> map, AbicraftNodeExecution e, T obj)
        {
            if (e == null)
                return;

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

        protected object GetObjectByIterationIndex<T>(AbicraftNodeExecution e, string fieldName)
        {
            if (e == null)
                return default(T);

            if (e.loopKeys.Count > 0)
            {
                int iterationIndex = e.GetIterationIndex(loopKeys[e.ae.guid]);

                foreach (var item in loopKeys[e.ae.guid])
                {
                    if (iterationIndexValues.ContainsKey(item + iterationIndex + fieldName))
                    {
                        return iterationIndexValues[item + iterationIndex + fieldName];
                    }

                }
                return default(T);
            }
            else
            {
                if (iterationIndexValues.ContainsKey("default_no_loop"))
                    return iterationIndexValues["default_no_loop"];
                else
                    return default(T);
            }
        }

        protected T GetObjectByIterationIndex<T>(ref Dictionary<string, T> map, AbicraftNodeExecution e)
        {
            if (e == null)
                return default(T);

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


