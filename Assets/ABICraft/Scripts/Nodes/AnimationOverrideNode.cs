using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

using AbicraftNodeEditor;
using AbicraftCore;
using AbicraftMonos;
using AbicraftNodes.Meta;

namespace AbicraftNodes.Action
{
    public class AnimationOverrideNode : AbicraftExecutionNode
    {
        public static readonly List<AbicraftObject> IsAnimating = new List<AbicraftObject>();

        [Output]
        public float LengthSeconds;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AbicraftObject Obj;
        private AbicraftObject obj;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public float  Speed = 1;

        [HideInInspector]
        public int    selectedIndex;
        //Wanna pass arguments for editor? Use non-seriziable sturct to hold that data
        //and access it in custom editor via Target.
        [HideInInspector]
        public AnimationClip[] clips;

        //-----------------------------------------------------------------------------

        AnimatorOverrideController overrideController;
        int prevLoadAnimPathHash;
        string prevLoadAnimName;

        Animator animator;
   
        AnimatorStateInfo[] layerInfo;
        List<AnimatorClipInfo[]> clipInfos = new List<AnimatorClipInfo[]>();

        public AnimationClip clip;
        Animation org_clip;

        public override IEnumerator ExecuteNode(AbicraftNodeExecution e)
        {
            obj = GetInputValue<AbicraftObject>(e, "Obj");
            animator = obj.GetComponent<Animator>();

            var navAgent = obj.GetComponent<NavMeshAgent>();

            if (navAgent)
                navAgent.ResetPath();

            clips = animator.runtimeAnimatorController.animationClips;

            if (obj && clip)
            {
                if (animator != null)
                {
                    if (IsAnimating.Contains(obj) && overrideController != null)
                       animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;

                    if (animator.runtimeAnimatorController.GetType() != typeof(AnimatorOverrideController))
                    {
                        AddObjectToIterationIndex<float>(e, "LengthSeconds", clip.length / (Speed));

                        if (!IsAnimating.Contains(obj))
                            IsAnimating.Add(obj);

                        animator.speed = Speed;
                        overrideController = new AnimatorOverrideController();

                        if (!overrideController.runtimeAnimatorController)
                            overrideController.runtimeAnimatorController = animator.runtimeAnimatorController;

                        animator.runtimeAnimatorController = overrideController;
                        
                        LoadAnimation(clip);

                        yield return new WaitForSeconds(clip.length / (Speed));

                        UnloadPreviousLoadAnimation();
                        animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;

                        if (IsAnimating.Contains(obj))
                            IsAnimating.Remove(obj);
                    }
                }
            }
            yield return null;
        }

        public override object GetValue(AbicraftNodeExecution e, NodePort port)
        {
            if(port.fieldName.Equals("LengthSeconds"))
            {
                return GetObjectByIterationIndex<float>(e, "LengthSeconds");
            }

            return GetInputValue<AbicraftLifeline>(e, "In");
        }

        ///<summary>Set and play clip on override contoller</summary>
        void LoadAnimClip(AnimationClip clip)
        {
            overrideController[clipInfos[0][0].clip.name] = clip;
            animator.Update(0.0f);
            // Push back state
            for (int i = 0; i < animator.layerCount; i++)
            {
                animator.Play(layerInfo[i].fullPathHash, i, 0);
            }

            animator.SetTrigger("OVERRIDE");

            // Force an update
            
        }

        ///<summary>Load animation to override controller</summary>
        void LoadAnimation(AnimationClip clip)
        {
            //Save current state
            layerInfo = new AnimatorStateInfo[animator.layerCount];

            for (int i = 0; i < animator.layerCount; i++)
            {
                layerInfo[i] = animator.GetCurrentAnimatorStateInfo(i);
                clipInfos.Add( animator.GetCurrentAnimatorClipInfo(i));
            }
            prevLoadAnimName = clipInfos[0][0].clip.name;

            LoadAnimClip(clip);
        }

        ///<summary>Unload animation from override controller</summary>
        void UnloadPreviousLoadAnimation()
        {
            for (int i = 0; i < animator.layerCount; i++)
            {
                layerInfo[i] = animator.GetCurrentAnimatorStateInfo(i);
            }

            overrideController[prevLoadAnimName] = null;

            for (int i = 0; i < animator.layerCount; i++)
            {
                animator.Play(layerInfo[i].fullPathHash, i, layerInfo[i].normalizedTime);
            }

            // Force an update
            animator.Update(0.0f);
        }
    }
}


