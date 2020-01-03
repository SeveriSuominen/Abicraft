using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Attribute", menuName = "Abicraft/Object/Attribute", order = 2)]
public class AbicraftAttribute : ScriptableObject
{
    public string Name;
    
    public enum Type
    {
        Defensive,
        Boosting,
        Consutable
    }
}
