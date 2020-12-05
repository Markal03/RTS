using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObjectInfo : MonoBehaviour {

	public bool isSelected = false;

	public int id = 0;

	public string objectName;
	public GameObject selectionIndicator;

	public bool isLocalPlayerUnit = false;

	private NavMeshAgent agent;

	public AnimationStateController animationStateController;

	private GameObject attackTarget;

	public Vector3 lastPosition;
	public HealthBar healthBar;

	public float lastAttackTime;
	//Stats
	public int maxHealth = 100;
	public int currentHealth;
	public float attackRange = 1.5f;
	public float attackSpeed = 1.5f;
	public float attackDamage = 10f;




    private void Awake()
    {
		attackTarget = null;
		lastAttackTime = attackSpeed;
    }

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

		//testing
		if (Input.GetKeyDown(KeyCode.K))
		{
			TakeDamage(20);
		}

		if (IsObjectMoving())
        {
			animationStateController.SetWalking(true);
			if(isLocalPlayerUnit)
			ClientSend.UnitPositionUpdate(id, gameObject.transform.position, gameObject.transform.rotation);
		} else
        {
			animationStateController.SetWalking(false);
		}

		if(CanAttack())
        {
			if (lastAttackTime < attackSpeed)
            {
				lastAttackTime += Time.deltaTime;
            }
			else
            {
				Attack(attackTarget);
				lastAttackTime = 0;
            }
        }

		lastPosition = gameObject.transform.position;
	}

	public bool IsObjectMoving() => gameObject.transform.position != lastPosition;
	public bool CanAttack()
	{

		if (attackTarget is null) return false;

		var distance = Vector3.Distance(this.transform.position, attackTarget.transform.position);

		return distance <= attackRange;
	}
  
	public void RightClick()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, 100))
		{
			if (hit.collider.CompareTag("Ground"))
			{
				agent.destination = hit.point;
				attackTarget = null;
				Debug.Log("Moving");
			} 
			else if (hit.collider.CompareTag("Selectable"))
            {
				agent.destination = hit.collider.gameObject.transform.position;

				if (hit.collider.CompareTag("Selectable") && !hit.collider.gameObject.GetComponent<ObjectInfo>().isLocalPlayerUnit)
                {
					attackTarget = hit.collider.gameObject;
				}

            }
		}
	}

	public void Attack(GameObject _target, bool sendUpdate = true)
    {
		if (sendUpdate == true)
        {
			ClientSend.UnitAttack(id, _target.GetComponent<ObjectInfo>().id, _target.GetComponentInParent<PlayerManager>().id);
		}

		animationStateController.SetAttack();
		_target.GetComponent<ObjectInfo>().TakeDamage((int) attackDamage);
    }
	void TakeDamage(int _damage)
	{
		currentHealth -= _damage;
		healthBar.SetHealth(currentHealth);

		if (currentHealth <= 0 )
		{
			Die();
		}
	}
    private void Die()
	{
		//remove object
		animationStateController.SetDies();
	}
}
