using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;
using AbicraftCore;
using AbicraftMonos;
using AbicraftNodes.Meta;

namespace AbicraftNodes.Action
{
    public class AudioNode : AbicraftExecutionNode
    {
        public static uint id = 113;

        [Input(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AbicraftObject Obj;
        public AudioClip Clip;

        public bool SetAudioSourceSettings;

        [Range(0, 256)]
        public int priority = 128;
        [Range(0, 1)]
        public float volume = 1;
        [Range(0, 3)]
        public float pitch = 1;
       /* [Range(-1, 1)]
        public float stereoPan;
        [Range(0, 1)]
        public float spatialBlend;
        [Range(0, 1)]
        public float reverbZoneMix;*/

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
                    {
                       /* int   orgPriority = aSource.priority;
                        float orgVolume   = aSource.volume;
                        float orgPitch    = aSource.pitch;

                        if (SetAudioSourceSettings)
                        {
                            aSource.priority = priority;
                            aSource.volume = volume;
                            aSource.pitch = pitch;
                        }*/
                        aSource.PlayOneShot(clip);

                        /*aSource.priority = orgPriority;
                        aSource.volume = orgVolume;
                        aSource.pitch = orgPitch;*/
                    }
                    else
                    {
                        Debug.LogWarning("Abicraft: AudioNode trying to play null audio clip");
                    }
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