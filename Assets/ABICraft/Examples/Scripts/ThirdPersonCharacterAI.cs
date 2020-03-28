using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ThirdPersonCharacterAI : MonoBehaviour
{
    public RaycastHit hitpoint;
    static GameObject cursor;

    [Range(0, 1)]
    public float animationSpeedMultiplier;

    NavMeshAgent agent;
    Animator anim;
    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        anim  = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        cam   = Camera.main;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (anim)
        {
            anim.SetFloat("Speed", agent.velocity.magnitude);

            if(anim.GetFloat("Speed") >= 0.001f)
            {
                anim.speed = agent.velocity.magnitude * animationSpeedMultiplier;
            }
            else
            {
                anim.speed = 1;
            }
        }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hitpoint))
        {
            if (!cursor)
                cursor = GameObject.Find("Cursor");

            cursor.transform.position = hitpoint.point;

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                agent.SetDestination(hitpoint.point);
            }
        }
    }
}
