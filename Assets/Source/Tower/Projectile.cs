using Source.Enemies;
using Source.Util;
using UnityEngine;

namespace Source.Tower
{
    /// <summary>
    /// Projectiles are shoot from a <see cref="ProjectileController"/> and follow a given enemy target until reached
    /// (unless the target dies in time or reaches its destination)
    /// </summary>
    public class Projectile : PooledObject
    {
        [SerializeField] private float relativeMoveSpeed;
        [SerializeField] private float damageRadius;
        
        private float damage;
        private EnemyController enemyTarget;
        private float currentMoveSpeed;

        private void Update()
        {
            if (enemyTarget != null)
            {
                MoveProjectile();
            }
        }
        
        /// <summary>
        /// Sets Target that will be followed by the projectile.
        /// </summary>
        /// <param name="target">enemy to follow</param>
        public void SetEnemyTarget(EnemyController target)
        {
            enemyTarget = target;
            enemyTarget.DespawnedEvent.AddListener(OnTargetDied);
        }

        /// <summary>
        /// Sets the damage of the projectile, which it deals to the hit enemy target.
        /// </summary>
        /// <param name="value"></param>
        public void SetDamage(float value)
        {
            damage = value;
        }

        private void MoveProjectile()
        {
            transform.position = Vector3.MoveTowards(transform.position, enemyTarget.transform.position,
                currentMoveSpeed * Time.deltaTime);
            float distanceToTarget = (enemyTarget.transform.position - transform.position).magnitude;

            if (distanceToTarget < damageRadius)
            {
                enemyTarget.ReceiveDamage(damage);
                ObjectPooler.ReturnToPool(this);
            }
        }

        private void OnTargetDied(PooledObject obj)
        {
            ObjectPooler.ReturnToPool(this);
        }

        public override void OnReturnToPool()
        {
            base.OnReturnToPool();
            enemyTarget = null;
        }

        protected override void OnTimeSpeedChanged(int timeScale)
        {
            currentMoveSpeed = relativeMoveSpeed * timeScale;
        }
    }
}
