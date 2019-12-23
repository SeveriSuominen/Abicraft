using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;

namespace AbicraftCore
{
    [CreateAssetMenu(fileName = "Ability", menuName = "Abicraft/Graphs/Ability", order = 2)]
    public class AbicraftAbility : NodeGraph
    {
        public float Cooldown;
    }

}
