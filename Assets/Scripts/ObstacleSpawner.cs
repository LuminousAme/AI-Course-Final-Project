using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] float interval = 5f;
    [SerializeField] float minY, maxY;
    [SerializeField] EndlessRunnerAgent agent;
    [SerializeField] GameObject prefab;
    float elapsed = 0.0f;

    List<GameObject> obstacles = new List<GameObject>();
    public List<GameObject> GetObstacles() => obstacles;

    // Start is called before the first frame update
    void Start()
    {
        Restart();
        agent.SetSpawner(this);
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;

        if(elapsed >= interval)
        {
            GameObject GO = Instantiate(prefab, transform);
            Obstacle obstacle = GO.GetComponent<Obstacle>();
            obstacle.SetAgent(agent);
            obstacle.SetSpawner(this);
            GO.transform.localPosition = new Vector3(0.0f, Random.Range(minY, maxY), agent.transform.localPosition.z);
            obstacles.Add(GO);
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
        obstacles.Clear();
    }

    public void Remove(GameObject go)
    {
        obstacles.Remove(go);
    }
}
