using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour {

    public int speed = 45;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private Vector3 currentlyMoveTowardsThisPosition;
    public Transform endPoint;
    public GameObject platform;
    public bool isMoving = true;
   // Rigidbody rb;
    public int startSpeed;
    public Material[] materials;
    public Renderer rend;
   


    //  public float MoveSpeed = 1.0f; //3

    void Start()
    {
       
       
        startPosition = platform.transform.position;
        endPosition = endPoint.position;
       

        //  m_Material = GetComponent<Renderer>().material;
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
                currentlyMoveTowardsThisPosition = endPosition;
            }
            if (platform.transform.position == endPosition)
            {
                currentlyMoveTowardsThisPosition = startPosition;
            }

            // Move our position a step closer to the target.
            platform.transform.position = Vector3.MoveTowards(platform.transform.position, currentlyMoveTowardsThisPosition, step);
        
    }

    public void Frozen()
    {


        if (isMoving == true)
        {

            Debug.Log("Frozen");
            speed = 0;
            isMoving = false;
         
            rend.materials[1].color = Color.red;
          

        }

        else if (!isMoving)
        {

            Debug.Log("Moving");
            speed = startSpeed;  //speed
            isMoving = true;
    
            rend.materials[1].color = Color.green;
           
        }



    }
}
