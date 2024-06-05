using Source.Util;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Source.Enemies
{
    /// <summary>
    /// Spawns <see cref="EnemyController"/> from a given <see cref="ObjectPooler"/>. There can be used several
    /// ObjectPools with different Enemy Types.
    /// </summary>
    public class EnemySpawner : TimeDependentBehaviour
    {
        [SerializeField] private ObjectPooler[] pooler;
        [Tooltip("Will be increased by 1 every wave")]
        [SerializeField] private int enemyCount;
        [SerializeField] private float relativeDelayBetwSpawns;

        private float spawnTimer;
        private int enemiesSpawned;
        private int enemiesDespawned;
        private float currentDelayBetwSpawns;

        [HideInInspector] public UnityEvent WaveFinishedEvent;

        private void Update()
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer < 0)
            {
                spawnTimer = currentDelayBetwSpawns;
                if (enemiesSpawned < enemyCount + GameManager.CurrentWaveCount)
                {
                    enemiesSpawned++;
                    PooledObject newEnemy = SpawnEnemy();
                    newEnemy.DespawnedEvent.AddListener(OnDespawn);
                }
            }
        }

        private void OnDespawn(PooledObject enemy)
        {
            enemy.DespawnedEvent.RemoveListener(OnDespawn);
            enemiesDespawned++;
            if (enemiesDespawned >= enemyCount + GameManager.CurrentWaveCount)
            {
                WaveFinishedEvent.Invoke();
            }
        }

        /// <summary>
        /// Resets the enemy spawner to prepare for the next wave.
        /// </summary>
        public void Reset()
        {
            spawnTimer = 0;
            enemiesSpawned = 0;
            enemiesDespawned = 0;
        }

        private PooledObject SpawnEnemy()
        {
            if (Random.value <= 0.5f)
            {
                return pooler[0].SpawnInstanceFromPool();
            }
            else
            {
                return pooler[1].SpawnInstanceFromPool();
            }
            
        }

        protected override void OnTimeSpeedChanged(int timeScale)
        {
            if (spawnTimer > 0f)
            {
                float relativeSpawnTime = spawnTimer / currentDelayBetwSpawns;
                spawnTimer = relativeSpawnTime * relativeDelayBetwSpawns / timeScale;
            }
            currentDelayBetwSpawns = relativeDelayBetwSpawns / timeScale;
        }
    }
}