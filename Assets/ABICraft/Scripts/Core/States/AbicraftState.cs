using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AbicraftCore
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Attribute", menuName = "Abicraft/Object/State", order = 2)]
    public class AbicraftState : ScriptableObject
    {
        [System.Serializable]
        public enum StateType
        {
            Positive,
            Negative,
            Neutral
        }

        public string    name;
        public Texture2D icon;
        public StateType type;

        public AbicraftAbility statePassiveAbility;
    }
}
