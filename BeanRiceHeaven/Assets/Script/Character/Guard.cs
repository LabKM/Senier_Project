using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Guard : LivingEntity
{
    BeanController target;
    NavMeshAgent m_Agent;
    Behavior action;
    Animator animator;

    float last_configured_to_action = 0;
    public float TracePlayerPerMin = 20;
    //public float WaitTimeToAction = 1f;
    //public float AttackAnimationTime = 0.1f;
    float attackDistanceThreshold = 0.5f;

    public void Spawn(Vector3 pos)
    {
        transform.position = pos;
        transform.rotation = Quaternion.identity;
        transform.gameObject.SetActive(true);
        Spawn();
    }

    public void Spawn(){
        m_Agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        target = GameObject.FindWithTag("Player").GetComponent<BeanController>();
        action = new Behavior(trace);
        m_Agent.destination = target.transform.position;
    }

    void trace()
    {
        if ((transform.position - target.transform.position).magnitude < attackDistanceThreshold)
        {
            action = new Behavior(attack);
            last_configured_to_action = Time.time;
            m_Agent.isStopped = true;

            animator.SetBool("Run", false);
            animator.SetBool("Walk", false);
        }
        else if (last_configured_to_action + (60 / TracePlayerPerMin) < Time.time)
        {
            last_configured_to_action = Time.time;
            m_Agent.destination = target.transform.position;
            
            animator.SetBool("Walk", true);
            animator.SetBool("Run", true);
        }
    }

    void attack(){
         if ((transform.position - target.transform.position).magnitude > attackDistanceThreshold)
        {
            action = new Behavior(trace);
            last_configured_to_action = Time.time;
            m_Agent.isStopped = false;
            m_Agent.destination = target.transform.position;
        }
    }

    public override void Start()
    {
        base.Start();
        Spawn();
    }

    void Update()
    {
        action();
    }
}
