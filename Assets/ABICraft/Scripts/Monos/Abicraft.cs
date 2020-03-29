using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;
using AbicraftCore;

namespace AbicraftMonos
{
    [System.Serializable]
    [ExecuteInEditMode]
    [RequireComponent(typeof(AbicraftAbilityDispatcher))]
    public class Abicraft : MonoBehaviour
    {
        public AbicraftGlobalDataFile dataFile;

        [HideInInspector]
        public AbicraftAbilityDispatcher dispatcher;

        public List<AbicraftObjectPoolInstantiate> InstantiateToPool = new List<AbicraftObjectPoolInstantiate>();

        private void Awake()
        {
            if (!AbicraftGlobalContext.abicraft)
                AbicraftGlobalContext.TryInjectAbicraftInstance();
        }

        private void Update()
        {
            if (!AbicraftGlobalContext.abicraft)
                AbicraftGlobalContext.TryInjectAbicraftInstance();
        }

        public void Inject()
        {
            AbicraftGlobalContext.AddAbicraftInstance(this);
        }

        private void Start()
        {
            if (Application.isPlaying)
            {
                if (!dataFile)
                {
                    Debug.LogError("Abicraft: Missing data file reference");
                    return;
                }

                AbicraftGlobalContext.AddAbicraftInstance(this);

                AbicraftGameStateSnapshot.InjectInputDataReferences(
                    new AbicraftInputReferences
                    {
                        Player = null
                    }
                );

                AbicraftObjectPool.LoadPooledObjects(InstantiateToPool);
            }
        }
    }
}
