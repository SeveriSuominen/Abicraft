using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

using XNode;

namespace AbicraftNodes.Action
{
    public class AnimationNode : AbicraftExecutionNode
    {
        public static uint id = 111;

        public static bool IsAnimating;

        /*[Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public string AnimationTrigger;*/
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AbicraftObject Obj;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public float  Speed;

        //Wanna pass arguments for editor? Use non-seriziable sturct to hold that data
        //and access it in custom editor via Target.
        public NodeData data;

        public class NodeData
        {
            public int selectedIndex = 0;
            public AnimationClip[] clips;
        }
        //-----------------------------------------------------------------------------

        AnimatorOverrideController overrideController;
        string prevLoadAnim;
        Animator animator;
        ResourceRequest request;
        AnimatorStateInfo[] layerInfo;

        public override void Initialize(AbicraftAbilityExecution.AbicraftNodeExecution execution)
        {
            //execution.Block();
        }

        public override IEnumerator ExecuteNode(AbicraftAbilityExecution.AbicraftNodeExecution execution)
        {
            Obj = GetInputValue<AbicraftObject>("Obj");
            Obj.GetComponent<NavMeshAgent>().ResetPath();

            if (Obj)
            {
                animator = Obj.GetComponent<Animator>();

                if (animator != null)
                {
                    if (IsAnimating)
                        animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;

                    if (animator.runtimeAnimatorController.GetType() != typeof(AnimatorOverrideController))
                    {
                        IsAnimating = true;
                        animator.speed = Speed;

                        overrideController = new AnimatorOverrideController();

                        if (!overrideController.runtimeAnimatorController)
                            overrideController.runtimeAnimatorController = animator.runtimeAnimatorController;

                        animator.runtimeAnimatorController = overrideController;
                        
                        LoadAnimation(data.clips[data.selectedIndex]);

                        yield return new WaitForSeconds(data.clips[data.selectedIndex].length / (Speed));

                        animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
                        IsAnimating = false;
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
                animator.Play(layerInfo[i].fullPathHash, i, layerInfo[i].normalizedTime);
            }
            //animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            //overrideController.a;
            // Force an update
            animator.Update(0.0f);

            animator.ResetTrigger("StartOverride");
            animator.SetTrigger("StartOverride");
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


