using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoveablePlatform : MonoBehaviour
{

    public int speed = 45;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private Vector3 currentlyMoveTowardsThisPosition;
    public Transform endPoint;
    public GameObject platform;
    public bool canMove = true;
    Rigidbody rb;
    public AnimationClip sbox;
    public Animation spikebox;
  

    //  public float MoveSpeed = 1.0f; //3

    void Start()
    {

        rb = GetComponent<Rigidbody>(); 
        spikebox = GetComponentInChildren<Animation>();
        spikebox.AddClip(sbox, "SpikeBox");
        startPosition = platform.transform.position;
        endPosition = endPoint.position;
        canMove = true;
    }

    void Update()
    {

        // The step size is equal to speed times frame time.
        float step = speed * Time.deltaTime;

        //if position reached, switch currentlyMoveTowardsThisPosition
        if (platform.transform.position == startPosition)
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

    public void Froze()
    {


        if (canMove == true)
        {

            Debug.Log("Frozen");
            gameObject.GetComponent<Renderer>().material.color = Color.green;
            speed = 0;
            // rb.constraints = RigidbodyConstraints.FreezeAll;
            canMove = false;

            if (!spikebox.isPlaying)
            {
                spikebox.Play();
            }
        }

        else if (!canMove)
        {

            Debug.Log("Moving");
            gameObject.GetComponent<Renderer>().material.color = Color.red;
            // rb.constraints = RigidbodyConstraints.None;
            speed = 45;  //speed
            canMove = true;
            if (spikebox.isPlaying)
            {
                spikebox.Stop();
            }
        }



    }


}




