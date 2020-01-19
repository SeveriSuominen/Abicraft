using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;
using AbicraftCore.Variables;

/// <summary> Holds all Abicraft core components and scripts </summary>
namespace AbicraftCore
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Ability", menuName = "Abicraft/Graphs/Ability", order = 2)]
    /// <summary> Abicraft ability, contains all execution info of ability that can be dispatched, with AbicraftAbilityDispatcher, edited with node editor</summary>
    public class AbicraftAbility : AbicraftNodeEditor.AbicraftAbilityGraph {}
}
