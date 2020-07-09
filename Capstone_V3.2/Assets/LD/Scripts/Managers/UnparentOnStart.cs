using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script will unparent the object it is attached to when the level starts. This is useful to add to objects that rely on being in the scene root (level persistent objects, for example).
/// </summary>

[DisallowMultipleComponent]                         //Prevent multiple copies of this component to be added to the same gameObject

public class UnparentOnStart : MonoBehaviour {

    private void Awake()
    {
        //Unparent the GameObject this script is attached to, putting it in the root
        transform.parent = null;
    }
}
