using AbicraftCore;
using AbicraftMonos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbicraftMonos
{
    public class AbicraftGUI : MonoBehaviour
    {
        public static AbicraftGUI Instance { get; private set; }

        public ObjectImpactGUIObject useObjectImpactGUIObject;

        public Canvas canvas { get; private set; }

        void Start()
        {
            Instance = this;
            StartCoroutine(Setup());
        }

        IEnumerator Setup()
        {
            while (AbicraftGlobalContext.abicraft == null)
            {
                yield return new WaitForEndOfFrame();
            }

            if (!canvas)
            {
                var gobj = Instantiate(AbicraftGlobalContext.abicraft.dataFile.AbicraftCanvasPrefab.gameObject);
                canvas = gobj.GetComponent<Canvas>();
            }
        }

        public void SpawnObjectImpact(AbicraftObject target, AbicraftAttribute attribute, int amount)
        {
            SpawnObjectImpact(target, attribute.AttributeIcon, (amount < 0 ? "" : "+ ") + amount, Color.grey);
        }

        public void SpawnObjectImpact(AbicraftObject target, AbicraftState state, bool showStateName = true)
        {
            Color color = Color.white;

            switch (state.type)
            {
                case AbicraftState.StateType.Neutral:
                    color = Color.yellow;
                    break;
                case AbicraftState.StateType.Negative:
                    color = Color.red;
                    break;
                case AbicraftState.StateType.Positive:
                    color = Color.green;
                    break;
            }

            SpawnObjectImpact(target, state.icon, showStateName ? state.StateName : string.Empty, color);
        }

        void SpawnObjectImpact(AbicraftObject target, Texture2D icon, string impactString, Color color)
        {
            var impactGUIAbj = useObjectImpactGUIObject.GetComponent<AbicraftObject>();

            if (!impactGUIAbj)
            {
                Debug.LogError("Abicraft: ObjectImpactGUIObject is required have AbicraftObject component attached");
                return;
            }

            var abj = AbicraftObjectPool.Spawn(impactGUIAbj, canvas.transform);
            var impactGUIAbjComponent = abj.GetComponent<ObjectImpactGUIObject>();

            impactGUIAbjComponent.targetAbj = target;
            impactGUIAbjComponent.impactString = impactString;
            impactGUIAbjComponent.icon = icon;
            impactGUIAbjComponent.color = color;

            impactGUIAbjComponent.StartActionMono(impactGUIAbjComponent.secondsUntilFullFaded, true);
        }
    }
}


