using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mob2Movement : Enemy
{

    public float baseSpeed, baseAcceleration, kiteRange, overshootDistance;

    private Transform target;
    private NavMeshAgent agent;

    public float attackCD;
    private float cd;

    private Vector3 temporal;

 [Header("Attack")]
    public GameObject bullet;
    public float bulletSpeed;

    public int numOfBullet;
    // Start is called before the first frame update
    void Start()
    {
        OnStart(this);
        cd = attackCD;
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        agent.speed = baseSpeed;
        agent.acceleration = baseAcceleration;
    }

    // Update is called once per frame
    void Update()
    {
       float distance = Vector3.Distance(transform.position, target.position);
       if(distance < kiteRange){
        baseSpeed = 4;
        baseAcceleration = 10;
        agent.speed = baseSpeed;
        agent.acceleration = baseAcceleration;
        agent.SetDestination((transform.position - target.position).normalized * overshootDistance);
       } else {
        baseSpeed = 2;
        baseAcceleration = 2;
        agent.speed = baseSpeed;
        agent.acceleration = baseAcceleration;
        agent.SetDestination((target.position - transform.position));
       }
       
       cd -= 1*Time.deltaTime;
       if(cd <= 0){
            StartCoroutine("Firing");
            cd = attackCD;
       }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Collision(collision);
    }
    

    IEnumerator Firing(){
        for(int i = 0; i < numOfBullet; i++){
            temporal = transform.position;
            // Changed the height so that bullets spawn inside of the enemy, not on their head. Also adjusted it so they spawn a bit forward.
            temporal.y += 0.5f;
            temporal += transform.forward * 0.4f;
            GameObject attack = Instantiate(bullet, temporal, Quaternion.identity);
            attack.GetComponent<Rigidbody>().velocity = bulletSpeed * (target.position - transform.position).normalized;
            yield return new WaitForSeconds(0.75f);
            Destroy(attack, 3);
        }
    }

}
