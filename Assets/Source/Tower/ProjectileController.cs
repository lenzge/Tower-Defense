using Source.Util;
using UnityEngine;

namespace Source.Tower
{
    /// <summary>
    /// Shoots <see cref="Projectile"/> from a given <see cref="ObjectPooler"/> on given targets.
    /// </summary>
    public class ProjectileController : TimeDependentBehaviour
    {
        [SerializeField] private Transform projectileSpawnPosition;
        [SerializeField] private float relativeDelayBtwAttacks;
        [SerializeField] private float damage;
        [SerializeField] private ObjectPooler pooler;
        [SerializeField] private Tower tower;

        private float attackTimer;
        private Projectile currentProjectile;
        private float currentDelayBtwAttacks;

        private void Start()
        {
            LoadProjectile();
        }

        private void Update()
        {
            if (GameManager.CurrentGameState != GameState.Wave) { return; }
            
            attackTimer -= Time.deltaTime;
            
            if (currentProjectile == null)
            {
                LoadProjectile();
            }
            
            if (attackTimer < 0)
            {
                if (tower.CurrentEnemyTarget != null && currentProjectile != null)
                {
                    currentProjectile.transform.parent = null;
                    currentProjectile.SetEnemyTarget(tower.CurrentEnemyTarget);
                    attackTimer = currentDelayBtwAttacks;
                    currentProjectile = null;
                }
                else
                {
                    attackTimer = currentDelayBtwAttacks / 4f;
                }
            }
        }

        private void LoadProjectile()
        {
            PooledObject newProjectile = pooler.SpawnInstanceFromPool();
            newProjectile.transform.localPosition = projectileSpawnPosition.position;
            newProjectile.transform.SetParent(projectileSpawnPosition);

            currentProjectile = newProjectile.GetComponent<Projectile>();
            currentProjectile.SetDamage(damage);
        }

        protected override void OnTimeSpeedChanged(int timeScale)
        {
            if (attackTimer > 0)
            {
                float relativeAttackTime = attackTimer / currentDelayBtwAttacks;
                attackTimer = relativeAttackTime * relativeDelayBtwAttacks / timeScale;
            }
            currentDelayBtwAttacks = relativeDelayBtwAttacks / timeScale;
        }
    }
}