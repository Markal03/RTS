using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();

    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;

    public GameObject localUnitPrefab;
    public GameObject [] unitPrefabs;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destrying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        players.Add(1, GameObject.Find("LocalPlayer").GetComponent<PlayerManager>());
        players.Add(2, GameObject.Find("Player").GetComponent<PlayerManager>());
    }

}
