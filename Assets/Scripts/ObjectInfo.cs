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
	public Dictionary<int , UpdatePositionPacket> lastReceivedPositions = new Dictionary<int, UpdatePositionPacket>(); //contains the last 3 position updates

	public int totalPositionUpdates = 0;
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

		if (!(attackTarget is null) && attackTarget.GetComponent<ObjectInfo>().currentHealth < 1)
        {
			attackTarget = null;
        } 

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

			if (isLocalPlayerUnit)
				ClientSend.UnitPositionUpdate(id, gameObject.transform.position, gameObject.transform.rotation);
			//else RunPrediction();

		} else
        {
			animationStateController.SetWalking(false);
			lastReceivedPositions.Clear();
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

    private void OnMouseEnter()
    {
		if(!isLocalPlayerUnit && GameManager.players[1].isUnitSelected == true)
			CursorController.instance.ActivateAttackCursor();
    } 
	
	private void OnMouseExit()
    {
		if (!isLocalPlayerUnit)
			CursorController.instance.ActivateDefaultCursor();
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
			animationStateController.enabled = false;
			gameObject.transform.LookAt(hit.collider.gameObject.transform.position);
			animationStateController.enabled = true;

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

	public void Attack(GameObject _target, bool _sendUpdate = true)
    {
		if (_sendUpdate == true)
        {
			ClientSend.UnitAttack(id, _target.GetComponent<ObjectInfo>().id, _target.GetComponentInParent<PlayerManager>().id, (int) attackDamage);
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
		animationStateController.SetDies();
		Invoke("RemoveObject", 2);
	}

	private void RemoveObject()
    {
		Destroy(gameObject);
	}

	private void RunPrediction()
    {
		float predictedX = -1.0f;
		float predictedY = -1.0f;
		float predictedZ = -1.0f;

		int size = lastReceivedPositions.Count;

		if (size < 3)
        {
			//unsufficient data to run prediction
        } else
        {
			UpdatePositionPacket packet0 = lastReceivedPositions[0];
			UpdatePositionPacket packet1 = lastReceivedPositions[1];
			UpdatePositionPacket packet2 = lastReceivedPositions[2];

			predictedX = packet0.position.x;
			predictedY = packet0.position.y;
			predictedZ = packet0.position.z;

			Vector3 velocity;
			Vector3 distanceBetweenLastMessages;
			float timeBetweenLastMessages;

			distanceBetweenLastMessages.x = packet0.position.x - packet1.position.x;
			distanceBetweenLastMessages.z = packet0.position.z - packet1.position.z;
			distanceBetweenLastMessages.y = 0;

			timeBetweenLastMessages = packet0.time - packet1.time;

			velocity = distanceBetweenLastMessages / timeBetweenLastMessages;

			Vector3 lastPosition = new Vector3(packet0.position.x, packet0.position.y, packet0.position.z);

			Vector3 displacement;

			displacement.x = velocity.x * (packet1.time - packet0.time);
			displacement.z = velocity.z * (packet1.time - packet0.time);

			predictedX = lastPosition.x + displacement.x;
			predictedZ = lastPosition.z + displacement.z;

			Vector3 predictedPosition = new Vector3(predictedX, predictedY, predictedZ);
			gameObject.transform.position = predictedPosition;

        }

    }
}
