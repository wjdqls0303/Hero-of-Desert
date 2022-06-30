using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float damage = 10f;
    public float delay = 0;
    public float maxDelay = 1.5f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            delay = 0;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            if (delay > 0)
            {
                delay -= Time.deltaTime;
            }
            else
            {
                other.transform.GetComponent<PlayerMove>().TakeDamage(damage);
                delay = maxDelay;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            delay = maxDelay;
        }
    }
}
