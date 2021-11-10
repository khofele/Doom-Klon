using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private float health = 60f;

    public void TakeDamage(float damage)
    {
        if(health > 0)
        {
            health -= damage;
            GetComponent<Animator>().Play("Zombie Reaction Hit"); ;
        }
        else
        {
            GetComponent<Animator>().Play("Falling Back Death");
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        int time = 0;

        while (time < 1000)
        {
            time += 1;
            yield return null;
        }

        Destroy(gameObject);
    }
}
