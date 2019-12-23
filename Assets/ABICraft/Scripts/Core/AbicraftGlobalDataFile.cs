using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbicraftCore
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AbicraftGlobalDataFile", menuName = "Abicraft/Global data file", order = 2)]
    public class AbicraftGlobalDataFile : ScriptableObject
    {
        public List<AbicraftState> GlobalStates = new List<AbicraftState>();
        public List<AbicraftObject> InstantiateToPool = new List<AbicraftObject>();
    }
}
