using AbicraftCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbicraftMonos
{
    [System.Serializable]
    public sealed class AbicraftObject : MonoBehaviour
    {
        //OBJECT PROFILE;
        public AbicraftObjectProfile Profile;

        [HideInInspector]
        public Rigidbody rigidBody;

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

        public AbicraftAttribute.AbicraftObjectAttributeMap attributes { get; private set; }
        public readonly List<AbicraftState> activeStates = new List<AbicraftState>();

        void Awake()
        {
            ImplementProfile();

            rigidBody = GetComponent<Rigidbody>();

            if(!rigidBody && Profile.PhysicalObject)
            {
                Debug.LogError("Abicraft: Physical Abicraft objects without rigidBody not allowed: " + gameObject.name);
                return;
            }

            AbicraftGlobalContext.AllObjects.Add(this);
        }

        private void OnDestroy()
        {
            AbicraftGlobalContext.AllObjects.Remove(this);
        }

        public void ImplementProfile()
        {
            if (Profile)
            {
                //IMPLEMENTING PROFILE ATTRIBUTES TO OBJECT, AND CREATEING ATTRIBUTE MAP;
                attributes = new AbicraftAttribute.AbicraftObjectAttributeMap(Profile);

                //APPLYING STATES TO OBJECT;
                for (int i = 0; i < Profile.allwaysHasStates.Count; i++)
                {
                    ApplyState(activeStates[i]);
                }
            }
        }

        public void SetAttributeValue(AbicraftAttribute attribute, float setTo, float seconds)
        {
            AbicraftAttribute.AbicraftObjectAttribute attributeObject;

            if ((attributeObject = attributes[attribute]) != null)
            {
                attributeObject.baseValue = setTo;
            }
        }

        public void SetAttributeValueForSeconds(AbicraftAttribute attribute, float setTo, float seconds)
        {
            AbicraftAttribute.AbicraftObjectAttribute attributeObject;

            if ((attributeObject = attributes[attribute]) != null)
            {
                AttributeChangeTimedWrapper wrapper = gameObject.AddComponent<AttributeChangeTimedWrapper>();

                wrapper.obj = this;
                wrapper.attribute = attribute;
                wrapper.setToValue = setTo;

                wrapper.StartActionMono(seconds, true);
            }
        }

        public void IncreaseAttributeForSeconds(AbicraftAttribute attribute, float amount, float seconds)
        {
            AbicraftAttribute.AbicraftObjectAttribute attributeObject;

            if ((attributeObject = attributes[attribute]) != null)
            {
                AttributeChangeTimedWrapper wrapper = gameObject.AddComponent<AttributeChangeTimedWrapper>();

                wrapper.obj = this;
                wrapper.attribute = attribute;
                wrapper.setToValue = attributeObject.baseValue + amount;

                wrapper.StartActionMono(seconds, true);
            }
        }

        public void ApplyStateForSeconds(AbicraftState state, float seconds = -1)
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
            if (!activeStates.Contains(state) && !Profile.IsImmuneToState(state))
            {
                if(state.statePassiveAbility.Passive == false)
                {
                    Debug.LogError("Abicraft: Only passive abilities allowed for states");
                    return;
                }

                AbicraftGlobalContext.abicraft.dispatcher.Dispatch(this, state.statePassiveAbility, state.statePassiveAbility.DefaultLifetime);
                activeStates.Add(state);
            }
        }

        public void RemoveState(AbicraftState state)
        {
            if (activeStates.Contains(state) && !Profile.AllwaysHasState(state))
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

        public void RemoveStates(params AbicraftState[] states)
        {
            for (int i = 0; i < states.Length; i++)
            {
                RemoveState(states[i]);
            }
        }

        public void RemoveAllStates()
        {
            for (int i = 0; i < activeStates.Count; i++)
            {
                if(!Profile.AllwaysHasState(activeStates[i]))
                    activeStates.RemoveAt(i);
            }
        }

        public void RemoveAllStatesTypeOf(AbicraftState.StateType stateType)
        {
            for (int i = 0; i < activeStates.Count; i++)
            {
                if (activeStates[i].type == stateType && !Profile.AllwaysHasState(activeStates[i]))
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


