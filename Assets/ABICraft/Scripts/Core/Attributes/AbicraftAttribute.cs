using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Attribute", menuName = "Abicraft/Object/Attribute", order = 2)]
public class AbicraftAttribute : ScriptableObject
{
    public string AttributeName;

    public enum Type
    {
        Increasive,
        Consutable
    }

    [System.Serializable]
    public class AbicraftObjectAttributeMap
    {
        readonly AbicraftObjectAttribute[] ATTRIBUTES;

        public int Count { get { return ATTRIBUTES.Length; } }

        public AbicraftObjectAttributeMap(AbicraftObjectProfile profile)
        {
            ATTRIBUTES = new AbicraftObjectAttribute[profile.attributes.Count];

            for (int i = 0; i < profile.attributes.Count; i++)
            {
                ATTRIBUTES[i] = new AbicraftObjectAttribute(profile.attributes[i]);
            }
        }

        public AbicraftObjectAttribute this[AbicraftAttribute attribute]
        {
            get
            {
                for (int i = 0; i < ATTRIBUTES.Length; i++)
                {
                    if (ATTRIBUTES[i].attribute.Equals(attribute))
                        return ATTRIBUTES[i];
                }
                return null;
            }
        }
    }

    [System.Serializable]
    public class AbicraftObjectAttribute
    {
#if UNITY_EDITOR
        [ReadOnly]
#endif
        public AbicraftAttribute attribute;

        public AnimationCurve powerCurve;
        public float baseValue;

        public AbicraftObjectAttribute(){}
        public AbicraftObjectAttribute(AbicraftObjectAttribute attributeObject)
        {
            this.attribute  = attributeObject.attribute;
            this.powerCurve = attributeObject.powerCurve;
            this.baseValue  = attributeObject.baseValue;
        }
    }
}
