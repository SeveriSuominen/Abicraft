using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

using AbicraftNodeEditor;
using AbicraftCore;

namespace AbicraftNodes.Action
{
    public class AnimationOverrideNode : AbicraftExecutionNode
    {
        public static readonly List<AbicraftObject> IsAnimating = new List<AbicraftObject>();

        /*[Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public string AnimationTrigger;*/
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AbicraftObject Obj;
        private AbicraftObject obj;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public float  Speed;

        [HideInInspector]
        public int    selectedIndex;
        //Wanna pass arguments for editor? Use non-seriziable sturct to hold that data
        //and access it in custom editor via Target.
        [HideInInspector]
        public AnimationClip[] clips;

        //-----------------------------------------------------------------------------

        AnimatorOverrideController overrideController;
        string prevLoadAnim;
        Animator animator;
        ResourceRequest request;
        AnimatorStateInfo[] layerInfo;

        public override void Initialize(AbicraftNodeExecution execution)
        {
           
        }

        public override IEnumerator ExecuteNode(AbicraftNodeExecution e)
        {

            obj = GetInputValue<AbicraftObject>(e, "Obj");
            animator = obj.GetComponent<Animator>();

            obj.GetComponent<NavMeshAgent>().ResetPath();

            clips = animator.runtimeAnimatorController.animationClips;

            if (obj)
            {
                if (animator != null)
                {
                    if (IsAnimating.Contains(obj) && overrideController != null)
                        animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
                   
                    if (animator.runtimeAnimatorController.GetType() != typeof(AnimatorOverrideController))
                    {
                        if (!IsAnimating.Contains(obj))
                            IsAnimating.Add(obj);

                        animator.speed = Speed;

                        overrideController = new AnimatorOverrideController();

                        if (!overrideController.runtimeAnimatorController)
                            overrideController.runtimeAnimatorController = animator.runtimeAnimatorController;

                        animator.runtimeAnimatorController = overrideController;
                        
                        LoadAnimation(clips[selectedIndex]);

                        yield return new WaitForSeconds(clips[selectedIndex].length / (Speed));

                        animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;

                        if (IsAnimating.Contains(obj))
                            IsAnimating.Remove(obj);
                    }
                }
            }
            yield return null;
        }

        void LoadAnimClip(AnimationClip clip)
        {
            overrideController["ElfMage_Attack3"] = clip;

            // Push back state
            for (int i = 0; i < animator.layerCount; i++)
            {
                animator.Play("OVERRIDE", i, 0);
            }
            //animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            //overrideController.a;
            // Force an update
            animator.Update(0.0f);

            //animator.ResetTrigger("StartOverride");
            //animator.SetTrigger("StartOverride");
        }

        public void LoadAnimation(AnimationClip clip)
        {
            //Save current state
            layerInfo = new AnimatorStateInfo[animator.layerCount];

            for (int i = 0; i < animator.layerCount; i++)
            {
                layerInfo[i] = animator.GetCurrentAnimatorStateInfo(i);
            }
 
            LoadAnimClip(clip);

            prevLoadAnim = "ElfMage_Attack3";
        }

        public void UnloadPreviousLoadAnimation()
        {
            for (int i = 0; i < animator.layerCount; i++)
            {
                layerInfo[i] = animator.GetCurrentAnimatorStateInfo(i);
            }

            overrideController[prevLoadAnim] = null;

            for (int i = 0; i < animator.layerCount; i++)
            {
                animator.Play(layerInfo[i].fullPathHash, i, layerInfo[i].normalizedTime);
            }

            // Force an update
            animator.Update(0.0f);
        }
    }
}


