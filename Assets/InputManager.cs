﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

	public float panSpeed;
	public float rotateSpeed;
	public float rotateAmount;

	private Quaternion rotation;

	private float panDetect = 15f;
	private float minHeight = 10f;
	private float maxHeight = 100f;


	public GameObject selectedObject;

	private ObjectInfo selectedInfo;

	public RectTransform selectionBox;
	private Vector2 startPos;

	// Use this for initialization
	void Start () {
		
		rotation = Camera.main.transform.rotation;

	}
	
	// Update is called once per frame
	void Update () {
		
		MoveCamera();
		RotateCamera();

		if (Input.GetMouseButtonDown(0))
		{
			LeftClick();

		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			Camera.main.transform.rotation = rotation;
		}

		if (Input.GetMouseButtonUp(0))
        {
			ReleaseSelectionBox();

		}

		if (Input.GetMouseButton(0))
        {
			UpdateSelectionBox(Input.mousePosition);
		}
	}

	public void LeftClick()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, 100))
		{
			if (hit.collider.tag == "Ground")
			{
				selectedObject = null;
				Debug.Log("Deselected");
			}
			else if (hit.collider.tag == "Selectable")
			{
				selectedObject = hit.collider.gameObject;
				selectedInfo = selectedObject.GetComponent<ObjectInfo>();

				selectedInfo.isSelected = true;

				Debug.Log("Selected" + selectedInfo.objectName);
			}
		}

		//Used to create selection box
		startPos = Input.mousePosition;
	}
	void MoveCamera() {

		float moveX = Camera.main.transform.position.x;
		float moveY = Camera.main.transform.position.y;
		float moveZ = Camera.main.transform.position.z;

		float xPos = Input.mousePosition.x;
		float yPos = Input.mousePosition.y;
	
		if (Input.GetKey(KeyCode.A) || xPos > 0 && xPos < panDetect) {
			moveX -= panSpeed;
		} else if (Input.GetKey(KeyCode.D) || xPos < Screen.width && xPos > Screen.width - panDetect) {
			moveX += panSpeed;
		} 

		if (Input.GetKey(KeyCode.W) || yPos < Screen.height && yPos > Screen.height - panDetect) {
			moveZ += panSpeed;
		} else if (Input.GetKey(KeyCode.S) || yPos > 0 && yPos < panDetect) {
			moveZ -= panSpeed;
		}

		moveY -= Input.GetAxis("Mouse ScrollWheel") * (panSpeed * 20);

		moveY = Mathf.Clamp(moveY, minHeight, maxHeight);

		Vector3 newPos = new Vector3(moveX, moveY, moveZ);

		Camera.main.transform.position = newPos;
	}

	void UpdateSelectionBox(Vector2 curMousePos)
    {
		if (!selectionBox.gameObject.activeInHierarchy)
			selectionBox.gameObject.SetActive(true);

		float width = curMousePos.x - startPos.x;
		float height = curMousePos.y - startPos.y;

		selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
		selectionBox.anchoredPosition = startPos + new Vector2(width / 2, height / 2);

		//foreach(Unit unit in units)
		//{
        //	Vector3 screenPos = cam.WorldToScreenPoint(unit.transform.position);

		//	if(screenPos.x > min.x && screenPos.x < max.x && screenPos.y > min.y && screenPos.y < max.y)
		//  {
		//		selectedUnits.Add(unit);
		//		unit.ToggleSelectionVisual(true);
		//  }
		//}
    }

	void ReleaseSelectionBox()
    {
		selectionBox.gameObject.SetActive(false);

		Vector2 min = selectionBox.anchoredPosition - (selectionBox.sizeDelta / 2);
		Vector2 max = selectionBox.anchoredPosition + (selectionBox.sizeDelta / 2);

		
    }
	void RotateCamera() {

		Vector3 origin = Camera.main.transform.eulerAngles;
		Vector3 destination = origin;

		if (Input.GetMouseButton(2))
		{
			destination.x -= Input.GetAxis("Mouse Y" ) * rotateAmount;
			destination.y += Input.GetAxis("Mouse X" ) * rotateAmount;
		}

		if (destination != origin)
		{
			Camera.main.transform.eulerAngles = Vector3.MoveTowards(origin, destination, Time.deltaTime * rotateSpeed);
		}
	}
}
