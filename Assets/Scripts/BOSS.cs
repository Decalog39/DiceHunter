using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class BOSS : Enemy
{
    private Transform target;

 public float baseSpeed, baseAcceleration, chargingSpeed, chargingAcceleration, chargeDelay, chargeCooldown, chargeRange, overshootDistance;

    private NavMeshAgent agent;
     public float attackCD;
    private float cd;
    public int numOfBullet;
    private Vector3 temporal;
    public GameObject bullet;
    public float bulletSpeed;
    private bool charging = false;

    void Start()
    {
        OnStart(this);
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        agent.speed = 1;
        agent.acceleration = 1;
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
        cd -= 1*Time.deltaTime;
       if(cd <= 0){
            StartCoroutine("Firing1");
            StartCoroutine("Firing2");
            StartCoroutine("Firing3");
            StartCoroutine("Firing4");
            cd = attackCD;
       }



    }

    float GetDistance(){
        return Vector3.Distance(transform.position, target.position);
    }

     IEnumerator Firing1(){
        for(int i = 0; i < numOfBullet; i++){
            temporal = transform.position;
            // Changed the height so that bullets spawn inside of the enemy, not on their head. Also adjusted it so they spawn a bit forward.
            temporal.y += 0.5f;
            temporal += transform.forward * 0.4f;
            GameObject attack = Instantiate(bullet, temporal, Quaternion.identity);
            attack.GetComponent<Rigidbody>().velocity = bulletSpeed * (target.position - transform.position).normalized;
            yield return new WaitForSeconds(0.75f);
            Destroy(attack, 5);
        }
    }
    IEnumerator Firing2(){
        for(int i = 0; i < numOfBullet; i++){
            temporal = transform.position;
            // Changed the height so that bullets spawn inside of the enemy, not on their head. Also adjusted it so they spawn a bit forward.
            temporal.y += 0.5f;
            temporal += transform.forward * 0.4f;
            GameObject attack = Instantiate(bullet, temporal, Quaternion.identity);
            attack.GetComponent<Rigidbody>().velocity = bulletSpeed * (Quaternion.AngleAxis(90, Vector3.up)*(target.position - transform.position).normalized);
            yield return new WaitForSeconds(0.75f);
            Destroy(attack, 5);
        }
    }
    IEnumerator Firing3(){
        for(int i = 0; i < numOfBullet; i++){
            temporal = transform.position;
            // Changed the height so that bullets spawn inside of the enemy, not on their head. Also adjusted it so they spawn a bit forward.
            temporal.y += 0.5f;
            temporal += transform.forward * 0.4f;
            GameObject attack = Instantiate(bullet, temporal, Quaternion.identity);
             attack.GetComponent<Rigidbody>().velocity = bulletSpeed * (Quaternion.AngleAxis(180, Vector3.up)*(target.position - transform.position).normalized);
            yield return new WaitForSeconds(0.75f);
            Destroy(attack, 5);
        }
    }
    IEnumerator Firing4(){
        for(int i = 0; i < numOfBullet; i++){
            temporal = transform.position;
            // Changed the height so that bullets spawn inside of the enemy, not on their head. Also adjusted it so they spawn a bit forward.
            temporal.y += 0.5f;
            temporal += transform.forward * 0.4f;
            GameObject attack = Instantiate(bullet, temporal, Quaternion.identity);
            attack.GetComponent<Rigidbody>().velocity = bulletSpeed * (Quaternion.AngleAxis(270, Vector3.up)*(target.position - transform.position).normalized);
            yield return new WaitForSeconds(0.75f);
            Destroy(attack, 5);
        }
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

    private void OnCollisionEnter(Collision collision)
    {
        Collision(collision);
    }

    private void OnDestroy()
    {
        GameObject temp = GameObject.Find("LoseText");
        temp.GetComponent<TMP_Text>().text = "Congratulations, you won!";
        temp.GetComponent<TMP_Text>().enabled = true;
    }
}
