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

        public enum Interaction
        {
            Success,
            Failed,
            ImmuneToState,
            DontHaveAttribute,
            AlreadyHasState,
            AllwaysMustHaveState
        }

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
                    ApplyState(null, activeStates[i]);
                }
            }
        }
        public int GetAttributeAmount(AbicraftObject senderObject, AbicraftAttribute attribute)
        {
            AbicraftAttribute.AbicraftObjectAttribute attributeObject;

            if ((attributeObject = attributes[attribute]) != null)
            {
                return attributeObject.baseValue;
            }
            return -1;
        }

        public  Interaction SetAttributeValue(AbicraftObject senderObject, AbicraftAttribute attribute, int setTo)
        {
            AbicraftAttribute.AbicraftObjectAttribute attributeObject;

            if ((attributeObject = attributes[attribute]) != null)
            {
                attributeObject.baseValue = setTo;
                return Interaction.Success;
            }
            return Interaction.DontHaveAttribute;
        }

        public Interaction ImpactAttributeValue(AbicraftObject senderObject, AbicraftAttribute attribute, int amount)
        {
            AbicraftAttribute.AbicraftObjectAttribute attributeObject;

            if ((attributeObject = attributes[attribute]) != null)
            {
                attributeObject.baseValue = attributeObject.baseValue + amount;
                return Interaction.Success;
            }
            return Interaction.DontHaveAttribute;
        }

        public Interaction SetAttributeValueForSeconds(AbicraftObject senderObject, AbicraftAttribute attribute, int setTo, float seconds)
        {
            AbicraftAttribute.AbicraftObjectAttribute attributeObject;

            if ((attributeObject = attributes[attribute]) != null)
            {
                AttributeChangeTimedWrapper wrapper = gameObject.AddComponent<AttributeChangeTimedWrapper>();

                wrapper.obj = this;
                wrapper.attribute = attribute;
                wrapper.setToValue = setTo;

                wrapper.StartActionMono(seconds, true);
                return Interaction.Success;
            }
            return Interaction.DontHaveAttribute;
        }

        public Interaction ImpactAttributeForSeconds(AbicraftObject senderObject, AbicraftAttribute attribute, int amount, float seconds)
        {
            AbicraftAttribute.AbicraftObjectAttribute attributeObject;

            if ((attributeObject = attributes[attribute]) != null)
            {
                AttributeChangeTimedWrapper wrapper = gameObject.AddComponent<AttributeChangeTimedWrapper>();

                wrapper.obj = this;
                wrapper.attribute = attribute;
                wrapper.setToValue = attributeObject.baseValue + amount;

                wrapper.StartActionMono(seconds, true);

                return Interaction.Success;
            }
            return Interaction.DontHaveAttribute;
        }

        public Interaction ApplyStateForSeconds(AbicraftObject senderObject, AbicraftState state, float seconds = -1)
        {
            if (!activeStates.Contains(state))
            {
                if(!Profile.IsImmuneToState(state))
                {
                    if (state.statePassiveAbility.Passive == false)
                    {
                        Debug.LogError("Abicraft: Only passive abilities allowed for states");
                        return Interaction.Failed;
                    }

                    StateApplyTimedWrapper wrapper = gameObject.AddComponent<StateApplyTimedWrapper>();

                    wrapper.obj = this;
                    wrapper.state = state;
                    wrapper.passiveAbilityLifetime = seconds < 0 ? wrapper.state.statePassiveAbility.DefaultLifetime : seconds;

                    wrapper.StartActionMono(wrapper.passiveAbilityLifetime, true);

                    return Interaction.Success;
                }

                return Interaction.ImmuneToState;
            }
            return Interaction.AlreadyHasState;
        }

        public Interaction ApplyState(AbicraftObject senderObject, AbicraftState state)
        {
            if (!activeStates.Contains(state))
            {
                if (!Profile.IsImmuneToState(state))
                {
                    if (state.statePassiveAbility.Passive == false)
                    {
                        Debug.LogError("Abicraft: Only passive abilities allowed for states");
                        return Interaction.Failed;
                    }
                    AbicraftGlobalContext.abicraft.dispatcher.Dispatch(this, state.statePassiveAbility, state.statePassiveAbility.DefaultLifetime);
                    activeStates.Add(state);
                    return Interaction.Success;
                }
                return Interaction.ImmuneToState;
            }
            return Interaction.AlreadyHasState;
        }

        public Interaction RemoveState(AbicraftObject senderObject, AbicraftState state)
        {
            if (activeStates.Contains(state))
            {
                if (!Profile.AllwaysHasState(state))
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
                        if (wrappers[i].state == state)
                            wrappers[i].CompleteActionAs(true);
                    }
                    activeStates.Remove(state);
                    return Interaction.Success;
                }
                return Interaction.AllwaysMustHaveState;
            }
            return Interaction.Success;
        }

        public List<Interaction> RemoveStates(AbicraftObject senderObject, params AbicraftState[] states)
        {
            List<Interaction> interactions = new List<Interaction>();

            for (int i = 0; i < states.Length; i++)
            {
                interactions.Add(RemoveState(senderObject, states[i]));
            }

            return interactions;
        }

        public List<Interaction> RemoveAllStates(AbicraftObject senderObject)
        {
            List<Interaction> interactions = new List<Interaction>();

            for (int i = 0; i < activeStates.Count; i++)
            {
                if (!Profile.AllwaysHasState(activeStates[i]))
                {
                    activeStates.RemoveAt(i);
                    interactions.Add(Interaction.Success);
                }
                interactions.Add(Interaction.AllwaysMustHaveState);
            }
            return interactions;
        }

        public List<Interaction> RemoveAllStatesTypeOf(AbicraftObject senderObject, AbicraftState.StateType stateType)
        {
            List<Interaction> interactions = new List<Interaction>();

            for (int i = 0; i < activeStates.Count; i++)
            {
                if (activeStates[i].type == stateType && !Profile.AllwaysHasState(activeStates[i]))
                {
                    activeStates.RemoveAt(i);
                    interactions.Add(Interaction.Success);
                }
                interactions.Add(Interaction.AllwaysMustHaveState);
            }
            return interactions;
        }

        public void ResetObject()
        {
            transform.position   = Original.transform.position;
            transform.rotation   = Original.transform.rotation;
            transform.localScale = Original.transform.localScale;

            ImplementProfile();
        }
    }
}


