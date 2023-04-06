using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float dieX = -43f;
    EndlessRunnerAgent agent;
    ObstacleSpawner spawner;

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(-moveSpeed, 0.0f, 0.0f) * Time.deltaTime;

        if(transform.localPosition.x < dieX)
        {
            if (agent != null) agent.Reward();
            spawner.Remove(gameObject);
            Destroy(gameObject);
        }
    }

    public void SetAgent(EndlessRunnerAgent agent)
    {
        this.agent = agent;
    }

    public void SetSpawner(ObstacleSpawner spawner)
    {
        this.spawner = spawner;
    }
}
