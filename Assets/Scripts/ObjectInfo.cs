using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObjectInfo : MonoBehaviour {

	public bool isSelected = false;
	public string objectName;
	public GameObject selectionIndicator;
	//Health management
	public int maxHealth = 100;
	public int currentHealth;

	public Vector3 lastPosition;
	public HealthBar healthBar;

	public DeathAnimation death;

	private NavMeshAgent agent;

	public ClientConnection connection;

	public AnimationStateController animationStateController;
	// Use this for initialization
	void Start () {
		lastPosition = gameObject.transform.position;
		agent = GetComponent<NavMeshAgent>();
		currentHealth = maxHealth;
		healthBar.SetMaxHealth(maxHealth);
	}
	
	// Update is called once per frame
	void Update () {

		selectionIndicator.SetActive(isSelected);

		if (Input.GetMouseButtonDown(1) && isSelected)
		{
			RightClick();
		}
		if (Input.GetKeyDown(KeyCode.K))
		{
			TakeDamage(20);
		}

		if (IsObjectMoving())
        {
			animationStateController.SetWalking(true);
			//connection.SendPositionUpdates(gameObject);
		} else
        {
			animationStateController.SetWalking(false);
		}

		lastPosition = gameObject.transform.position;
	}

	public bool IsObjectMoving() => gameObject.transform.position != lastPosition;
  
	public void RightClick()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, 100))
		{
			if (hit.collider.tag == "Ground")
			{
				agent.destination = hit.point;
				Debug.Log("Moving");
			}
		}
	}

	void TakeDamage(int damage)
	{
		currentHealth -= damage;
		healthBar.SetHealth(currentHealth);

		if (currentHealth <= 0 )
		{
			Die();
		}
	}

	private void Die()
	{
		animationStateController.SetDying(true);
	}
}
