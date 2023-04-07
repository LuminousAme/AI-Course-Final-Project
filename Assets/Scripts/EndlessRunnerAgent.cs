using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class EndlessRunnerAgent : Agent
{
    Rigidbody body;
    ObstacleSpawner spawner;

    // Ground detection variables
    [SerializeField] bool isGrounded = false;   // Whether the agent is currently on the ground
    [SerializeField] LayerMask ground;          // The ground layer
    [SerializeField] float validGroundDistance = 1f;   // The distance from the ground at which the agent is considered grounded

    [SerializeField] float jumpForceMultiplier = 20f;

    //audio sources 
    [SerializeField] private AudioSource deathAudioSource;
    [SerializeField] private AudioSource jumpAudioSource;
    [SerializeField] private AudioSource musicAudioSource;

    Vector3 initalPosition;  // The initial position of the agent
    Vector3 closestObstaclePosition;// The position of the closest obstacle to the agent
    float timeSinceJump = 0.0f;     // The amount of time since the agent last jumped


    void Start()
    {
        body = GetComponent<Rigidbody>();
        // Save the agent's initial position and set grounded status and time since jump
        initalPosition = transform.localPosition;
        isGrounded = false;
        timeSinceJump = 1.0f;
    }

    private void Update()
    {
        // Increment time since jump
        timeSinceJump += Time.deltaTime;

        // Raycast downwards to check if the agent is on the ground
        if (Physics.Raycast(transform.position, Vector3.down, validGroundDistance, ground))
        {
            isGrounded = true;
        }
        else isGrounded = false;


        // Get a list of all obstacles in the scene and find the closest one to the agent
        List<GameObject> obstacles = spawner.GetObstacles();
        if (obstacles.Count < 0) return;

        float xdistance = float.MaxValue;
        for(int i = 0; i < obstacles.Count; i++)
        {
            Vector3 obstaclex = new Vector3(obstacles[i].transform.position.x, 0f, 0f);
            Vector3 thisx = new Vector3(transform.position.x, 0f, 0f);

            float distance = Vector3.Distance(obstaclex, thisx);

            if(distance < xdistance)
            {
                xdistance = distance;
                closestObstaclePosition = obstacles[i].transform.localPosition;
            }
        }
    }

    public override void OnEpisodeBegin()
    {
        // Reset the agent's position to its initial position
        transform.localPosition = initalPosition;
        musicAudioSource.Play();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Add the agent's grounded status, y position, and position of the closest obstacle to the sensor
        sensor.AddObservation(isGrounded);
        sensor.AddObservation(transform.position.y);
        sensor.AddObservation(closestObstaclePosition);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Set the continuous action to the vertical input axis
        ActionSegment<float> actions = actionsOut.ContinuousActions;
        actions[0] = Input.GetAxisRaw("Vertical");
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Get the continuous action input for jumping
        float jumpInput = actions.ContinuousActions[0];

        // Check if the jump input is within a valid range and the agent is on the ground and has not jumped recently
        if (jumpInput > 0.5f && jumpInput <= 1f && isGrounded && timeSinceJump > 0.5f)
        {
            // Apply force to the rigidbody to make the agent jump
            body.AddForce(Vector3.up * jumpForceMultiplier);
            timeSinceJump = 0.0f;
            // Add a positive reward for successful jump
            AddReward(0.1f);
            // Play sound effect
            jumpAudioSource.Play();

        }
        // If the jump input is not within the valid range, negative reward
        else if (jumpInput > 1f)
        {
            AddReward(-2f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent(out Obstacle _obstacle))
        {
            // Play sound effect
            deathAudioSource.Play();
            // negative rewards for hitting an obstacle
            AddReward(-30f);
            // extra negative rewards for hitting an obstacle while on the ground
            if (isGrounded) AddReward(-30f);
            // Restart the obstacle spawner and end the episode
            spawner.Restart();
            EndEpisode();

       
        }
    }

    public void Reward()
    {
        // Add a positive reward
        AddReward(10f);
    }

    public void SetSpawner(ObstacleSpawner spawner)
    {
        // Set the obstacle spawner
        this.spawner = spawner;
    }
}
