using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CombatEnemy : MonoBehaviour
{
    [Header("Atributtes")] 
    public float totalHealth;
    public float attackDamage;
    public float moveSpeed;
    public float LookRadius;
    public float colliderRadius = 2f;

    [Header("Components")] 
    private Animator anim;
    private CapsuleCollider capsule;
    private NavMeshAgent agent;
    private bool Running;
    private bool Attacking;
    private bool waitFor;
    private bool hit;

    [Header("Others")] 
    private Transform player;
    void Start()
    {
        anim = GetComponent<Animator>();
        capsule = GetComponent<CapsuleCollider>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("player").transform;
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        if (distance <= LookRadius)
        {
            
            // personagem no raio de visão
            agent.isStopped = false;
            if (!Attacking)
            {
                agent.SetDestination(player.position);
                anim.SetBool("Run Forward", true);
                Running = true;
            }
            if( distance <= agent.stoppingDistance)
            {
                StartCoroutine("Attack");
                agent.isStopped = true;
            }
            else
            {
                Attacking = false;
            }
            
        }
        else
        {   //personagem quando não está no raio de visão
            anim.SetBool("Run Forward", false);
            agent.isStopped = true;
            Running = false;
            Attacking = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, LookRadius);
    }
    IEnumerator Attack()
    {
        if (!waitFor)
        {
            waitFor = true;
            Attacking = true;
            Running = false;
            anim.SetBool("Run Forward", false);
            anim.SetBool("Bite Attack", true);
            yield return new WaitForSeconds(1.1f);
            GetPlayer();
            //yield return new WaitForSeconds(1f);
            waitFor = false;
        }
        
    }

    void GetPlayer()
    { 
        foreach (Collider c in Physics.OverlapSphere((transform.position + transform.forward * colliderRadius), colliderRadius))
        {
            if (c.gameObject.CompareTag("player"))
            {
                Debug.Log("bateu");
            }
        }
        
    }

    public void getHit(float Damage)
    {
        totalHealth -= Damage;
        if (totalHealth > 0)
        {
            //vivo
            StopCoroutine("Attack");
            anim.SetTrigger("Take Damage");
            hit = true;
        }
        else
        {
            anim.SetTrigger("Die");
            //morto
        }
    }

    IEnumerator RecoveryFromHit()
    {
        yield return new WaitForSeconds(1f);
        anim.SetBool("Run Forward", false);
        anim.SetBool("Bite Attack", false);
        hit = false;
        waitFor = false;
    }
}
