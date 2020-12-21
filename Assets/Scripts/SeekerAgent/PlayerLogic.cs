using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    public bool respawn;
    public SeekerArea myArea;

    public void OnKilled()
    {
        if (respawn)
        {
            Spawn( new Vector3(Random.Range(-myArea.range, myArea.range),
                3f,
                Random.Range(-myArea.range, myArea.range)) + myArea.transform.position);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Spawn(Vector3 _position)
    {
        var hitColliders = Physics.OverlapSphere(_position, 0.1f);
        if (hitColliders.Length > 2)
        {
            Vector3 _randomPosition = new Vector3(Random.Range(-myArea.range, myArea.range), 0.5f, Random.Range(-myArea.range, myArea.range)) + transform.position;
            Spawn( _randomPosition);
            Debug.Log("Spawn Occupied");
        }
        else
        {
            transform.position = _position;
        }
    }
}
