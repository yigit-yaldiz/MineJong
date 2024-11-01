using System;
using System.Collections.Generic;
using UnityEngine;

namespace BaseAssets
{
    [DefaultExecutionOrder(-1)]
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance { get; private set; }

        [SerializeField] private List<Pool> pools = new List<Pool>();

        private GameObject poolParent;

        [Serializable]
        public class Pool
        {
            public int AvailableCount => poolList.Count;
            public string id;
            public GameObject poolObject;
            public int count;
            public bool deactivateOnCreate = true;
            public bool activateOnGet = true;
            public bool addOnRunOut = true;
            private List<GameObject> poolList = new List<GameObject>();

            public void CreateOne()
            {
                GameObject ins = Instantiate(poolObject, Instance.poolParent.transform);
                if (deactivateOnCreate)
                    ins.SetActive(false);
                poolList.Add(ins);
            }
            public void Create()
            {
                for (int i = 0; i < count; i++)
                {
                    GameObject ins = Instantiate(poolObject, Instance.poolParent.transform);
                    if (deactivateOnCreate)
                        ins.SetActive(false);
                    poolList.Add(ins);
                }
            }
            public void Create(GameObject item, int count, string id)
            {
                poolObject = item;
                this.count = count;
                this.id = id;

                for (int i = 0; i < count; i++)
                {
                    GameObject ins = Instantiate(poolObject, Instance.poolParent.transform);
                    if (deactivateOnCreate)
                        ins.SetActive(false);
                    poolList.Add(ins);
                }
            }
            public void Add(GameObject item)
            {
                poolList.Add(item);
            }

            public GameObject Get()
            {
                GameObject obj = poolList[0];
                poolList.RemoveAt(0);
                if (activateOnGet)
                    obj.SetActive(true);
                return obj;
            }
        }
        private void Awake()
        {
            Instance = this;
            poolParent = new GameObject("OBJECT POOL");

            for (int i = 0; i < pools.Count; i++)
            {
                pools[i].Create();
            }
        }

        private void Start()
        {
            poolParent.transform.position = Managers.RequirementPanel.Instance.transform.position - Vector3.up;

            foreach (Transform child in transform)
            {
                child.position = transform.position;
            }
        }

        public bool IsTherePool(string id)
        {
            foreach (var pool in pools)
            {
                if (pool.id == id)
                {
                    return true;
                }
            }

            return false;
        }

        public void CreatePool(GameObject item, int count, bool deactivateOnCreate = true, bool activateOnGet = true, bool addOnRunOut = true)
        {
            for (int i = 0; i < pools.Count; i++)
            {
                if (pools[i].id == item.name)
                {
                    Debug.LogWarning("Pool cant created. There is pool with this object!");
                    return;
                }
            }
            Pool pool = new Pool();
            pool.activateOnGet = activateOnGet;
            pool.deactivateOnCreate = deactivateOnCreate;
            pool.addOnRunOut = addOnRunOut;
            pool.Create(item, count, item.name);
            pools.Add(pool);
        }
        public void CreatePool(GameObject item, int count, string id, bool deactivateOnCreate = true, bool activateOnGet = true)
        {
            for (int i = 0; i < pools.Count; i++)
            {
                if (pools[i].id == id)
                {
                    Debug.LogWarning("Pool cant created. There is pool with this object!");
                    return;
                }
            }
            Pool pool = new Pool();
            pool.activateOnGet = activateOnGet;
            pool.deactivateOnCreate = deactivateOnCreate;
            pool.Create(item, count, id);
            pools.Add(pool);
        }

        public GameObject GetPoolObject(string id)
        {
            for (int i = 0; i < pools.Count; i++)
            {
                if (pools[i].id == id)
                {
                    if (pools[i].AvailableCount > 0)
                        return pools[i].Get();
                    else
                    {
                        if (pools[i].addOnRunOut)
                        {
                            pools[i].CreateOne();
                            return pools[i].Get();
                        }
                        Debug.LogWarning("There is no object left in pool with id : " + id);
                        return null;
                    }
                }
            }
            Debug.LogWarning("There is no pool with id : " + id);
            return null;
        }
        public T GetPoolObject<T>(string id) where T : UnityEngine.Object
        {
            return GetPoolObject(id).GetComponent<T>();
        }
        public void PutPoolObject(GameObject item, string id)
        {
            for (int i = 0; i < pools.Count; i++)
            {
                if (pools[i].id == id)
                {
                    pools[i].Add(item);
                    return;
                }
            }
            Debug.LogWarning("There is no pool with id : " + id);
        }
    }
}
