using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Target : MonoBehaviour
{
    [SerializeField] private float health = 60f;
    [SerializeField] private float lookWeight = 0;
    [SerializeField] private float watchDistance = 8f;
    [SerializeField] private float attackDistance = 3.5f;

    private float distance = 0;
    private GameObject player = null;
    private Animator animator = null;
    private NavMeshAgent agent = null;
    private IEnumerator followCoroutine = null;
    private IEnumerator attackCoroutine = null;
    private float damage = 0.025f;
    private bool enemyDead = false;
    private bool attackAllowed = false;
    private bool followAllowed = false;
    private int counter = 0;
    private int hitCounter = 0;
    private bool allowedToCountUp = true;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        followCoroutine = FollowPlayer();
        attackCoroutine = AttackPlayer();
    }

    private void Update()
    {
        distance = Vector3.Distance(player.transform.position, gameObject.transform.position);

        RaycastHit hit;
        if (distance <= watchDistance && enemyDead == false)
        {
            followAllowed = true;
            StopCoroutine(attackCoroutine);
            animator.SetBool("Walking", true);
            StartCoroutine(followCoroutine);
        }


        if (Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out hit, watchDistance))
        {
            Collider collider = hit.collider;

            if (collider.gameObject == player && enemyDead == false)
            {
                if (distance <= attackDistance)
                {
                    attackAllowed = true;
                    StopCoroutine(followCoroutine);
                    animator.SetBool("Attack", true);
                    StartCoroutine(AttackPlayer());
                }
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if(health > 0)
        {
            health -= damage;
            counter += 1;
            StopCoroutine(attackCoroutine);
            StopCoroutine(followCoroutine);
            if(counter%5 == 0)
            {
                animator.SetTrigger("HitTrigger");
            }
        }
        else
        {
            if(allowedToCountUp == true)
            {
                UI.ZombieCounter += 1;
                allowedToCountUp = false;
            }
            enemyDead = true;
            StopCoroutine(attackCoroutine);
            StopCoroutine(followCoroutine);

            animator.SetBool("Dead", true);
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Zombie Death"))
        {
            yield return new WaitForSeconds(3f);
            Destroy(gameObject);
        }

    }

    private void OnAnimatorIK()
    {
        if (distance < watchDistance && enemyDead == false)
        {
            animator.SetLookAtPosition(player.transform.position);
            animator.SetLookAtWeight(lookWeight);
        }
    }

    private IEnumerator FollowPlayer()
    {
        while (gameObject.activeSelf)
        {
            if (agent.enabled && distance > attackDistance)
            {
                if (agent.CalculatePath(player.transform.position, agent.path) == false)
                {
                    agent.ResetPath();
                    agent.SetDestination(player.transform.position);
                }
                else
                {
                    agent.SetDestination(player.transform.position);
                }

            }
            else if(agent.enabled && distance <= attackDistance +1)
            {
                yield return null;
            }

            else 
            {
                animator.SetBool("Walking", false);
                yield break;
            }
            yield return null;
        }
        StopCoroutine(followCoroutine);
    }

    private IEnumerator AttackPlayer()
    {
        while (gameObject.activeSelf)
        {
            if (agent.enabled && distance <= attackDistance)
            {
                hitCounter += 1;
                if(hitCounter%50 == 0 && this.animator.GetCurrentAnimatorStateInfo(0).IsName("Zombie Attack"))
                {
                    player.GetComponent<CharController>().TakeDamage(damage);
                }
            }
            else
            {
                animator.SetBool("Attack", false);
            }
            yield return null;
        }
        StopCoroutine(attackCoroutine);
    }
}
