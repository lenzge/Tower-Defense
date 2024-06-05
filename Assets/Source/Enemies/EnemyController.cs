using System;
using Source.Util;
using UnityEngine;

namespace Source.Enemies
{
    /// <summary>
    /// Controller for an Enemy. Spawn prefabs via <see cref="EnemySpawner"/>. Always add an kinematic Rigidbody to
    /// allow collision detection with projectiles.
    /// </summary>
    public class EnemyController : PooledObject
    {
        [SerializeField] private float initialHealth;
        [SerializeField] private float relativeMoveSpeed;
        [SerializeField] public Waypoints waypoints;
        
        // static events are listened to by the resourceManager
        public static Action EnemyDiedEvent = delegate{};
        public static Action EnemyFinishedEvent = delegate{};

        private float currentHealth;
        private Vector3 currentPointPosition;
        private int currentWaypointIndex;
        private float currentMoveSpeed;
        private bool isGameOver;

        private void Start()
        {
            GameManager.GameStateChangedEvent += OnGameStateChanged;
            Reset();
        }

        private void Update()
        {
            if (isGameOver) return;
            
            Move();

            if (CurrentPointPositionReached())
            {
                UpdateCurrentPointIndex();
            }
        }
        
        /// <summary>
        /// Deals damage to the enemy. Enemy will die if it's current health is empty.
        /// </summary>
        /// <param name="damage">damage to deal</param>
        public void ReceiveDamage(float damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Die();
            }
        }

        /// <summary>
        /// Resets the Parameter of the instance
        /// </summary>
        public override void OnReturnToPool()
        {
            base.OnReturnToPool();
            Reset();
        }

        private void Move()
        {
            transform.position =
                Vector3.MoveTowards(transform.position, currentPointPosition, currentMoveSpeed * Time.deltaTime);
        }

        private void UpdateCurrentPointIndex()
        {
            if (currentWaypointIndex < waypoints.Points.Length - 1)
            {
                currentWaypointIndex++;
                currentPointPosition = waypoints.Points[currentWaypointIndex];
            }
            else
            {
                EndPointReached();
            }
        }

        private void EndPointReached()
        {
            EnemyFinishedEvent.Invoke();
            ObjectPooler.ReturnToPool(this);
        }

        private bool CurrentPointPositionReached()
        {
            float distanceToNextPointPosition = (transform.position - currentPointPosition).magnitude;
            if (distanceToNextPointPosition < 0.1f)
            {
                return true;
            }
            
            return false;
        }

        private void Reset()
        {
            currentHealth = initialHealth;
            currentWaypointIndex = 0;
            currentPointPosition = waypoints.Points[currentWaypointIndex];
            transform.position = currentPointPosition;
        }

        private void Die()
        {
            EnemyDiedEvent.Invoke();
            ObjectPooler.ReturnToPool(this);
        }

        protected override void OnTimeSpeedChanged(int timeScale)
        {
            currentMoveSpeed = relativeMoveSpeed * timeScale;
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