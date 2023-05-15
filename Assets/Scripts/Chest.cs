using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public float healthGained, damageIncrease, spreadReductionRatio, delayReductionRatio, moveSpeedIncrease;
    public int bulletIncrease;

    public void Interact()
    {
        if (!GetComponent<Animator>().enabled)
        {
            Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            player.health += healthGained;
            player.moveSpeed += moveSpeedIncrease;
            player.damage += damageIncrease;
            player.bullets += bulletIncrease;
            player.spread *= (1 - spreadReductionRatio);
            player.cooldownTime *= (1 - delayReductionRatio);
            GetComponent<Animator>().enabled = true;
            transform.GetChild(0).LookAt(Camera.main.transform.position);
            transform.GetChild(0).Rotate(Vector3.up * 180);
            player.EnemyDestroyed();
        }
    }
}
