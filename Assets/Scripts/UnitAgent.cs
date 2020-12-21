using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class UnitAgent : Agent
{
    public Transform unitViewPoint;
    public int minStepsBetweenActions = 50;
    public override void OnActionReceived(float[] vectorAction)
    {
        base.OnActionReceived(vectorAction);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Heuristic(float[] actionsOut)
    {
        base.Heuristic(actionsOut);
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
    }
}
