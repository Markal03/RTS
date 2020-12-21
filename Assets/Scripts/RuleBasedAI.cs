using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System;

public class RuleBasedAI : MonoBehaviour
{
    ObjectInfo objectInfo;
    NavMeshAgent agent;

    //Patrol variables
    public Transform[] points;
    private int destPoint = 0;

    public float minimumDistance;
    public float minimumAllyDistance;

    private bool isEnemyNearby;
    private bool isAllyNearby;

    private int currentActionPriority;

    private Dictionary<float, GameObject> closeUnits = new Dictionary<float, GameObject>();
    private Dictionary<float, GameObject> closeAllies = new Dictionary<float, GameObject>();

    private GameObject target;

    private float talkCooldown = 300;
    private float lastTalkTime = 300;

    private bool isTalking = false;
    private void Awake()
    {
        objectInfo = gameObject.GetComponent<ObjectInfo>();
        agent = GetComponent<NavMeshAgent>();
        isEnemyNearby = false;
        isAllyNearby = false;
        currentActionPriority = 1;
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
        Patrol();

        SpotEnemy();

        Chase();

        SpotAlly();
        Talk();


    }

    //Priority 1 - Low
    private void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 1f && currentActionPriority == 1 && !isTalking)
        {
             GoToNextPoint();
        }
    }

    //Priority 2 - Medium
    private void Talk()
    {
        if (lastTalkTime < talkCooldown)
        {
            lastTalkTime += Time.deltaTime;
            currentActionPriority = 1;

        }
        else if (currentActionPriority == 2)
        {
            target = GetClosestAlly();
            agent.destination = target.transform.position;
            Debug.Log("Starting to talk");
            StartCoroutine(Wait(2, TalkCallback));
            lastTalkTime = 0;
        }
    }

    //Priority 3 - High
    private void Chase()
    {
        if (currentActionPriority == 3)
        {
            target = GetClosestEnemy();
            agent.destination = target.transform.position;
            gameObject.transform.LookAt(target.transform);
            objectInfo.SetAttackTarget(target);
        }

    }

    private IEnumerator Wait(float duration, Action callback)
    {
        isTalking = true;
        yield return new WaitForSeconds(duration);
        callback();
    }

    void TalkCallback()
    {
        isTalking = false;
        currentActionPriority = 1;
    }
    
    private void SpotEnemy()
    {
        isEnemyNearby = CheckIfTagIsNearby("Selectable");
        this.currentActionPriority = isEnemyNearby ? 3 : 1;
    }
    
    private void SpotAlly()
    {
        isAllyNearby = CheckIfAllyTagIsNearby("Seeker");
        if (currentActionPriority < 3)
        { 
            this.currentActionPriority = isAllyNearby ? 2 : 1;
        }
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
    
    private bool CheckIfAllyTagIsNearby(string _tag)
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(_tag);
        closeAllies.Clear();

        foreach (GameObject gameObject in gameObjects)
        {
            float distance = Vector3.Distance(transform.position, gameObject.transform.position);
            if (distance <= minimumAllyDistance && gameObject.GetComponent<ObjectInfo>().currentHealth > 0 && distance > 0)
            {
                closeAllies.Add(distance, gameObject);
            }

        }

        return closeAllies.Count > 0;
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
    
    public GameObject GetClosestAlly() =>
             closeAllies
            .OrderBy(gameObject => gameObject.Key)
            .ToDictionary
            (
            gameObject => gameObject.Key, 
            gameObject => gameObject.Value
            )
            .First()
            .Value;
    


}
