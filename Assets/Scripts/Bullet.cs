using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public float explosionRadius;


    void OnDestroy()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (explosionRadius > 0.05f)
        {
            Transform particles = transform.GetChild(0);
            particles.GetComponent<ParticleSystem>().Play();
            particles.GetComponent<AudioSource>().Play();
            particles.parent = null;
            Destroy(particles.gameObject, 1f);

            Transform player = GameObject.FindGameObjectWithTag("Player").transform;
            player.GetComponent<Player>().Damage(Mathf.Lerp(damage, 0, Mathf.Clamp01(Vector3.Distance(transform.position, player.position) / explosionRadius)));
        }
        if (collision.collider.tag == "Player")
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().Damage(damage);
            Destroy(gameObject);
        }
        if (collision.collider.tag == "Terrain")
        {
            Destroy(gameObject);
        } else if (collision.collider.tag == "Interactible")
        {
            collision.collider.GetComponent<Chest>().Interact();
            Destroy(gameObject);
        }
    }
}
