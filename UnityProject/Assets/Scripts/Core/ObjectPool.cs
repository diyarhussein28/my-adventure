using System.Collections.Generic;
using UnityEngine;

namespace SeasOfLegends.Core
{
    /// <summary>
    /// Generic object pool for GameObjects. Dramatically reduces GC pressure
    /// during combat by recycling hit effects, projectiles, and debris.
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        public static ObjectPool Instance { get; private set; }

        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int size;
        }

        [SerializeField] private List<Pool> pools;
        private Dictionary<string, Queue<GameObject>> poolDictionary;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializePools();
        }

        private void InitializePools()
        {
            poolDictionary = new Dictionary<string, Queue<GameObject>>();

            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }

                poolDictionary.Add(pool.tag, objectPool);
            }
        }

        /// <summary>
        /// Spawns an object from the pool at the specified position/rotation.
        /// </summary>
        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
                return null;
            }

            GameObject objectToSpawn;
            Queue<GameObject> pool = poolDictionary[tag];

            if (pool.Count > 0)
            {
                objectToSpawn = pool.Dequeue();
            }
            else
            {
                // Pool exhausted - create new instance (emergency fallback)
                Pool poolConfig = pools.Find(p => p.tag == tag);
                if (poolConfig != null)
                    objectToSpawn = Instantiate(poolConfig.prefab);
                else
                    return null;
            }

            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

            // Notify the spawned object so it can reset itself
            IPoolable poolable = objectToSpawn.GetComponent<IPoolable>();
            poolable?.OnSpawnFromPool();

            return objectToSpawn;
        }

        /// <summary>
        /// Returns an object to its pool for reuse.
        /// </summary>
        public void ReturnToPool(string tag, GameObject objectToReturn)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool with tag {tag} doesn't exist. Destroying object instead.");
                Destroy(objectToReturn);
                return;
            }

            objectToReturn.SetActive(false);
            poolDictionary[tag].Enqueue(objectToReturn);

            IPoolable poolable = objectToReturn.GetComponent<IPoolable>();
            poolable?.OnReturnToPool();
        }
    }

    /// <summary>
    /// Implement this interface on any component that needs to reset
    /// when spawned from or returned to the object pool.
    /// </summary>
    public interface IPoolable
    {
        void OnSpawnFromPool();
        void OnReturnToPool();
    }
}
