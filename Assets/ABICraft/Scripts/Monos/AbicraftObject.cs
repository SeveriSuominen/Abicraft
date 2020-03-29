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
            AllwaysMustHaveState,
            NoCastEffectsSet
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

        public void ValidateAttributeObject(ref AbicraftAttribute.AbicraftObjectAttribute attributeObj)
        {
            int max = 0;

            if((max = Max(attributeObj.attribute)) != int.MinValue)
            {
                attributeObj.baseValue = attributeObj.baseValue > max ? max : attributeObj.baseValue;
            }
        }

        public Interaction CastAttributeOn(int amount, AbicraftObject senderObject, AbicraftAttribute attribute)
        {
            return Cast(amount, attribute, senderObject, this);
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

        public  Interaction SetAttributeValue(AbicraftObject senderObject, AbicraftAttribute attr, int setTo)
        {
            AbicraftAttribute.AbicraftObjectAttribute attributeObj;

            if ((attributeObj = attributes[attr]) != null)
            {
                attributeObj.baseValue = setTo;

                ValidateAttributeObject(ref attributeObj);

                return Interaction.Success;
            }
            return Interaction.DontHaveAttribute;
        }

        public Interaction ImpactAttributeValue(AbicraftObject senderObject, AbicraftAttribute attribute, int amount)
        {
            AbicraftAttribute.AbicraftObjectAttribute attributeObj;

            if ((attributeObj = attributes[attribute]) != null)
            {
                attributeObj.baseValue = attributeObj.baseValue + amount;
                ValidateAttributeObject(ref attributeObj);

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

                    if(AbicraftGUI.Instance)
                        AbicraftGUI.Instance.SpawnObjectImpact(this, state);
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

                    AbicraftGUI.Instance.SpawnObjectImpact(this, state);
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

        public int Max(AbicraftAttribute attr)
        {
            AbicraftAttribute.AttributeEffect max_effect = null;
            int value = 0;

            for (int i = 0; i < attr.effects.Count; i++)
            {
                if (attr.effects[i].effect == AbicraftAttribute.AttributeEffect.Effect.Max)
                {
                    max_effect = attr.effects[i];
                }
            }

            if (max_effect != null && max_effect.options.Count > 0)
            {
                for (int i = 0; i < max_effect.options.Count; i++)
                {
                    var effectoption = max_effect.options[i];

                    switch (effectoption.option)
                    {
                        case AbicraftAttribute.AttributeEffect.EffectOption.Add:
                            value += GetAttributeAmount(this, effectoption.attribute);
                            break;
                        case AbicraftAttribute.AttributeEffect.EffectOption.Substract:
                            value -= GetAttributeAmount(this, effectoption.attribute);
                            break;
                        case AbicraftAttribute.AttributeEffect.EffectOption.Multiply:
                            value *= GetAttributeAmount(this, effectoption.attribute);
                            break;
                        case AbicraftAttribute.AttributeEffect.EffectOption.Divide:
                            value /= GetAttributeAmount(this, effectoption.attribute);
                            break;
                    }
                }
                return value;
            }
            else
            {
                return int.MinValue;
            }
        }

        Interaction Cast(int amount, AbicraftAttribute attr, AbicraftObject self, AbicraftObject target)
        {
            List<AbicraftAttribute.AttributeEffect> casteffects = new List<AbicraftAttribute.AttributeEffect>();

            for (int i = 0; i < attr.effects.Count; i++)
            {
                if (attr.effects[i].effect == AbicraftAttribute.AttributeEffect.Effect.OnCast)
                {
                    casteffects.Add(attr.effects[i]);
                }
            }

            for (int i = 0; i < casteffects.Count; i++)
            {
                int value = amount;

                if (casteffects[i] != null && casteffects[i].options.Count > 0)
                {
                    for (int j = 0; j < casteffects[i].options.Count; j++)
                    {
                        var effectoption = casteffects[i].options[j];
                        var optionAbj = effectoption.targetOption == AbicraftAttribute.AttributeEffect.TargetOption.Target ?
                            target :
                            self;

                        switch (effectoption.operation)
                        {
                            case AbicraftAttribute.AttributeEffect.OperationOption.Amount:
                                switch (effectoption.option)
                                {
                                    case AbicraftAttribute.AttributeEffect.EffectOption.Add:
                                        value = Mathf.FloorToInt(value + (optionAbj.GetAttributeAmount(self, effectoption.attribute) * effectoption.amount));
                                        break;
                                    case AbicraftAttribute.AttributeEffect.EffectOption.Substract:
                                        value = Mathf.FloorToInt(value - (optionAbj.GetAttributeAmount(self, effectoption.attribute) * effectoption.amount));
                                        break;
                                    case AbicraftAttribute.AttributeEffect.EffectOption.Multiply:
                                        value = Mathf.FloorToInt(value * (optionAbj.GetAttributeAmount(self, effectoption.attribute) * effectoption.amount));
                                        break;
                                    case AbicraftAttribute.AttributeEffect.EffectOption.Divide:
                                        value = Mathf.FloorToInt(value / (optionAbj.GetAttributeAmount(self, effectoption.attribute) * effectoption.amount));
                                        break;
                                }
                                break;
                            case AbicraftAttribute.AttributeEffect.OperationOption.Cast:
                                var finalValue = Mathf.FloorToInt(value * effectoption.amount);
                                optionAbj.ImpactAttributeValue(self, effectoption.attribute, finalValue);

                                if(AbicraftGUI.Instance)
                                    AbicraftGUI.Instance.SpawnObjectImpact(this, effectoption.attribute, finalValue);
                                break;
                        }
                    }
                }
            }
            return Interaction.Success;
        }
    }
}


