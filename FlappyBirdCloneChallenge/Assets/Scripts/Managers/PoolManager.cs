using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{

    public static PoolManager Instance;

    private Dictionary<int, Queue<ObjectInstance>> _poolDictionary;

    private void Awake()
    {
        Instance = this;

        _poolDictionary = new Dictionary<int, Queue<ObjectInstance>>();
    }


    public void CreatePool(GameObject prefab, int poolSize, Transform parent = null)
    {
        int poolKey = prefab.GetInstanceID();

        if (!_poolDictionary.ContainsKey(poolKey))
        {
            _poolDictionary.Add(poolKey, new Queue<ObjectInstance>());
        }

        for (int i = 0; i < poolSize; i++)
        {
            ObjectInstance newObject = new ObjectInstance(Instantiate(prefab) as GameObject, parent);
            _poolDictionary[poolKey].Enqueue(newObject);
        }
    }

    public GameObject ReuseObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int poolKey = prefab.GetInstanceID();

        if (_poolDictionary.ContainsKey(poolKey))
        {
            ObjectInstance objectToReuse = _poolDictionary[poolKey].Dequeue();
            _poolDictionary[poolKey].Enqueue(objectToReuse);

            objectToReuse.Reuse(position, rotation);

            return objectToReuse.gameObject;
        }
        else
            return null;
    }
    public int PoolAmount()
    {
        return _poolDictionary.Count;
    }

    public class ObjectInstance
    {
        public GameObject gameObject;

        private bool _hasPoolObjectScript;
        private PoolObject _poolObjectScript;

        public ObjectInstance(GameObject objectInstance, Transform parent = null)
        {
            gameObject = objectInstance;
            gameObject.SetActive(false);
            if (parent != null)
                gameObject.transform.SetParent(parent, false);

            if (gameObject.GetComponent<PoolObject>())
            {
                _hasPoolObjectScript = true;
                _poolObjectScript = gameObject.GetComponent<PoolObject>();
            }
        }

        public void Reuse(Vector3 position, Quaternion rotation)
        {
            gameObject.SetActive(true);
            gameObject.transform.position = position;
            gameObject.transform.rotation = rotation;

            if (_hasPoolObjectScript)
            {
                _poolObjectScript.OnObjectReuse();
            }
        }
    }
}