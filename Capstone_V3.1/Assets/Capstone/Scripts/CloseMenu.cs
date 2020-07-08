using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseMenu : MonoBehaviour
{
    public GameObject introMenu;
    public bool active = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.X) && active == true)
        {
            introMenu.SetActive(false);
            active = false;
        }

        if (Input.GetKey(KeyCode.X) && active == false)
        {
            introMenu.SetActive(true);
            active = true;
        }
    }

    void Close() 
    {
        
    }

}
