using UnityEngine;
using UnityEngine.UI;
using Unity.MLAgents;

public class SeekerSettings : MonoBehaviour
{
    [HideInInspector]
    public GameObject[] agents;
    [HideInInspector]
    public SeekerArea[] listArea;

    public float totalScore;
    public Text scoreText;

    StatsRecorder m_Recorder;

    public void Awake()
    {
        Academy.Instance.OnEnvironmentReset += EnvironmentReset;
        m_Recorder = Academy.Instance.StatsRecorder;
    }

    void EnvironmentReset()
    {
        ClearObjects(GameObject.FindGameObjectsWithTag("Selectable"));

        agents = GameObject.FindGameObjectsWithTag("Seeker");
        listArea = FindObjectsOfType<SeekerArea>();
        foreach (var fa in listArea)
        {
            fa.ResetSeekerArea();
        }

        totalScore = 0;
    }

    void ClearObjects(GameObject[] objects)
    {
        foreach (var player in objects)
        {
            Destroy(player);
        }
    }

    public void Update()
    {
        scoreText.text = $"Score: {totalScore}";

        // Send stats via SideChannel so that they'll appear in TensorBoard.
        // These values get averaged every summary_frequency steps, so we don't
        // need to send every Update() call.
        if ((Time.frameCount % 100) == 0)
        {
            m_Recorder.Add("TotalScore", totalScore);
        }
    }
}
