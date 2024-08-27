using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PoolManager : SingletonMonobehaviour<PoolManager>
{
    [SerializeField] private Pool[] poolArray = null;

    // 검색 비용을 절약하기 위해 Dictionary 자료형을 사용 
    private Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>>();

    private Transform objectPoolTransform;

    [System.Serializable]
    public struct Pool
    {
        public int poolSize;
        public GameObject prefab;
    }

    private void Start()
    {
        objectPoolTransform = this.gameObject.transform;

        for (int i = 0; i < poolArray.Length; i++)
        {
            CreatePool(poolArray[i].prefab, poolArray[i].poolSize);
        }
    }

    private void CreatePool(GameObject prefab, int poolSize)
    {
        int poolKey = prefab.GetInstanceID();

        if (!poolDictionary.ContainsKey(poolKey))
        {
            poolDictionary.Add(poolKey, new Queue<GameObject>());

            for (int i = 0; i < poolSize; i++)
            {
                GameObject newObject = Instantiate(prefab, objectPoolTransform);

                newObject.SetActive(false);

                poolDictionary[poolKey].Enqueue(newObject);
            }
        }
    }

    public GameObject ReuseGameObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int poolKey = prefab.GetInstanceID();

        if (poolDictionary.ContainsKey(poolKey))
        {
            GameObject objectToReuse = GetObjectFromPool(poolKey);

            ResetObject(position, rotation, objectToReuse, prefab);

            objectToReuse.SetActive(true);

            return objectToReuse;
        }
        else
        {
            Debug.Log("No Object Pool for " + prefab);
            return null;
        }
    }

    private GameObject GetObjectFromPool(int poolKey)
    {
        GameObject objectToReuse = poolDictionary[poolKey].Dequeue();

        poolDictionary[poolKey].Enqueue(objectToReuse);

        if (objectToReuse.gameObject.activeSelf == true)
        {
            objectToReuse.SetActive(false);
        }

        return objectToReuse;
    }

    private void ResetObject(Vector3 position, Quaternion rotation, GameObject objectToReuse, GameObject prefab)
    {
        objectToReuse.transform.position = position;
        objectToReuse.transform.rotation = rotation;
        objectToReuse.transform.localScale = prefab.transform.localScale;
    }
}
