using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathAnimation : MonoBehaviour {
	public GameObject objectToRotate;	
	private IEnumerator Rotate( Vector3 angles, float inTime )
	{
 		var fromAngle = transform.rotation;
         var toAngle = Quaternion.Euler(transform.eulerAngles + angles);
         for(var t = 0f; t < 1; t += Time.deltaTime/inTime) {
             transform.rotation = Quaternion.Slerp(fromAngle, toAngle, t);
		 
			yield return null;

         }
			//Kill();
	}
 
	private void Kill()
	{
		
		Destroy(objectToRotate);
	}
	public void StartRotation()
	{
			StartCoroutine( Rotate( new Vector3(-90, 0, 0), 1 ) ) ;

	}
}
