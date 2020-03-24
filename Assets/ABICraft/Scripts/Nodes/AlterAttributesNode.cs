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
    public class AlterAttributesNode : AbicraftExecutionNode
    {    
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AbicraftObject Obj;

        [HideInInspector]
        public int    selectedIndex, lastIndex;

        [HideInInspector]
        public List<AttributeAlter> allSelectedIndices = new List<AttributeAlter>();
        [HideInInspector]
        public List<int> allSelectedIndiceAmounts = new List<int>();

        [System.Serializable]
        public enum AlterMode
        {
            Change,
            ChangeForSeconds,
            Set,
            SetForSeconds
        }

        [System.Serializable]
        public class AttributeAlter
        {
            public AttributeAlter(AbicraftAttribute attribute)
            {
                this.attribute = attribute;
            }

            public AbicraftAttribute attribute;
            public int amount, seconds;
            public AlterMode mode;
        }

        public override IEnumerator ExecuteNode(AbicraftNodeExecution e)
        {
            AbicraftObject abj = GetInputValue<AbicraftObject>(e, "Obj");

            if (abj)
            {
                for (int i = 0; i < allSelectedIndices.Count; i++)
                {
                    AttributeAlter alter = allSelectedIndices[i];
                    switch (alter.mode)
                    {
                        case AlterMode.Change:
                            abj.ImpactAttributeValue(e.ae.senderObject, alter.attribute, alter.amount);
                            break;
                        case AlterMode.ChangeForSeconds:
                            abj.ImpactAttributeForSeconds(e.ae.senderObject, alter.attribute, alter.amount, alter.seconds);
                            break;
                        case AlterMode.Set:
                            abj.SetAttributeValue(e.ae.senderObject, alter.attribute, alter.amount);
                            break;
                        case AlterMode.SetForSeconds:
                            abj.SetAttributeValueForSeconds(e.ae.senderObject, alter.attribute, alter.amount, alter.seconds);
                            break;

                    }                      
                }
            }

            yield return null;
        }
    }
}


