using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour {

	//The Object type being pooled
	public GameObject pooledObject;
	//How many objects in the pool
	public int pooledAmount;
	//Declare a list for the pooled objects
	List<GameObject> pooledObjects;

	void Awake(){
		//Re-initialize the list
		pooledObjects = new List<GameObject>();

        //fill the list with gameObjects
        for (int i = 0; i < pooledAmount; i++)
        {
            //instantiate Pooled Object as new gameObject
            GameObject obj = (GameObject)Instantiate(pooledObject);
            //parent the new object under the pooler that created it
            obj.transform.parent = gameObject.transform;
            //make sure gameObject is not active
            obj.SetActive(false);
            //Add new instantiated gameObject to the list
            pooledObjects.Add(obj);
        }
    }
	/// <summary>
	/// Gets the pooled object.
	/// </summary>
	/// <returns>The pooled object.</returns>
	public GameObject GetPooledObject(){
		//Search through the pooled Objects list for an inactive object
        if(pooledObjects != null)
        {
            for (int i = 0; i < pooledObjects.Count; i++)
            {
                //if the object currently being searched is not active, return it
                if (!pooledObjects[i].activeInHierarchy)
                {
                    //Return the object
                    return pooledObjects[i];
                }
            }
        }

		//if there are no inactive objects in the list, we must create a new one
		//instantiate Pooled Object as new gameObject
		GameObject obj = (GameObject)Instantiate(pooledObject);
		//make sure gameObject is not active
		obj.SetActive(false);
		//Add new instantiated gameObject to the list
		pooledObjects.Add(obj);
		//return the object we just created
		return obj;
	}
}
