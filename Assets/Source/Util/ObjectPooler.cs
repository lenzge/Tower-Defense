using System.Collections.Generic;
using UnityEngine;

namespace Source.Util
{
    public class ObjectPooler : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int poolSize;
        
        private List<PooledObject> pool = new List<PooledObject>();
        private GameObject poolContainer;

        private void Awake()
        {
            poolContainer = new GameObject($"Pool - {prefab.name}");
            CreatePooler();
        }
        
        /// <summary>
        /// Spawn a created instance from the pool into the game scene. Create a new instance if the pool is empty.
        /// </summary>
        /// <returns>Instance of the object</returns>
        public PooledObject SpawnInstanceFromPool()
        {
            PooledObject newInstance = null;
            for (int i = 0; i < pool.Count; i++)
            {
                if (!pool[i].gameObject.activeInHierarchy)
                {
                    newInstance = pool[i];
                }
            }

            if (newInstance == null)
            {
                newInstance = CreateInstance(100);
            }

            newInstance.OnSpawnInstanceFromPool();
            newInstance.gameObject.SetActive(true);
            return newInstance;
        }

        /// <summary>
        /// Returns an instance back to the pool.
        /// </summary>
        /// <param name="instance">instance to return to the pool</param>
        public static void ReturnToPool(PooledObject instance)
        {
            instance.OnReturnToPool();
            instance.gameObject.SetActive(false);
        }

        private void CreatePooler()
        {
            for (int i = 0; i < poolSize; i++)
            {
                pool.Add(CreateInstance(i));
            }
        }

        private PooledObject CreateInstance(int i)
        {
            PooledObject newInstance = Instantiate(prefab, (poolContainer.transform), true).GetComponent<PooledObject>();
            newInstance.gameObject.SetActive(false);
            newInstance.name = $"{prefab.name} {i}";
            return newInstance;
        }

    }
}