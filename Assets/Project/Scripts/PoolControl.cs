using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolControl : MonoBehaviour 
{
	public Transform pooledObject;		// Object to be pooled
	public int poolSize = 15;			// size of the pool
	public bool poolgrows;               // does the pool grows when necesary?
	List<Transform> pool;				// the pool

	void Start () 
	{
		InitPool();
	}

	//  Take an available object from the pool and returned
	public Transform Instantiate()
	{
		for (int i = 0; i < pool.Count; i++)
		{
			if (!pool[i].gameObject.activeSelf)
			{
				pool[i].gameObject.SetActive(true);
				pool[i].SetParent(null);
				return pool[i];
			}
		}
		if (poolgrows)
		{
			poolSize++;
			return CreateOneElement(true);
		}
		else
			return null;
	}

	//Put the unused object back to the pool
	public void Destroy(Transform pollObj)
	{
		int index = pool.FindLastIndex( (obj => obj == pollObj));

		if (index >= 0)
		{
			pool[index].gameObject.SetActive(false);
			pool[index].SetParent(transform);
		}
		else
			Debug.Log("Element not found in pool");
	}

	//Create the Pool and every object
	public void InitPool()
	{
		pool = new List<Transform>();

		for (int i = 0; i < poolSize; i++)
		{
			CreateOneElement(false);
		}
	}

	//Create one object to be put it on the pool
	Transform CreateOneElement(bool active)
	{
		Transform newObj = Instantiate(pooledObject) as Transform;
		newObj.gameObject.SetActive(active);
		newObj.SetParent(transform);
		pool.Add(newObj);
		return newObj;
	}
	// Return all objects on screen to the pool
	public void DestroyAll()
	{
		for (int i = 0; i < poolSize; i++)
		{
			this.Destroy(pool[i]);
		}
	}
}
