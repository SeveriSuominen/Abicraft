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

		/* Example of node input field */
		[Input(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Multiple, typeConstraint = TypeConstraint.Strict)]
		public List<AbicraftObject> ObjectList; 

		/* Example node output field */
		[Output]
		public List<AbicraftObject> ObjectListOut; 

		// Return value when another node is requesting output field value
		public override object GetValue(AbicraftNodeExecution e, NodePort port) {
			//Example returning of data
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
                            merged.Add(values[i][j]);
                        }
                    }
					return merged;
				default:
					return default;
			}
		}
	}
}