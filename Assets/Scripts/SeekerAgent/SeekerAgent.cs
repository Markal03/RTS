using System;
 using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;
using System.Linq;
public class SeekerAgent : Agent
{
    SeekerSettings m_SeekerSettings;
    public GameObject area;
    SeekerArea m_SeekerArea;
    Rigidbody m_AgentRb;

    // Speed of agent rotation.
    public float turnSpeed = 2f;
    // Speed of agent movement.
    public float moveSpeed = 1f;

    public GameObject myLaser;
    public bool contribute;
    public bool useVectorObs;
    [Tooltip("Use only the frozen flag in vector observations. If \"Use Vector Obs\" " +
             "is checked, this option has no effect. This option is necessary for the " +
             "VisualFoodCollector scene.")]
    public bool useVectorFrozenFlag;

    public int soldiersCaught = 0;

    EnvironmentParameters m_ResetParams;

    public List<GameObject> _targets = new List<GameObject>();
    public GameObject _target;

    public override void Initialize()
    {
        m_AgentRb = GetComponent<Rigidbody>();
        m_SeekerArea = area.GetComponent<SeekerArea>();
        m_SeekerSettings = FindObjectOfType<SeekerSettings>();
        m_ResetParams = Academy.Instance.EnvironmentParameters;
    }

    private void FixedUpdate()
    {
        RequestDecision();
    }
    public override void CollectObservations(VectorSensor sensor)
    {

        // Check if any unit is on sight
       sensor.AddObservation(GetSight());

        // Add velocity observation
        Vector3 localVelocity = transform.InverseTransformDirection(m_AgentRb.velocity);
        sensor.AddObservation(localVelocity.x);
        sensor.AddObservation(localVelocity.z);

    }

    //Function that moves and rotates agent based on input received
    public void MoveAgent(ActionSegment<int> act)
    {
        float rotateAmount = 0;
        if (act[1] == 1)
        {
            rotateAmount = -turnSpeed;
        }
        else if (act[1] == 2)
        {
            rotateAmount = turnSpeed;
        }

        // Apply the rotation
        Vector3 rotateVector = transform.up * rotateAmount;
        m_AgentRb.MoveRotation(Quaternion.Euler(m_AgentRb.rotation.eulerAngles + rotateVector * turnSpeed));

        // Determine move action
        float moveAmount = 0;
        if (act[0] == 1)
        {
            moveAmount = moveSpeed;
        }
        else if (act[0] == 2)
        {
            moveAmount = moveSpeed * -.5f; // move at half-speed going backwards
        }

        // Apply the movement
        Vector3 moveVector = transform.forward * moveAmount;
        m_AgentRb.AddForce(moveVector * moveSpeed, ForceMode.Impulse);

        //If things go very bad reset the area because learning won't be beneficial at this point
        if (GetCumulativeReward() <= -5f)
        {
            EndEpisode();
            m_SeekerArea.ResetArea();
        } else
        { 
            //remove a small reward for each it in order to discourage the AI from standing still
            AddReward(-.001f);
            m_SeekerSettings.totalScore += GetCumulativeReward();
        }

    }

    //Function that gets enemies placed in front of the agent
    private Vector2 GetSight()
    {
        List<GameObject> soldiers = area.GetComponent<SeekerArea>().GetSoldiers();
        if (soldiers == null)
        return Vector2.zero;

        float leftEye = 0;
        Vector3 leftEyeposition = transform.position - 0.5f * transform.right;
        float rightEye = 0;
        Vector3 rightEyePosition = transform.position + 0.5f * transform.right;

        foreach(GameObject soldier in soldiers)
        {
            if (soldier != null)
            {
                leftEye += .8f - .5f * Mathf.Log10(Vector3.Distance(soldier.transform.position, leftEyeposition));
                rightEye += .8f - .5f * Mathf.Log10(Vector3.Distance(soldier.transform.position, rightEyePosition));
            }
        }

        return new Vector2(leftEye, rightEye);
    }

    //Catch a soldier and add a generous reward
    void Catch()
    {
        soldiersCaught++;
        AddReward(1f);
    }

    //Override of OnActionReceived function telling the agent what to do when some input is received
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        MoveAgent(actionBuffers.DiscreteActions);
    }

    //Override of Heuristic function declaring how to interpretate action received
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = 0; // 1 if forward, 2 if backward
        discreteActionsOut[1] = 0; // 1 if left, 2 if right

        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[1] = 2;
        }
        if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[1] = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
    }

    //Initialisation values of agent
    public override void OnEpisodeBegin()
    {
        m_AgentRb.velocity = Vector3.zero;
        transform.position = new Vector3(Random.Range(-m_SeekerArea.range, m_SeekerArea.range),
            0.7f, Random.Range(-m_SeekerArea.range, m_SeekerArea.range))
            + area.transform.position;
        transform.rotation = Quaternion.identity;
    }

    //Collision detection handler
    void OnCollisionEnter(Collision collision)
    {
        //If the agent collides with a playable unit
        if (collision.gameObject.CompareTag("Selectable")) 
        {
            //Catch and restart episode
            Catch();
            collision.gameObject.GetComponent<PlayerLogic>().OnKilled();
            EndEpisode();

        }
        //If the agent collides with another agent or a wall
        else if (collision.gameObject.CompareTag("Environment") || collision.gameObject.CompareTag("Seeker"))
        {
            //Add a small negative reward
            AddReward(-0.01f);
        }
    }
}
