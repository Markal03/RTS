using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class RuleBasedAI : MonoBehaviour
{
    ObjectInfo objectInfo;
    NavMeshAgent agent;

    //Patrol variables
    public Transform[] points;
    private int destPoint = 0;

    public float minimumDistance;

    private bool isEnemyNearby;

    private Dictionary<float, GameObject> closeUnits = new Dictionary<float, GameObject>();

    private GameObject target;
    private void Awake()
    {
        objectInfo = gameObject.GetComponent<ObjectInfo>();
        agent = GetComponent<NavMeshAgent>();
        isEnemyNearby = false;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RunRuleset();
    }

    public void RunRuleset()
    {
        SpotEnemy();
        Patrol();
        Chase();
        //Talk();
    }

    private void Patrol()
    {
        if (!isEnemyNearby && !agent.pathPending && agent.remainingDistance < 1f)
        {
             GoToNextPoint();
        }
    }

    private void Chase()
    {
        if (isEnemyNearby)
        {
            target = GetClosestEnemy();
            agent.destination = target.transform.position;
            objectInfo.SetAttackTarget(target);
        }


    }
    private void SpotEnemy()
    {
        isEnemyNearby = CheckIfTagIsNearby("Selectable");
    }
    private bool CheckIfTagIsNearby(string _tag)
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(_tag);
        closeUnits.Clear();

        foreach (GameObject gameObject in gameObjects)
        {
            float distance = Vector3.Distance(transform.position, gameObject.transform.position);
            if (distance <= minimumDistance && gameObject.GetComponent<ObjectInfo>().currentHealth > 0)
            {
                closeUnits.Add(distance, gameObject);
            }

        }

        return closeUnits.Count > 0;
    }

    private void GoToNextPoint()
    {
        if (points.Length == 0)
            return;

        agent.destination = points[destPoint].position;

        destPoint = (destPoint + 1) % points.Length;
    }

    public GameObject GetClosestEnemy() =>
             closeUnits
            .OrderBy(gameObject => gameObject.Key)
            .ToDictionary
            (
            gameObject => gameObject.Key, 
            gameObject => gameObject.Value
            )
            .First()
            .Value;
    

    private void Talk()
    {
        throw new System.NotImplementedException();
    }
}
