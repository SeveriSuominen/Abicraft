using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace AbicraftNodes.Object
{
    public class AbicraftObjectFindNode : AbicraftValueNode
    {
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public string Name;

        [Output]
        public AbicraftObject Obj;

        public override object GetValue(NodePort port)
        {
            GameObject obj;

            if ((obj = GameObject.Find(Name)) != null)
                return obj.GetComponent<AbicraftObject>();

            return null;
        }
    }
}