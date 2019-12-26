using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AbicraftCore
{
    [System.Serializable]
    public class AbicraftState
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
