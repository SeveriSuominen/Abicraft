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

    public static AbicraftAttribute Attribute(string name)
    {
        for (int i = 0; i < AbicraftGlobalContext.abicraft.dataFile.GlobalAttributes.Count; i++)
        {
            var attr = AbicraftGlobalContext.abicraft.dataFile.GlobalAttributes[i];

            if (attr.AttributeName == name)
                return attr;
        }
        return null;
    }

    public enum AttributeCategory
    {
        Base,
        Special
    }

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
            //TEMPORARY
            List<AbicraftAttribute> all_attrs = AbicraftGlobalContext.abicraft.dataFile.GlobalAttributes;
            ATTRIBUTES = new AbicraftObjectAttribute[all_attrs.Count];

            for (int i = 0; i < all_attrs.Count; i++)
            {
                ATTRIBUTES[i] = new AbicraftObjectAttribute(all_attrs[i]);
            }
           

            //ATTRIBUTES = new AbicraftObjectAttribute[profile.attributes.Count];
            /*
            for (int i = 0; i < profile.attributes.Count; i++)
            {
                ATTRIBUTES[i] = new AbicraftObjectAttribute(profile.attributes[i]);
            }*/
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

        public AbicraftObjectAttribute(){}
        public AbicraftObjectAttribute(AbicraftAttribute attribute)
        {
            this.attribute = attribute;
        }
    }
}
