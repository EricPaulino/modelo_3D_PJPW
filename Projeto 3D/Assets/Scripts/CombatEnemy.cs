using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;


public class CombatEnemy : MonoBehaviour
{
    [Header("Atributtes")] 
    public float totalHealth = 75;
    public float attackDamage;
    public float moveSpeed;
    public float LookRadius;
    public float colliderRadius = 2f;
    public float rotationSpeed;

    [Header("Components")] 
    private Animator anim;
    private CapsuleCollider capsule;
    private NavMeshAgent agent;
    private bool Running;
    private bool Attacking;
    private bool waitFor;
    private bool hit;
    public bool PLayerisCompletelyDead;

    [Header("Others")] 
    private Transform player;

    [Header("WayPoints")] public List<Transform> WayPoints = new List<Transform>();
    public int CPIndex;
    public float PathDistance;
    
    void Start()
    {
        anim = GetComponent<Animator>();
        capsule = GetComponent<CapsuleCollider>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("player").transform;
    }

    void Update()
    {
        if (totalHealth > 0)
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

                if (distance <= agent.stoppingDistance)
                {
                    StartCoroutine("Attack");
                    agent.isStopped = true;
                    LookTarget();
                }
                else
                {
                    Attacking = false;
                }

            }
            else
            {
                //personagem quando não está no raio de visão
                //agent.isStopped = true;
                anim.SetBool("Run Forward", false);
                Running = false;
                Attacking = false;
                MoveToWayPoint();
            }
        }
    }

    void MoveToWayPoint()
    {
        if (WayPoints.Count > 0 )
        {
            float distance = Vector3.Distance(WayPoints[CPIndex].position, transform.position);
            agent.destination = WayPoints[CPIndex].position;
            
            if (distance <= PathDistance)
            {
                //proximo ponto
                //CPIndex = Random.Range(0, WayPoints.Count);
                //falta ajeitar essa parte 
            }
            anim.SetBool("Run Forward", true);
            Running = true;
            
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, LookRadius);
    }
    IEnumerator Attack()
    {
        if (!waitFor && !hit && !PLayerisCompletelyDead)
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

        if (PLayerisCompletelyDead)
        {
            anim.SetBool("Run Forward", false);
            anim.SetBool("Bite Attack",  false);
            Running = false;
            Attacking = false;
            agent.isStopped = true;
        }
        
    }

    void GetPlayer()
    { 
        foreach (Collider c in Physics.OverlapSphere((transform.position + transform.forward * colliderRadius), colliderRadius))
        {
            if (c.gameObject.CompareTag("player"))
            {
                c.gameObject.GetComponent<player>().getHit(attackDamage);
                PLayerisCompletelyDead = c.gameObject.GetComponent<player>().isDead;
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
            StartCoroutine("RecoveryFromHit");
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

    void LookTarget()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion LookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, LookRotation, Time.deltaTime * rotationSpeed);
    }
}
