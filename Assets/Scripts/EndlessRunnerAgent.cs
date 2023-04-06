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
    [SerializeField] bool isGrounded = false;
    [SerializeField] LayerMask ground;
    [SerializeField] float validGroundDistance = 1f;
    [SerializeField] float jumpForceMultiplier = 20f;

    Vector3 initalPosition;
    Vector3 closestObstaclePosition;
    float timeSinceJump = 0.0f;


    void Start()
    {
        body = GetComponent<Rigidbody>();
        initalPosition = transform.localPosition;
        isGrounded = false;
        timeSinceJump = 1.0f;
    }

    private void Update()
    {
        timeSinceJump += Time.deltaTime;

        if (Physics.Raycast(transform.position, Vector3.down, validGroundDistance, ground))
        {
            isGrounded = true;
        }
        else isGrounded = false;


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
        transform.localPosition = initalPosition;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(isGrounded);
        sensor.AddObservation(transform.position.y);
        sensor.AddObservation(closestObstaclePosition);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> actions = actionsOut.ContinuousActions;
        actions[0] = Input.GetAxisRaw("Vertical");
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float jumpInput = actions.ContinuousActions[0];

        if(jumpInput > 0.5f && jumpInput <= 1f && isGrounded && timeSinceJump > 0.5f)
        {
            body.AddForce(Vector3.up * jumpForceMultiplier);
            timeSinceJump = 0.0f;
            AddReward(0.1f);
        }
        else if (jumpInput > 1f)
        {
            AddReward(-2f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent(out Obstacle _obstacle))
        {
            AddReward(-30f);
            if (isGrounded) AddReward(-30f);
            spawner.Restart();
            EndEpisode();
        }
    }

    public void Reward()
    {
        AddReward(10f);
    }

    public void SetSpawner(ObstacleSpawner spawner)
    {
        this.spawner = spawner;
    }
}
