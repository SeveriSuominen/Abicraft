using AbicraftCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "ObjectType", menuName = "Abicraft/Types/ObjectProfile", order = 2)]
public class AbicraftObjectProfile : ScriptableObject
{
    public string TypeName;
    public bool PhysicalObject;

    public readonly List<AbicraftAttribute.AbicraftObjectAttribute>    attributes        = new List<AbicraftAttribute.AbicraftObjectAttribute>();
    public readonly List<AbicraftState>                                allwaysHasStates  = new List<AbicraftState>();
    public readonly List<AbicraftState>                                immuneToStates    = new List<AbicraftState>();

    public bool HasAttribute(AbicraftAttribute attribute)
    {
        for (int i = 0; i < attributes.Count; i++)
        {
            if (attributes[i].attribute == attribute)
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
