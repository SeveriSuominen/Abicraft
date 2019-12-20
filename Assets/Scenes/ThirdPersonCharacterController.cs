using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ThirdPersonCharacterController : MonoBehaviour
{
    public RaycastHit hitpoint;
    public Transform  pivot;
    public GameObject cursor;

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
    void Update()
    {
        if (anim)
        {
            anim.SetFloat("Speed", agent.velocity.magnitude);

            if(anim.GetFloat("Speed") >= 0.001f)
            {
                anim.speed = agent.velocity.magnitude * 0.25f;
            }
            else
            {
                anim.speed = 2;
            }
        }

        if (Vector3.Distance(
            new Vector3(transform.position.x, 0, transform.position.z),
            new Vector3(pivot.transform.position.x, 0, pivot.transform.position.z))
            >= 0.33f
        )
        {
            pivot.transform.position = Vector3.Lerp(pivot.transform.position, transform.position, 0.05f);
        }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hitpoint))
        {
            cursor.transform.position = hitpoint.point;

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                agent.SetDestination(hitpoint.point);
            }
        }
    }
}
