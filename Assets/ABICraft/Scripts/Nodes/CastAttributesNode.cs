using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

using AbicraftNodeEditor;
using AbicraftCore;
using AbicraftMonos;
using AbicraftNodes.Meta;

namespace AbicraftNodes.Action
{
    public class CastAttributesNode : AbicraftExecutionNode
    {    
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AbicraftObject TargetObj;

        [HideInInspector]
        public int    selectedIndex, lastIndex;

        [HideInInspector]
        public List<AttributeCast> allSelectedIndices = new List<AttributeCast>();
        [HideInInspector]
        public List<int> allSelectedIndiceAmounts = new List<int>();

        [System.Serializable]
        public enum CastSourceMode
        {
            FromSenderObject,
            Manual
        }

        [System.Serializable]
        public class AttributeCast
        {
            public AttributeCast(AbicraftAttribute attribute)
            {
                this.attribute = attribute;
            }

            public AbicraftAttribute attribute;
            public int amount;
            public CastSourceMode mode;
        }

        public override IEnumerator ExecuteNode(AbicraftNodeExecution e)
        {
            AbicraftObject abj = GetInputValue<AbicraftObject>(e, "TargetObj", TargetObj);

            if (abj)
            {
                for (int i = 0; i < allSelectedIndices.Count; i++)
                {
                    AttributeCast cast = allSelectedIndices[i];

                    switch (cast.mode)
                    {
                        case CastSourceMode.FromSenderObject:
                            abj.CastAttributeOn(e.ae.senderObject.GetAttributeAmount(e.ae.senderObject, cast.attribute), e.ae.senderObject, cast.attribute);
                            break;

                        case CastSourceMode.Manual:
                            abj.CastAttributeOn(cast.amount, e.ae.senderObject, cast.attribute);
                            break;
                    }                      
                }
            }
            yield return null;
        }
    }
}


