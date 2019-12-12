using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[System.Serializable]
[CreateAssetMenu(fileName = "Ability", menuName = "Abicraft/Graphs/Ability", order = 2)]
public class Ability : NodeGraph
{
    public float Cooldown;
}
