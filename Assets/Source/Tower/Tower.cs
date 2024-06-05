using System.Collections.Generic;
using Source.Enemies;
using Source.Util;
using UnityEngine;

namespace Source.Tower
{
    /// <summary>
    /// Tower can be build via <see cref="BuildingController"/>. Detects <see cref="EnemyController"/> in range and
    /// rotate towards them. Use <see cref="ProjectileController"/> to shoot at the targets.
    /// </summary>
    public class Tower : MonoBehaviour
    {
        [SerializeField] private Transform model;
        
        public EnemyController CurrentEnemyTarget;
        public int Price;

        private List<EnemyController> enemiesInRange = new List<EnemyController>();
        private bool isGameOver;

        private void Start()
        {
            GameManager.GameStateChangedEvent += OnGameStateChanged;
        }
        
        private void Update()
        {
            if (isGameOver) return;
            RotateTowardTarget();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                EnemyController newEnemy = other.GetComponentInParent<EnemyController>();
                enemiesInRange.Add(newEnemy);
                GetCurrentEnemyTarget();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                EnemyController enemy = other.GetComponentInParent<EnemyController>();
                if (enemiesInRange.Contains(enemy))
                {
                    enemiesInRange.Remove(enemy);
                    GetCurrentEnemyTarget();
                }
            }
        }

        private void RotateTowardTarget()
        {
            if (CurrentEnemyTarget == null)
            {
                return;
            }

            Vector3 targetPosition = CurrentEnemyTarget.transform.position - model.position;
            float angle = Vector3.SignedAngle(model.forward, targetPosition, Vector3.up);
            Quaternion targetRotation = Quaternion.Euler(0f, angle, 0f);
            model.rotation = Quaternion.Slerp(model.rotation, targetRotation, Time.deltaTime * 10f);
        }

        private void GetCurrentEnemyTarget()
        {
            if (enemiesInRange.Count <= 0)
            {
                CurrentEnemyTarget = null;
                return;
            }

            if (CurrentEnemyTarget != enemiesInRange[0])
            {
                CurrentEnemyTarget = enemiesInRange[0];
                CurrentEnemyTarget.DespawnedEvent.AddListener(OnTargetDied);
            }
        }

        private void OnTargetDied(PooledObject enemy)
        {
            enemiesInRange.Remove(enemy.GetComponent<EnemyController>());
            GetCurrentEnemyTarget();
        }
        
        private void OnGameStateChanged(GameState gameState)
        {
            if (gameState == GameState.GameOver)
            {
                isGameOver = true;
            }
        }
    }
}
