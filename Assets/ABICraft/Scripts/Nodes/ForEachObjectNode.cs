using AbicraftCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

using AbicraftNodeEditor;

namespace AbicraftNodes.Action
{
    public class ForEachObjectNode : AbicraftExecutionLoopNode
    {
        [Output]
        public AbicraftObject Obj;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public  List<AbicraftObject> Objs;
        private List<AbicraftObject> objs;

        public override void Initialize(AbicraftNodeExecution e)
        {
            objs = GetInputValue<List<AbicraftObject>>(e, "Objs", Objs);
        }

        public override int IterationCount(AbicraftNodeExecution e)
        {
            return objs.Count;
        }

        public override object GetValue(AbicraftNodeExecution e, NodePort port)
        {
            object @base = base.GetValue(e, port);

            if (objs == null)
            {
                objs = GetInputValue<List<AbicraftObject>>(e, "Objs", Objs);
            }

            if (port.fieldName.Equals("Obj"))
            {
                if (e != null)
                {
                    int indexOfObj = -1;

                    foreach (var item in loopKeys)
                    {
                        string key = port.fieldName + item + e.GetIterationIndex(loopKeys[e.ae.guid]);

                        if (iterations.ContainsKey(key))
                        {
                            indexOfObj = (int)iterations[key];
                            break;
                        }
                        else
                        {
                            iterations.Add(key, e.GetIterationIndex(loopKeys[e.ae.guid]));
                            indexOfObj = (int)iterations[key];
                            break;
                        }
                    }
                    return objs[indexOfObj -1];
                }
            }
            return @base;
        }
    }
}


