using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mob4Movement : Enemy
{
    public float projectileVelocity, fireDelay;
    public GameObject projectile;

    private float timeUntilFire;
    private Transform target;
    private NavMeshAgent agent;

    void Start()
    {
        timeUntilFire = fireDelay;
        OnStart(this);
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    
    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(target.position);

        timeUntilFire -= Time.deltaTime;
        if (timeUntilFire < 0)
        {
            timeUntilFire += fireDelay;
            Transform temp = Instantiate(projectile, transform.position + Vector3.up * 0.5f, Quaternion.identity).transform;
            temp.LookAt(target);
            float v2 = projectileVelocity * projectileVelocity;
            float gravity = -Physics.gravity.y;
            float range = Vector3.Distance(target.position, temp.position);
            float numerator = v2 + Mathf.Sqrt(v2 * v2 - gravity * (gravity * range * range));
            float denominator = gravity * range;
            float angle = Mathf.Atan(numerator / denominator) * Mathf.Rad2Deg;
            temp.Rotate(Vector3.left * angle);
            temp.GetComponent<Rigidbody>().velocity = temp.forward * projectileVelocity;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Collision(collision);
    }
}
