using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObjectInfo : MonoBehaviour {

	public bool isSelected = false;
	public string objectName;

	//Health management
	public int maxHealth = 100;
	public int currentHealth;
	public HealthBar healthBar;

	public DeathAnimation death;

	private NavMeshAgent agent;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent>();

		currentHealth = maxHealth;
		healthBar.SetMaxHealth(maxHealth);
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetMouseButtonDown(1) && isSelected)
		{
			RightClick();
		}
		if (Input.GetKeyDown(KeyCode.K))
		{
			TakeDamage(20);
		}
	}

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
		death.StartRotation();
	}
}
