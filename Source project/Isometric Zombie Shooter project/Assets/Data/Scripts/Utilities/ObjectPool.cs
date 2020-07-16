using System.Collections.Generic;
using UnityEngine;

/// <summary> Instantiate and contain GameObjects for repeated use </summary>
public class ObjectPool
{
    private List<GameObject> objects = new List<GameObject>();
    private int capacity = 10;
    private int pointer = 0;                        // to use objects in turn
    private int activeObjectsCount = 0;
    private Transform parent = null;                // need to compact instantiate in hierarchy and positioning
    private Vector3 storagePosition = Vector3.zero;

    #region Properties
        public int Count
        {
            get
            {
                return capacity;
            }
        }
        /// <summary> Returns count of objects, that active in game proccess </summary>
        public int ActiveObjectsCount 
        {
            get
            {
                return activeObjectsCount;
            }
        }
    #endregion

    public GameObject this[int index]               // quick access
    {
        get
        {
            return objects[index];
        }
    }

    /// <summary> Initialize and instantiate  GameObjects to the pool </summary>
    public ObjectPool(GameObject prefab, int pool_capacity, Transform pool_parent)
    {
        capacity = pool_capacity;
        parent = pool_parent;
        storagePosition = parent.position;
        for(int i = 0; i < capacity; i++)
        {
            objects.Add(Object.Instantiate(prefab, parent, true));
            objects[i].SetActive(false);
        }
    }

    /// <summary> Activate next GameObject in pool, returns his ID </summary>
    public int ActivateObject(Vector3 position)
    {
        if(IsActive(pointer))   // Throw error if pool try to activate already active object
        {
            Debug.LogError("Object Pool try to activate object, but it's already active. Probably, limit of this pool has been reached." + 
                            "\nCurrent number of objects is " + activeObjectsCount + " with limit of " + capacity);
            return -1;
        }

        objects[pointer].transform.position = position;
        objects[pointer].SetActive(true);
        activeObjectsCount++;
        pointer++;

        if(pointer >= capacity)     // move pointer to the start of pool
        {
            pointer = 0;
            return capacity-1;
        }
        else
            return pointer-1;
    }

    /// <summary> Deactivate GameObject in pool by ID </summary>
    public void DeactivateObject(int id)
    {
        objects[id].SetActive(false);
        activeObjectsCount--;
        objects[id].transform.position = storagePosition;
    }

    /// <summary> Checks whether the object active by his ID </summary>
    public bool IsActive(int id)
    {
        return objects[id].activeInHierarchy;
    }

    /// <summary> Change position in World Space where will be contains inactived objects </summary>
    public void ChangeStoragePosition(Vector3 newPosition)
    {
        storagePosition = newPosition;
        for(int i = 0; i < capacity; i++)       // move objects thats inactive
        {
            if(!IsActive(i))
                objects[i].transform.position = storagePosition;
        }
    }
}
