using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiMove : MonoBehaviour
{

    public int speed = 45;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private Vector3 midPosition;
    private Vector3 currentlyMoveTowardsThisPosition;
    public Transform endPoint;
    public Transform midPoint;
    public GameObject platform;
    public bool isMoving = true;
    Rigidbody rb;
    public int startSpeed;


    //  public float MoveSpeed = 1.0f; //3

    void Start()
    {

        rb = GetComponent<Rigidbody>();

        startPosition = platform.transform.position;
        midPosition = midPoint.position; 
        endPosition = endPoint.position;
        isMoving = true;
        startSpeed = speed;
    }

    void Update()
    {

        // The step size is equal to speed times frame time.
        float step = speed * Time.deltaTime;

        //if position reached, switch currentlyMoveTowardsThisPosition
        if (platform.transform.position == startPosition)
        {
            currentlyMoveTowardsThisPosition = midPosition;
        }
          if (platform.transform.position == midPosition)
        {
            currentlyMoveTowardsThisPosition = endPosition;
        }
        if (platform.transform.position == endPosition)
        {
            currentlyMoveTowardsThisPosition = startPosition;
        }

        // Move our position a step closer to the target.
        platform.transform.position = Vector3.MoveTowards(platform.transform.position, currentlyMoveTowardsThisPosition, step);

    }

    public void Frize()
    {


        if (isMoving == true)
        {

            Debug.Log("Frozen");
            gameObject.GetComponent<Renderer>().material.color = Color.green;
            speed = 0;
            // rb.constraints = RigidbodyConstraints.FreezeAll;
            isMoving = false;


        }

        else if (!isMoving)
        {

            Debug.Log("Moving");
            gameObject.GetComponent<Renderer>().material.color = Color.red;
            // rb.constraints = RigidbodyConstraints.None;
            speed = startSpeed;  //speed
            isMoving = true;

        }



    }
}
