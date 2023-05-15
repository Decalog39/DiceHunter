using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [System.Serializable]
    public class LevelsOnSide
    {
        public List<GameObject> levels = new List<GameObject>();
    }

    private List<Behaviour> behaviours = new List<Behaviour>();
    private Player player;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        float timeSpawned = 0;
        float spawnTime = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().spawnTime;
        while (spawnTime > timeSpawned)
        {
            timeSpawned += Time.deltaTime;
            transform.position = Vector3.up * Mathf.SmoothStep(-4.5f, 0, timeSpawned / spawnTime);
            yield return null;
        }
        transform.position = Vector3.zero;
        foreach (Behaviour behaviour in behaviours)
        {
            behaviour.enabled = true;
        }
        player.LevelSpawned();
    }

    public void SetPlayer(Player target)
    {
        player = target;
    }

    public void Register(Behaviour behaviour)
    {
        behaviours.Add(behaviour);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
