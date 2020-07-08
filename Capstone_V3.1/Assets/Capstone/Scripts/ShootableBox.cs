using UnityEngine;
using System.Collections;

public class ShootableBox : MonoBehaviour
{


    // public bool isMoving = true;
    Rigidbody rb;
    public int currentHealth = 1000000001;
    public float maxHeight;
    public float buffer;
    public GameObject player;



    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        maxHeight = buffer + player.transform.position.y;

    }

    void Start()
    {
        // transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
    }

    /*  public void Collide()
      {


          if (isMoving == true)
          {
              rb.constraints = RigidbodyConstraints.FreezeAll;
              isMoving = false;

            else if (isMoving == false)
          {
              rb.constraints = RigidbodyConstraints.None;
              isMoving = true;

           //   Debug.Log(isMoving + "true");

          }

         /* if (isMoving == false)
          {
              Debug.Log(isMoving + "got it");

          }

      }
       */

    public void Damage(int damageAmount)
    {

        currentHealth -= damageAmount;



        if (currentHealth % 2 == 0)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
            gameObject.GetComponent<Renderer>().material.color = Color.green;

        }

        if (currentHealth % 2 == 1)
        {
            rb.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation;
            //  transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
            gameObject.GetComponent<Renderer>().material.color = Color.red;
        }
    }

}