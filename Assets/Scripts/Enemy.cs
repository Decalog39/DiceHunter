using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float health;
    public float contactDamage;

    public void OnStart(Behaviour enemyBehaviour)
    {
        transform.parent.GetComponent<Level>().Register(this);
        transform.parent.GetComponent<Level>().Register(GetComponent<NavMeshAgent>());
        enemyBehaviour.enabled = false;
    }

    public void Damage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().EnemyDestroyed();
            Destroy(gameObject);
        }
    }

    public void Collision(Collision collision)
    {
        if (collision.collider.tag == "Projectile")
        {
            Damage(collision.collider.GetComponent<Bullet>().damage);
            Destroy(collision.collider.gameObject);
        }
        else if (collision.collider.tag == "Player")
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().Damage(contactDamage);
        }
    }
}
