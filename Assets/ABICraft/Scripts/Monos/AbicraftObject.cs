using AbicraftCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbicraftMonos
{
    [System.Serializable]
    public sealed class AbicraftObject : MonoBehaviour
    {
        [SerializeField]
        public bool InstantiateObjectToPool;
        [SerializeField]
        public int InstantiateToPoolAmount;
        [HideInInspector]
        public AbicraftObject Original;
        [HideInInspector]
        public bool ActivePool;
        [HideInInspector]
        public int PoolIndex;
        [HideInInspector]
        public bool IsSceneObject;
        [HideInInspector]
        public bool IsPoolingClone;

        public readonly List<AbicraftState> activeStates = new List<AbicraftState>();

        void Awake()
        {
            AbicraftGlobalContext.AllObjects.Add(this);
        }

        private void OnDestroy()
        {
            AbicraftGlobalContext.AllObjects.Remove(this);
        }

        public void ApplyTimedState(AbicraftState state, float seconds = -1)
        {
            if (!activeStates.Contains(state))
            {
                if (state.statePassiveAbility.Passive == false)
                {
                    Debug.LogError("Abicraft: Only passive abilities allowed for states");
                    return;
                }

                StateApplyTimedWrapper wrapper = gameObject.AddComponent<StateApplyTimedWrapper>();

                wrapper.obj   = this;
                wrapper.state = state;
                wrapper.passiveAbilityLifetime = seconds < 0 ? wrapper.state.statePassiveAbility.DefaultLifetime : seconds;

                wrapper.StartActionMono(wrapper.passiveAbilityLifetime, true);
            }
        }

        public void ApplyState(AbicraftState state)
        {
            if (!activeStates.Contains(state))
            {
                if(state.statePassiveAbility.Passive == false)
                {
                    Debug.LogError("Abicraft: Only passive abilities allowed for states");
                    return;
                }

                AbicraftGlobalContext.abicraft.dispatcher.Dispatch(this, state.statePassiveAbility, 5);
                activeStates.Add(state);
            }
        }

        public void RemoveState(AbicraftState state)
        {
            if (activeStates.Contains(state))
            {
                AbicraftAbilityDispatcher dispatcher = AbicraftGlobalContext.abicraft.dispatcher;
                List<AbicraftAbilityExecution> executions = dispatcher.GetActiveExecutionsBySenderObject(state.statePassiveAbility, this);

                for (int i = 0; i < executions.Count; i++)
                {
                    dispatcher.EndAbicraftAbilityExecution(executions[i]);
                }

                StateApplyTimedWrapper[] wrappers = GetComponents<StateApplyTimedWrapper>();

                for (int i = 0; i < wrappers.Length; i++)
                {
                    if(wrappers[i].state == state)
                        wrappers[i].CompleteActionAs(true);
                }
                activeStates.Remove(state);
            }
        }

        public void RemoveAllStates()
        {
            for (int i = 0; i < activeStates.Count; i++)
            {
                activeStates.RemoveAt(i);
            }
        }

        public void RemoveAllStatesTypeOf(AbicraftState.StateType stateType)
        {
            for (int i = 0; i < activeStates.Count; i++)
            {
                if (activeStates[i].type == stateType)
                    activeStates.RemoveAt(i);
            }
        }

        public void ResetObject()
        {
            transform.position   = Original.transform.position;
            transform.rotation   = Original.transform.rotation;
            transform.localScale = Original.transform.localScale;
        }
    }
}


