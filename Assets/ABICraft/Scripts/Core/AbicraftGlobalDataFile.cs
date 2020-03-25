using AbicraftCore.Variables;
using AbicraftMonos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbicraftCore
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AbicraftGlobalDataFile", menuName = "Abicraft/Global data file", order = 2)]
    public class AbicraftGlobalDataFile : ScriptableObject
    {
        public List<AbicraftState>      GlobalStates      = new List<AbicraftState>();
        public List<AbicraftAttribute>  GlobalAttributes  = new List<AbicraftAttribute>();
        public List<AbicraftObject>     InstantiateToPool = new List<AbicraftObject>();

        [SerializeField]
        public List<AbicraftAbilityVariableDefinition> GlobalVariableDefinitions = new List<AbicraftAbilityVariableDefinition>();

        [HideInInspector]
        public InspectorCache cache;
    }

    [System.Serializable]
    public class InspectorCache
    {
        [System.Serializable]
        public class ScannedSceneObjCacheEntry
        {
            public Transform obj;
            public int indirectIndex;

            public ScannedSceneObjCacheEntry(Transform obj, int indirectIndex)
            {
                this.obj = obj;
                this.indirectIndex = indirectIndex;
            }
        }

        public int selectedTab = 0, lastTab = 0;

        public List<ScannedSceneObjCacheEntry> scannedSceneObjs = new List<ScannedSceneObjCacheEntry>();
        public List<AbicraftObject> scannedAssetAbjs = new List<AbicraftObject>();
        public List<AbicraftState> scannedStates = new List<AbicraftState>();

        public List<string> scannedAssetAbjsPaths = new List<string>();
        public List<string> scannedAssetStatePaths = new List<string>();
    }
}
