using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AbicraftNodeEditor;
using AbicraftCore;
using AbicraftMonos;
using AbicraftNodes.Meta;

namespace AbicraftNodes.Object
{
	public class MergeObjectListsNode : AbicraftValueNode {

        [Input(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Multiple, typeConstraint = TypeConstraint.Strict)]
		public List<AbicraftObject> ObjectList;

		[Output]
		public List<AbicraftObject> ObjectListOut;

        [HideInInspector]
        public bool CreateUniqueList;

        public override object GetValue(AbicraftNodeExecution e, NodePort port) {
			switch(port.fieldName)
			{
				case "ObjectListOut":
                    var values = GetInputValues<List<AbicraftObject>>(e, "ObjectList");
                    var merged = new List<AbicraftObject>();

                    for (int i = 0; i < values.Length; i++)
                    {
                        if (values[i] == null)
                            continue;

                        for (int j = 0; j < values[i].Count; j++)
                        {
                            if (CreateUniqueList)
                            {
                                if(!merged.Contains(values[i][j]))
                                    merged.Add(values[i][j]);
                            }
                            else
                            {
                                merged.Add(values[i][j]);
                            }  
                        }
                    }
					return merged;
				default:
					return default;
			}
		}
	}
}