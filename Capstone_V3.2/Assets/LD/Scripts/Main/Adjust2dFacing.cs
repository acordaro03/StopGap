using UnityEngine;
using System.Collections;

public class Adjust2dFacing : MonoBehaviour
{
	public CheckType inputType = CheckType.InputAxis;
	public float axisThreshold = 0.2f;
	public KeyCode inputKeyLeft = KeyCode.A;
	public KeyCode inputKeyRight = KeyCode.D;

	private bool isFacingRight = true;

	public enum CheckType
	{
		InputAxis,
		InputKeys
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(inputType == CheckType.InputAxis) // check axis input
		{
			float xInput = Input.GetAxis("Horizontal");

			if(!isFacingRight && xInput > axisThreshold) // facing left - turn right
			{
				transform.Rotate(0, 180, 0);
			}
			else if(isFacingRight && xInput < -axisThreshold) // facing right - turn left
			{
				transform.Rotate(0, 180, 0);
			}
		}
		else if(inputType == CheckType.InputKeys) // check key input
		{
			if(!isFacingRight && Input.GetKey(inputKeyRight)) // facing left - turn right
			{
				transform.Rotate(0, 180, 0);
			}
			else if(isFacingRight && Input.GetKey(inputKeyLeft)) // facing right - turn left
			{
				transform.Rotate(0, 180, 0);
			}
		}

		// update facing direction
		isFacingRight = (transform.position.x + (transform.forward * 5).x) > transform.position.x;
	}
}
