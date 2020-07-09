using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {

	[Tooltip("What direction does the object rotate?")]
	public Vector3 rotateDirection = new Vector3(15,30,45);		//What direction does the object rotate

	[Tooltip("How fast does the object rotate?")]
	public float rotateSpeed = 1f;		//How fast the object rotates

	// Update is called once per frame
	void Update () {

		//Rotate the object at the designated direction and speed
		transform.Rotate(rotateDirection * Time.deltaTime * rotateSpeed);

	}
}
