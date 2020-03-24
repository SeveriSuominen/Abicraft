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
    public class HasAttributeAmountNode : AbicraftExecutionNode
    {
        
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AbicraftObject Obj;

        [HideInInspector]
        public int    selectedIndex, lastIndex;

        [HideInInspector]
        public List<AttributeAmount> allSelectedIndices = new List<AttributeAmount>();
        [HideInInspector]


        public enum ConditionMode
        {
            Greater,
            Lesser
        }

        [System.Serializable]
        public class AttributeAmount
        {
            public AttributeAmount(AbicraftAttribute attribute)
            {
                this.attribute = attribute;
            }

            public AbicraftAttribute attribute;
            public ConditionMode conditionMode;
            public int amount;
        }

        public override void Initialize(AbicraftNodeExecution e)
        {
            e.Block();
        }

        public override IEnumerator ExecuteNode(AbicraftNodeExecution e)
        {
            AbicraftObject abj = GetInputValue<AbicraftObject>(e, "Obj");

            bool hasAllAttributes = true;

            if (abj)
            {
                for (int i = 0; i < allSelectedIndices.Count; i++)
                {
                    AttributeAmount amount = allSelectedIndices[i];
                 
                    switch (amount.conditionMode)
                    {
                        case ConditionMode.Greater:
                            if (abj.GetAttributeAmount(e.ae.senderObject, amount.attribute) < amount.amount)
                            {
                                hasAllAttributes = false;
                            }
                            break;
                        case ConditionMode.Lesser:
                            if (abj.GetAttributeAmount(e.ae.senderObject, amount.attribute) > amount.amount)
                            {
                                hasAllAttributes = false;
                            }
                            break;
                    }
                }
            }
            else
            {
                Debug.LogError("Abicraft: Abicraft Object is null");
                hasAllAttributes = false;
            }
         
            if (!hasAllAttributes)
            {
                e.EndExecutionBranch();
            }
            else
            {
                e.ReleaseBlock();
            }

            yield return null;
        }
    }
}


