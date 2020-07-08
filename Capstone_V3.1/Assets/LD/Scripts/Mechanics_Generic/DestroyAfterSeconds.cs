using UnityEngine;
using System.Collections;

/// <summary>
/// This script applies a delayed Destroy command onto this GameObject. After the designated number of seconds, this object will be destroyed
/// </summary>

[DisallowMultipleComponent]                         //Prevent multiple copies of this component to be added to the same gameObject

public class DestroyAfterSeconds : MonoBehaviour {

    [Header("Destroy Settings")]
    [Tooltip("Delay in seconds after which this object will be destroyed")]
    public float secondsUntilDestroy = 5f;

	// Use this for initialization
	void Start () {

		Destroy(gameObject, secondsUntilDestroy);

	}
}
