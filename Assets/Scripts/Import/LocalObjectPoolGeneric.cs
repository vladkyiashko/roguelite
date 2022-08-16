using System.Collections.Generic;
using UnityEngine;

public class LocalObjectPoolGeneric<T> : ILocalObjectPoolGeneric
{
    public GameObject ObjectHolder { get; private set; }
    private readonly Dictionary<GameObject, List<GameObject>> InstancePools = new();
    private Dictionary<GameObject, T> CachedComponentByGameObject = new();
    private readonly Transform ObjectHolderTransform;

    public LocalObjectPoolGeneric()
    {
        ObjectHolder = new GameObject {name = "ObjectHolderGeneric"};
        ObjectHolderTransform = ObjectHolder.transform;
        ObjectHolder.SetActive(false);        
    }

    public T Instantiate(GameObject prefab, Transform transform = null)
    {
        GameObject instanceObject = GetInstance(prefab);
        instanceObject.transform.SetParent(transform); //todo
        return CachedComponentByGameObject[instanceObject];
    }

    public void Destroy(Transform objectInstanceTransform)
    {       
        objectInstanceTransform.parent = ObjectHolderTransform;
    }

    private GameObject GetInstance(GameObject prefab)
    {
        if (!InstancePools.ContainsKey(prefab))
        {
            return CreateInstancePool(prefab);
        }            

        List<GameObject> pool = InstancePools[prefab];
        GameObject objectInstance = GetFreeFromPool(pool);

        if (objectInstance == null)
        {
            objectInstance = CreateInstance(pool, prefab);
        }
            
        return objectInstance;
    }

    private GameObject GetFreeFromPool(IList<GameObject> pool)
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i] == null)
            {
                pool.RemoveAt(i);
                continue;
            }

            if (pool[i].transform.parent == ObjectHolderTransform) //todo
            {
                return pool[i];
            }                
        }

        return null;
    }

    private GameObject CreateInstance(ICollection<GameObject> pool, GameObject prefab)
    {
        GameObject objectInstance = Object.Instantiate(prefab);
        objectInstance.transform.parent = ObjectHolderTransform; //todo
        pool.Add(objectInstance);

        if (!CachedComponentByGameObject.ContainsKey(objectInstance))
        {
            CachedComponentByGameObject.Add(objectInstance, objectInstance.GetComponent<T>());
        }

        return objectInstance;
    }

    private GameObject CreateInstancePool(GameObject prefab)
    {
        List<GameObject> pool = new();
        InstancePools.Add(prefab, pool);
        return CreateInstance(pool, prefab);
    }
}

public interface ILocalObjectPoolGeneric
{
    public void Destroy(Transform objectInstanceTransform);
}