using AbicraftCore;
using AbicraftMonos;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityBar : MonoBehaviour
{
    public AbicraftObject player;
    public GameObject abilityInfoTemplate;

    public float Spacing;

    public List<DispatchAbility> abilities;

    readonly List<AbilityInfo> abilityInfos = new List<AbilityInfo>();

    AbicraftAbilityDispatcher dispatcher;

    [System.Serializable]
    public class DispatchAbility
    {
        public AbicraftAbility ability;
        public KeyCode key;
    }

    void Start()
    {
        RectTransform rect = GetComponent<RectTransform>();

        for (int i = 0; i < abilities.Count; i++)
        {
            GameObject obj = Instantiate<GameObject>(abilityInfoTemplate, this.transform);
            obj.transform.localPosition = new Vector2( 5 + Spacing * i, 0);

            AbilityInfo info = obj.GetComponent<AbilityInfo>();
            info.ability = abilities[i].ability;

            abilityInfos.Add(info);
        }

        //rect.position = new Vector2(rect.position.x  + ((abilities.Count) * Spacing), rect.position.y);
        rect.offsetMax = new Vector2(rect.offsetMax.x + ((abilities.Count - 1) * Spacing) + 10, rect.offsetMax.y);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!dispatcher)
            dispatcher = AbicraftGlobalContext.abicraft.dispatcher;

        for (int i = 0; i < abilityInfos.Count; i++)
        {
            AbilityInfo info = abilityInfos[i];
            if(info.ability.icon)
                info.icon.texture = info.ability.icon;

            if (dispatcher)
            {
                List<AbicraftAbilityExecution> executions = dispatcher.GetActiveExecutionsBySenderObject(info.ability, player);

                if(executions.Count > 0)
                {
                    AbicraftAbilityExecution latestExecution = executions[executions.Count - 1];

                    if (latestExecution.OnCooldown())
                    {
                        info.cooldownFiller.gameObject.SetActive(true);
                        info.timerText.gameObject.SetActive(true);

                        float clampCooldown = Mathf.Clamp(latestExecution.elapsedCooldown / info.ability.Cooldown, 0, 1);

                        info.cooldownFiller.fillAmount = 1 - clampCooldown;
                        info.timerText.text = String.Format("{0:0.0}", latestExecution.GetCooldownLeft());

                        continue;
                    }
                }
            }

            info.cooldownFiller.gameObject.SetActive(false);
            info.timerText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            if (dispatcher && Input.GetKeyDown(abilities[i].key))
            {
                dispatcher.Dispatch(player, abilities[i].ability);
            }
        }
    }
}
