using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mob1Movement : Enemy
{
    // Start is called before the first frame update

    public float baseSpeed, baseAcceleration, chargingSpeed, chargingAcceleration, chargeDelay, chargeCooldown, chargeRange, overshootDistance;

    private Transform target;
    private NavMeshAgent agent;

   
    private bool charging = false;
    void Start()
    {
        OnStart(this);
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Collision(collision);
    }


    // Update is called once per frame
    void Update()
    {   
        if(!charging){
            if(GetDistance() > chargeRange){
                agent.SetDestination(target.position);
            } else {
                StartCoroutine("Charge");
            }   
        }
        
    }

    float GetDistance(){
        return Vector3.Distance(transform.position, target.position);
    }

    IEnumerator Charge(){
        charging = true;
        agent.speed = 0;
        Vector3 oldPlayerPosition = target.position;
        agent.SetDestination((target.position - transform.position).normalized * overshootDistance + target.position);
        
        yield return new WaitForSeconds(chargeDelay);
        agent.speed = chargingSpeed;
        agent.acceleration = chargingAcceleration;
        float distance0 = 10000f;
        while(true){
            yield return null;
            float distance1 = Vector3.Distance(transform.position, oldPlayerPosition);
            if(distance1 > distance0 || Vector3.Distance(transform.position, agent.destination) < 0.5f){
                break;
            }
            distance0 = distance1;
        }
        yield return new WaitForSeconds(chargeCooldown);
        agent.speed = baseSpeed;
        agent.acceleration = baseAcceleration;
        charging = false;
    }
}
