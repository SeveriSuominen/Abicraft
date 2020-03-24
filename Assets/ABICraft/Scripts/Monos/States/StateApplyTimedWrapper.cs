using AbicraftCore;
using AbicraftMonos;
using AbicraftMonos.Action;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateApplyTimedWrapper : AbicraftActionMono
{
    public AbicraftObject obj;
    public AbicraftObject senderObject;
    public AbicraftState state;
    public float passiveAbilityLifetime;

    public override void OnActive()
    {
        if(Active && obj != null)
        {
            AbicraftGlobalContext.abicraft.dispatcher.Dispatch(obj, state.statePassiveAbility, passiveAbilityLifetime);
            obj.activeStates.Add(state);
        }
    }

    public override void OnComplete(bool success)
    {
        if (obj)
        {
            obj.RemoveState(senderObject, state);
        }
    }

    void Update()
    {
        base.ActionMonoUpdate(Time.deltaTime);

        if (Active && obj == null)
            CompleteActionAs(false);
    }
}
