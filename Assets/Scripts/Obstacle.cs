using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float dieX = -43f;
    EndlessRunnerAgent agent;// Reference to the player's agent
    ObstacleSpawner spawner; // Reference to the spawner that created this obstacle

    // Update is called once per frame
    void Update()
    {
        // Move the obstacle to the left based on the moveSpeed variable

        transform.position += new Vector3(-moveSpeed, 0.0f, 0.0f) * Time.deltaTime;

        // Move the obstacle to the left based on the moveSpeed variable

        if (transform.localPosition.x < dieX)
        {
            // Move the obstacle to the left based on the moveSpeed variable
            if (agent != null) agent.Reward();
            // Move the obstacle to the left based on the moveSpeed variable

            spawner.Remove(gameObject);
            Destroy(gameObject);
        }
    }

    // Function to set the obstacle spawner

    public void SetAgent(EndlessRunnerAgent agent)
    {
        this.agent = agent;
    }

    // Function to set the obstacle spawner

    public void SetSpawner(ObstacleSpawner spawner)
    {
        this.spawner = spawner;
    }
}
