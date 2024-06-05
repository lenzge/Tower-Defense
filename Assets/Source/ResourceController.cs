using Source.BuildingSystem;
using Source.Enemies;
using UnityEngine;
using UnityEngine.Events;

namespace Source
{
    /// <summary>
    /// The resource Controller stores all resources (Hearts and Coins). Is informed by the static events of
    /// the <see cref="EnemyController"/>.
    /// </summary>
    public class ResourceController : MonoBehaviour
    {
        [SerializeField] private int initialCoins;
        [SerializeField] private int initialHearts;
        private int currentCoins;
        private int currentHearts;

        [HideInInspector] public UnityEvent GameOverEvent;
        [HideInInspector] public UnityEvent<int> UpdateCoinsEvent;
        [HideInInspector] public UnityEvent<int> UpdateHeartsEvent;

        private void Start()
        {
            currentCoins = initialCoins;
            currentHearts = initialHearts;
            UpdateCoinsEvent.Invoke(currentCoins);
            UpdateHeartsEvent.Invoke(currentHearts);

            EnemyController.EnemyDiedEvent += OnEnemyDied;
            EnemyController.EnemyFinishedEvent += OnEnemyFinished;
        }
        
        /// <summary>
        /// Buys the given tower if there are enough coins.
        /// </summary>
        /// <param name="tower">tower to buy</param>
        /// <returns>true if successful, otherwise false</returns>
        public bool TryToBuy(Source.Tower.Tower tower)
        {
            if (currentCoins >= tower.Price)
            {
                currentCoins -= tower.Price;
                UpdateCoinsEvent.Invoke(currentCoins);
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// Restore the price of a given tower. Use if the placement is cancelled.
        /// </summary>
        /// <param name="tower">tower to restore the price</param>
        public void RestoreCoins(PlaceableObject tower)
        {
            currentCoins += tower.GetComponentInChildren<Tower.Tower>().Price;
            UpdateCoinsEvent.Invoke(currentCoins);
        }

        /// <summary>
        /// Checks if a tower could be bought. Use for button indication.
        /// </summary>
        /// <param name="tower">tower to check</param>
        /// <returns>true if tower could be bought, otherwise false</returns>
        public bool CanBuy(GameObject tower)
        {
            if (currentCoins >= tower.GetComponentInChildren<Tower.Tower>().Price)
            {
                return true;
            }

            return false;
        }
        
        private void OnEnemyDied()
        {
            currentCoins += 1;
            UpdateCoinsEvent.Invoke(currentCoins);
        }

        private void OnEnemyFinished()
        {
            currentHearts--;
            UpdateHeartsEvent.Invoke(currentHearts);

            if (currentHearts == 0)
            {
                GameOverEvent.Invoke();
            }
        }
    }
}