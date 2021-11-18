using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float lookWeight = 0;

    private float watchDistance = 5f;
    private float attackDistance = 1f;
    private float distance = 0;
    private GameObject player = null;
    private Animator animator = null;
    private NavMeshAgent agent = null;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        distance = Vector3.Distance(player.transform.position, gameObject.transform.position);

        RaycastHit hit;
        if(Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out hit, watchDistance))
        {
            Collider collider = hit.collider;

            // check player
            if(collider.gameObject == player)
            {
                if (distance < attackDistance)
                {
                    StopAllCoroutines();
                    // attack
                    StartCoroutine(AttackPlayer());
                }
                else
                {
                    StopAllCoroutines();
                    StartCoroutine(FollowPlayer());
                }
            }
        }
    }

    private void OnAnimatorIK()
    {
        if(distance < watchDistance)
        {
            animator.SetLookAtPosition(player.transform.position);
            animator.SetLookAtWeight(lookWeight);
        }
    }

    private IEnumerator FollowPlayer()
    {
        while(gameObject.activeSelf && distance > attackDistance)
        {
            if(agent.enabled)
            {
                agent.SetDestination(player.transform.position);
            }
            yield return null;
        }
    }

    private IEnumerator AttackPlayer()
    {
        while(gameObject.activeSelf && distance < attackDistance)
        {
            if(agent.enabled)
            {
                CharController.Health -= 10f;
                Debug.Log(CharController.Health);
            }
            yield return null;
        }
    }
}
