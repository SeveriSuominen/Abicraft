using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AbicraftNodeEditor;
using AbicraftCore;
using AbicraftMonos;
using AbicraftNodes.Meta;

namespace AbicraftNodes.Math
{
	public class TowardsNode : AbicraftValueNode {

        public Mode originMode;
        public Mode towardsMode;

		
		[Input(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
		public AbicraftObject Origin;
        [Input(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AbicraftObject Towards;

  
        [Input(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public Vector3 OriginPosition;
        [Input(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public Vector3 TowardsPosition;
        
        [Output]
		public Vector3 Direction;

        [Output]
        public float Distance;

        [System.Serializable]
        public enum Mode
        {
            Cursor,
            Object,
            Position
        }

		// Return value when another node is requesting output field value
		public override object GetValue(AbicraftNodeExecution e, NodePort port) {
            AbicraftGameStateSnapshot snapshot = AbicraftGameStateSnapshot.TakeSnapshot;

            Vector3 direction;
            float   distance;

            Vector3 originPos = Vector3.zero, towardsPos = Vector3.zero;
           
            switch (originMode)
            {
                case TowardsNode.Mode.Cursor:
                    originPos = snapshot.mousePosition3D;
                    break;

                case TowardsNode.Mode.Object:
                    AbicraftObject origin = GetInputValue<AbicraftObject>(e, "Origin", null);
                    if (origin)
                        originPos = origin.transform.position;
                    break;

                case TowardsNode.Mode.Position:
                    originPos = GetInputValue<Vector3>(e, "OriginPosition", Vector3.zero);
                    break;
            }

            switch (towardsMode)
            {
                case TowardsNode.Mode.Cursor:
                    towardsPos = snapshot.mousePosition3D;
                    break;
                case TowardsNode.Mode.Object:
                    AbicraftObject towards = GetInputValue<AbicraftObject>(e, "Towards", null);
                    if (towards)
                        towardsPos = towards.transform.position;
                    break;
                case TowardsNode.Mode.Position:
                    towardsPos = GetInputValue<Vector3>(e, "TowardsPosition", Vector3.zero);
                    break;
            }

            distance  = Vector3.Distance(originPos, towardsPos);
            direction = (towardsPos - originPos).normalized;

            if (port.fieldName.Equals("Distance"))
                return distance;
            else
                return direction;
		}
	}
}