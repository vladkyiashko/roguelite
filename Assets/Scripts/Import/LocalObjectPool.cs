using System.Collections.Generic;
using UnityEngine;

public class LocalObjectPool : MonoBehaviour
{
    public bool Enabled = true;
    public GameObject ObjectHolder { get; private set; }
    protected Dictionary<GameObject, List<GameObject>> InstancePools = new Dictionary<GameObject, List<GameObject>>();
#if UNITY_EDITOR
    private bool MissObject;
#endif

    public static GameObject Instantiate(GameObject prefab, Transform transform = null)
    {
        GameObject instanceObject = Instance.Enabled ? Instance.GetInstance(prefab) : Object.Instantiate(prefab);
        instanceObject.transform.SetParent(transform);
        return instanceObject;
    }

    public static void PrewarmPool(List<GameObject> objectsToPool, int dublicates)
    {
        foreach (GameObject prefab in objectsToPool)
        {
            PrewarmPool(prefab, dublicates);
        }
    }

    public static void PrewarmPool(GameObject prefab, int dublicates)
    {
        if (!Instance.Enabled)
            return;

        if (!Instance.InstancePools.ContainsKey(prefab))
            Instance.InstancePools.Add(prefab, new List<GameObject>());

        List<GameObject> pool = Instance.InstancePools[prefab];

        for (int i = 0; i < dublicates; i++)
        {
            Instance.CreateInstance(pool, prefab);
        }
    }

    public static void Destroy(GameObject objectInstance)
    {       
        if (Instance.Enabled)
        {
            if (objectInstance.name.Contains("(Clone)"))
            {
#if UNITY_EDITOR
                Debug.LogError("Non ObjectPool object detected! " + objectInstance.name);
#endif
                Object.Destroy(objectInstance);
            }
            else
                if ( objectInstance.activeSelf)
                    objectInstance.transform.parent = Instance.ObjectHolder.transform;
        }
        else
        {
            Object.Destroy(objectInstance);
        }
    }

    private GameObject GetInstance(GameObject prefab)
    {
        if (!InstancePools.ContainsKey(prefab))
            return CreateInstancePool(prefab);

        List<GameObject> pool = InstancePools[prefab];
        GameObject objectInstance = GetFreeFromPool(pool);

#if UNITY_EDITOR
        if (MissObject && objectInstance == null)
            Debug.LogError("Wow you use non Object Poll Destroy() " + prefab);
        MissObject = false;
#endif

        if (objectInstance == null)
            objectInstance = CreateInstance(pool, prefab);
        return objectInstance;
    }

    private GameObject GetFreeFromPool(IList<GameObject> pool)
    {
#if UNITY_EDITOR
        for (int i = 0; i < pool.Count; i++)
        {
            GameObject item = pool[i];
            if (item != null)
                continue;

            pool.RemoveAt(i);
            MissObject = true;
        }

        if (MissObject)
            return null;
#endif
        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i] == null)
            {
                pool.RemoveAt(i);
                continue;
            }

            if (pool[i].transform.parent == ObjectHolder.transform)
                return pool[i];
        }

        return null;
    }

    private GameObject CreateInstance(ICollection<GameObject> pool, GameObject prefab)
    {
        GameObject objectInstance = Object.Instantiate(prefab);
        objectInstance.name = objectInstance.name.Replace("(Clone)", "_" + pool.Count);
        objectInstance.transform.parent = ObjectHolder.transform;
        pool.Add(objectInstance);
        return objectInstance;
    }

    private GameObject CreateInstancePool(GameObject prefab)
    {
        List<GameObject> pool = new List<GameObject>();
        InstancePools.Add(prefab, pool);
        return CreateInstance(pool, prefab);
    }

    private void Start()
    {
        if (instance != this)
            Destroy(gameObject);
    }

    static LocalObjectPool instance;

    public static LocalObjectPool Instance
    {
        get
        {
            if (instance != null)
                return instance;

            instance = new GameObject().AddComponent<LocalObjectPool>();
            instance.Enabled = true;
            instance.name = "LocalObjectPool";
            instance.ObjectHolder = new GameObject {name = "ObjectHolder"};
            instance.ObjectHolder.transform.parent = instance.transform;
            instance.ObjectHolder.SetActive(false);
            return instance;
        }
    }
}