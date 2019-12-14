using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace AbicraftNodes.VFX
{
    public class AudioNode : AbicraftExecutionNode
    {
        public static uint id = 113;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AbicraftObject Obj;
        public AudioClip Clip;

        public override void Initialize(AbicraftNodeExecution execution)
        {
            
        }

        public override IEnumerator ExecuteNode(AbicraftNodeExecution e)
        {
            AbicraftObject obj = GetInputValue<AbicraftObject>(e, "Obj", Obj);

            if(obj != null)
            {
                AudioSource aSource = obj.GetComponent<AudioSource>();

                if (aSource)
                {
                    AudioClip clip = GetInputValue<AudioClip>(e, "Clip", Clip);

                    if (clip != null)
                        aSource.PlayOneShot(clip);
                }
                else
                {
                    Debug.LogWarning("Abicraft: AudioNode input object dont have AudioSource.");
                }
            }
            yield return null;
        }
    }
}