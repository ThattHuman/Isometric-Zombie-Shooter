using System.Collections.Generic;
using UnityEngine;

/// <summary> Spawn bullets </summary>
public class AgentSpawner : MonoBehaviour, IAgentSpawnMessenger
{
    [SerializeField] private GameObject agentsTargetGO = null;      // set agents Target
    [SerializeField] private GameObject agentsPrefab = null;        // prefab for object pool
    [SerializeField] private int agentsPoolCapacity = 20;
    [SerializeField] private List<Vector3> spawnPositions = null;   // list of all positions, set up in Inspector
    [SerializeField] private bool TestGizmos = false;

    private ObjectPool agents = null;
    private IAgentObserver scenarioObserver = null;     // feedback to Scenario

    // instantiate objects in pool
    private void Awake() 
    {
        agents = new ObjectPool(agentsPrefab, agentsPoolCapacity, GetComponent<Transform>());
        
        for(int i = 0; i < agents.Count; i++)
        {
            Agent agentSetUp = agents[i].GetComponent<Agent>();
            agentSetUp.SetTarget(agentsTargetGO.GetComponent<Transform>());
        }
    }

    /// <summary> Spawn and set up new Agent </summary>
    public void SpawnAgent()
    {
        int id = agents.ActivateObject(spawnPositions[Random.Range(0, spawnPositions.Count)]);  // random position
        agents[id].GetComponent<Agent>().SetUp(id, this);
        scenarioObserver?.AgentActivated(); // feedback for counting
    }

    /// <summary> Remove Agent by ID </summary>
    public void RemoveAgent(int _id)
    {
        agents.DeactivateObject(_id);
        scenarioObserver?.AgentRemoved();   // feedback for counting
    }

    /// <summary> Set up observer for feedback </summary>
    public void SetUpScenarioObserver(IAgentObserver observer)
    {
        scenarioObserver = observer;
    }

    // display spawn positions
    private void OnDrawGizmosSelected() 
    {
        if(TestGizmos)
        {
            float radius = 0.5f;
            Gizmos.color = Color.magenta;

            for(int i = 0; i < spawnPositions.Count; i++)
            {
                float theta = 0;

                float x = radius * Mathf.Cos(theta);
                float z = radius * Mathf.Sin(theta); 

                Vector3 pos = spawnPositions[i] + new Vector3(x, 0, z);
                Vector3 newPos = pos;
                Vector3 lastPos = pos;

                for(theta = 0.1f; theta < Mathf.PI * 2; theta += 0.1f)  
                {
                    x = radius * Mathf.Cos(theta);
                    z = radius * Mathf.Sin(theta); 

                    newPos = spawnPositions[i] + new Vector3(x, 0, z);

                    Gizmos.DrawLine(pos, newPos);

                    pos = newPos;
                }
                Gizmos.DrawLine(pos, lastPos);
            }
        }
    }
}

/// <summary> Handle Agent removing </summary>
public interface IAgentSpawnMessenger
{
    void RemoveAgent(int _id);
}