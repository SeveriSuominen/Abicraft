using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace AbicraftNodes.VFX
{
    public class ShakeNode : AbicraftExecutionNode
    {
        public static uint id = 113;

        [Input] public AbicraftObject obj;

        // How long the object should shake for.
        [Input] public float shakeDuration = 10f;

        // Amplitude of the shake. A larger value shakes the camera harder.
        [Input] public float shakeAmount = 0.7f;
        [Input] public float decreaseFactor = 1.0f;

        public override void Initialize(AbicraftAbilityExecution.AbicraftNodeExecution execution)
        {
            
        }

        public override IEnumerator ExecuteNode(AbicraftAbilityExecution.AbicraftNodeExecution execution)
        {
            AbiCraftStateSnapshot   snapshot = execution.AbilityExecution.initial_snapshot;
            Shake shakeMono = snapshot.player.gameObject.AddComponent<Shake>();

            shakeMono.target = GetInputValue<AbicraftObject>("obj").gameObject;

            shakeMono.decreaseFactor = decreaseFactor;
            shakeMono.shakeAmount    = shakeAmount;
            shakeMono.shakeDuration  = shakeDuration;

            yield return null;
        }
    }
}