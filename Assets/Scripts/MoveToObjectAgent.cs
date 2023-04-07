using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToObjectAgent : Agent
{
    // Reference to the target object
    [SerializeField] private Transform targetTransform;

    // Agent movement speed
    [SerializeField] private float moveSpeed = 2.5f;

    // Material to display when the agent reaches the target
    [SerializeField] Material winMat;

    // Material to display when the agent hits a wall
    [SerializeField] Material loseMat;

    [SerializeField] MeshRenderer ground;

    // The initial position of the agent
    private Vector3 initalPosition;

    private void Awake()
    {
        // Set the initial position of the agent to its current position

        initalPosition = transform.localPosition;
    }

    public override void OnEpisodeBegin()
    {
        //float agentX = Random.Range(-8f, 8f);
        //float agentZ = Random.Range(2f, 18f);
        //transform.localPosition = new Vector3(agentX, initalPosition.y, agentZ);
        transform.localPosition = initalPosition;

        //float targetX = Random.Range(-8f, 8f);
        //float targetZ = Random.Range(2f, 18f);
        //targetTransform.localPosition = new Vector3(targetX, targetTransform.localPosition.y, targetZ);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Add the position of the agent and the target to the observation 
        sensor.AddObservation(transform.position);
        sensor.AddObservation(targetTransform.position);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> actions = actionsOut.ContinuousActions;

        actions[0] = Input.GetAxisRaw("Horizontal");
        actions[1] = Input.GetAxisRaw("Vertical");
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Add the position of the agent and the target to the observation vector

        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        transform.position += new Vector3(moveX, 0f, moveZ) * Time.deltaTime * moveSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Add the position of the agent and the target to the observation vector

        GameObject go = other.gameObject;
        if (go.CompareTag("Goal")) {
            // Set a negative reward and change the material of the ground to the lose material

            SetReward(1f);
            ground.material = winMat;
            EndEpisode();
        }
        else if (go.CompareTag("Wall"))
        {
            // Set a negative reward and change the material of the ground to the lose material

            SetReward(-1f);
            ground.material = loseMat;
            EndEpisode();
        }
    }
}
