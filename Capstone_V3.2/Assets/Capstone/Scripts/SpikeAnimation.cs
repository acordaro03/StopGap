using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSpeed : MonoBehaviour {

    public Animator animator;
    public float animspeed;

	void Start () {
		
	}

    // Update is called once per frame
    void ChangeSpeed() {

        animator.speed = animspeed;
    }


}
