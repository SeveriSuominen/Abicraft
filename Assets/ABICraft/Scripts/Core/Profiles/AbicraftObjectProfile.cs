using AbicraftCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static AbicraftAttribute;

[System.Serializable]
[CreateAssetMenu(fileName = "ObjectType", menuName = "Abicraft/Types/ObjectProfile", order = 2)]
public class AbicraftObjectProfile : ScriptableObject
{
    public string TypeName;
    public bool PhysicalObject;
    public bool Targetable;

    public List<AbicraftAttribute.AbicraftObjectAttribute>    attributeObjects        = new List<AbicraftAttribute.AbicraftObjectAttribute>();
    public List<AbicraftState>                                allwaysHasStates  = new List<AbicraftState>();
    public List<AbicraftState>                                immuneToStates    = new List<AbicraftState>();

     

    public AbicraftObjectAttribute AddAttributeObject(AbicraftAttribute attribute)
    {
        var contains = false;

        for (int i = 0; i < attributeObjects.Count; i++)
        {
            if (attributeObjects[i].attribute == attribute)
            {
                contains = true;
                break;
            }
        }

        if (!contains)
        {
            attributeObjects.Add(new AbicraftObjectAttribute(attribute));
            return attributeObjects[attributeObjects.Count - 1];
        }
        else
        {
            return null;
        }
    }

    public AbicraftObjectAttribute GetAttributeObject(AbicraftAttribute attribute)
    {
        for (int i = 0; i < attributeObjects.Count; i++)
        {
            if (attributeObjects[i].attribute == attribute)
                return attributeObjects[i];
        }
        return null;
    }

    public bool HasAttributeObject(AbicraftAttribute attribute)
    {
        for (int i = 0; i < attributeObjects.Count; i++)
        {
            if (attributeObjects[i].attribute == attribute)
                return true;
        }
        return false; 
    }

    //IMPLEMENTATION - IMMUNE_TO
    public bool IsImmuneToState(AbicraftState state)
    {
        return this.immuneToStates.Contains(state);
    }

    //IMPLEMENTATION - ALLWAYS_HAS
    public bool AllwaysHasState(AbicraftState state)
    {
        return this.allwaysHasStates.Contains(state);
    }
}
