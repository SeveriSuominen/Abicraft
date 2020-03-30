using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AbicraftNodeEditor;
using AbicraftCore;
using AbicraftMonos;
using AbicraftNodes.Meta;

namespace AbicraftNodes.Custom.Action
{
	public class NewAbicraftExecutionNode : AbicraftExecutionNode {

		/* Example of node input field */
		[Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
		public AbicraftObject EXAMPLE_INPUT_FIELD;
        
		/* Example node output field */
		[Output]
		public AbicraftObject EXAMPLE_OUTPUT_FIELD; 

		// Node execution logic comes here
		public override IEnumerator ExecuteNode(AbicraftNodeExecution e)
		{
			 yield return null;
		}

		// Return value when another node is requesting output field value
		public override object GetValue(AbicraftNodeExecution e, NodePort port) {

			//Example returning of data
			switch(port.fieldName)
			{
				case "EXAMPLE_OUTPUT_FIELD":
					return GetInputValue<AbicraftObject>(e, "EXAMPLE_INPUT_FIELD", null);
				default:
					return GetInputValue<AbicraftLifeline>(e, "In");
			}
		}
	}
}