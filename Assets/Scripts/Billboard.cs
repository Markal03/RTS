using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {

public Transform cam;
	private void Awake()
    {
		if (cam == null)
        {
			cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<Camera>().transform;
        }
    }
	void LateUpdate () {
		transform.LookAt(transform.position + cam.forward);
	}
}
