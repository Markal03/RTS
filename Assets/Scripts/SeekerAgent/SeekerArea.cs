using Unity.MLAgentsExamples;
using UnityEngine;
 using System.Collections.Generic;
 using System.Linq;
public class SeekerArea : Area
{
    public GameObject player;
    public int numPlayer;
    public bool respawnPlayer;
    public float range;
    private List<GameObject> players = new List<GameObject>();
    public List<GameObject> agents = new List<GameObject>();
    private void FixedUpdate()
    {
        foreach (GameObject agent in agents)
        {
            Vector3 agentLocation = agent.transform.localPosition;
            if (Mathf.Abs(agentLocation.x) > 70f || Mathf.Abs(agentLocation.z) > 70f || Mathf.Abs(agentLocation.y) > 1.0f)
            {
                Debug.LogWarning("Agent slipped off the arena!");
                SeekerAgent seekerAgentComponent = agent.GetComponent<SeekerAgent>();
                seekerAgentComponent.SetReward(-5f);
                ResetSeekerArea();
            }
        }
    }
    void SpawnSoldiers(int num, GameObject type)
    {
        for (int i = 0; i < num; i++)
        {
            Vector3 _randomPosition = new Vector3(Random.Range(-range, range), 0.5f, Random.Range(-range, range)) + transform.position;
            Quaternion _randomRotation = Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 0f));

            Spawn(type, _randomPosition, _randomRotation);
        }
    }

    public void ResetSeekerArea()
    {
        ResetSoldiers();
        ResetAgents();
    }

    public Vector3 GetCleanPosition(Vector3 _position)
    {
         var hitColliders = Physics.OverlapSphere(_position, 0.1f);
        if (hitColliders.Length > 2)
        {

            Vector3 _randomPosition = new Vector3(Random.Range(-range, range), 0.5f, Random.Range(-range, range)) + transform.position;
            Debug.Log("Spawn Occupied");
            return GetCleanPosition(_randomPosition);

        }
        else
        {
            return _position;
        }
    }
    public void Spawn(GameObject _objectToSpawn, Vector3 _position, Quaternion _rotation)
    {

        var hitColliders = Physics.OverlapSphere(_position, 0.1f);
        if (hitColliders.Length > 2)
        {

            Vector3 _randomPosition = new Vector3(Random.Range(-range, range), 0.5f, Random.Range(-range, range)) + transform.position;
            Quaternion _randomRotation = Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 0f));

            Spawn(_objectToSpawn, _randomPosition, _randomRotation);

            Debug.Log("Spawn Occupied");

        }
        else
        {
            GameObject p = Instantiate(_objectToSpawn, _position, _rotation);
            players.Add(p);
            p.GetComponent<PlayerLogic>().respawn = respawnPlayer;
            p.GetComponent<PlayerLogic>().myArea = this;
            p.transform.parent = gameObject.transform;
        }
    }
    public List<GameObject> GetSoldiers()
    {
        return players;
    }
    public void ResetSoldiers()
    {
        if (players != null)
        {
            foreach(GameObject player in players.ToArray())
            {
                Destroy(player);
            }
        }

        SpawnSoldiers(numPlayer, player);
    }

    public void ResetAgents()
    {
        foreach (GameObject agent in agents)
        {
            if (agent.transform.parent == gameObject.transform)
            {
                Vector3 _randomPosition = new Vector3(Random.Range(-range, range), 0.5f, Random.Range(-range, range)) + transform.position;
                agent.transform.position = GetCleanPosition(_randomPosition);
                agent.transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360), 0f));
                agent.GetComponent<SeekerAgent>().soldiersCaught = 0;
                agent.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
    }
    public override void ResetArea()
    {
        ResetSoldiers();
        ResetAgents();
    }
}
