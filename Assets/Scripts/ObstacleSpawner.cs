using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private float interval = 5f;
    [SerializeField] private float minY, maxY; // min and max height an obstacle can spawn
    [SerializeField] private EndlessRunnerAgent agent; //the agent character
    [SerializeField] private GameObject prefab; //prefab of the obstacle
    private float elapsed = 0.0f;

    private List<GameObject> obstacles = new List<GameObject>();

    public List<GameObject> GetObstacles() => obstacles;

    // Start is called before the first frame update
    private void Start()
    {
        Restart();
        agent.SetSpawner(this);
    }

    // Update is called once per frame
    private void Update()
    {
        elapsed += Time.deltaTime;

        if (elapsed >= interval)
        {
            Vector3 position = new Vector3(0.0f, Random.Range(minY, maxY), agent.transform.localPosition.z);

            if (Input.GetMouseButton(0))
            {
                // Create a ray from the camera to the mouse position
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                // Check if the ray hits any collider
                if (Physics.Raycast(ray, out hit))
                {
                    // Get the y position of the hit point
                    float hitY = hit.point.y;

                    // If the hit point is within the valid range, use it as the spawn position
                    if (hitY >= minY && hitY <= maxY)
                    {
                        position.y = hitY;
                        position.z = agent.transform.localPosition.z;
                        position.x = 0.0f;
                    }
                }
                else
                {
                    position = new Vector3(0.0f, Random.Range(minY, maxY), agent.transform.localPosition.z);
                    position.z = agent.transform.localPosition.z;
                    position.x = 0.0f;
                }

                Debug.Log("sdwsdw");
            }
            else
            {
                // If the user doesn't click, randomly choose the y position
                position = new Vector3(0.0f, Random.Range(minY, maxY), agent.transform.localPosition.z);
            }

            GameObject GO = Instantiate(prefab, transform);
            Obstacle obstacle = GO.GetComponent<Obstacle>();
            obstacle.SetAgent(agent);
            obstacle.SetSpawner(this);
            GO.transform.localPosition = position;//set obstacle positon
            obstacles.Add(GO); //add to list of obstacles
            elapsed = 0.0f;
        }
    }

    public void Restart()
    {
        elapsed = 0.0f;
        for (int i = 0; i < obstacles.Count; i++)
        {
            Destroy(obstacles[i]);
        }
        obstacles.Clear();//clear the obstacles list
    }

    public void Remove(GameObject go)
    {
        obstacles.Remove(go);//remove the obstacle
    }
}