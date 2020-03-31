using AbicraftCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Attribute", menuName = "Abicraft/Object/Attribute", order = 2)]
public class AbicraftAttribute : ScriptableObject
{
    public string       AttributeName;
    public Texture2D    AttributeIcon;
    public AttributeCategory Category;

    public List<AttributeEffect> effects = new List<AttributeEffect>();

    public static AbicraftAttribute Attribute(string name)
    {
        if (!AbicraftGlobalContext.HasValidAbicraftInstance())
            return null;

        for (int i = 0; i < AbicraftGlobalContext.abicraft.dataFile.GlobalAttributes.Count; i++)
        {
            var attr = AbicraftGlobalContext.abicraft.dataFile.GlobalAttributes[i];

            if (attr.AttributeName == name)
                return attr;
        }
        return null;
    }

    [System.Serializable]
    public class AttributeEffect
    {
        public Effect effect;

        public List<AttributeEffectOption> options = new List<AttributeEffectOption>();

        [System.Serializable]
        public class AttributeEffectOption
        {
            public OperationOption   operation;
            public EffectOption      option;
            public EffectCastOption  castoption;
            public TargetOption      targetOption;
            public float amount;
            public AbicraftAttribute attribute;
            public int attribute_index;
        }


        [System.Serializable]
        public enum OperationOption
        {
            Amount, Cast
        }

        [System.Serializable]
        public enum TargetOption
        {
            Self, Target
        }

        [System.Serializable]
        public enum EffectOption
        {
            Substract, Add, Divide, Multiply
        }

        [System.Serializable]
        public enum EffectCastOption
        {
            Value, Substract, Add, Divide, Multiply
        }

        [System.Serializable]
        public enum Effect
        {
            Max, Min, OnCast, Regenerate 
        }
    }
    [System.Serializable]
    public enum AttributeCategory
    {
        Base,
        Special
    }
    [System.Serializable]
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


        public AbicraftObjectAttribute[] GetList()
        {
            return ATTRIBUTES;
        }

        public AbicraftObjectAttributeMap(AbicraftObjectProfile profile)
        {
            ATTRIBUTES = new AbicraftObjectAttribute[profile.attributeObjects.Count];
            
            for (int i = 0; i < profile.attributeObjects.Count; i++)
            {
                ATTRIBUTES[i] = new AbicraftObjectAttribute(profile.attributeObjects[i]);
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
        public int baseValue;
        public bool scaling;

        public AbicraftObjectAttribute(){}

        public AbicraftObjectAttribute(AbicraftAttribute attribute)
        {
            this.attribute  = attribute;
            this.powerCurve = new AnimationCurve();
        }

        public AbicraftObjectAttribute(AbicraftObjectAttribute attributeObj)
        {
            this.attribute  = attributeObj.attribute;
            this.powerCurve = attributeObj.powerCurve;
            this.baseValue  = attributeObj.baseValue;
            this.scaling    = attributeObj.scaling;
        }
    }
}
