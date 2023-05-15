using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mob3Movement : Enemy
{
    private Transform target;
    private NavMeshAgent agent;

    void Start()
    {
        OnStart(this);
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    
    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(target.position);

    }

    private void OnCollisionEnter(Collision collision)
    {
        Collision(collision);
    }
}
