using System;
using System.Collections;
using Source.Enemies;
using Source.UI;
using UnityEngine;

namespace Source
{
    /// <summary>
    /// Manage Game State and Time Speed.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private int SpeedupSpeed;
        [SerializeField] private EnemySpawner enemySpawner;
        [SerializeField] private ResourceController resourceController;
        [SerializeField] private BuildingMenu buildingMenu;
        [SerializeField] private HUD hud;

        public static GameState CurrentGameState { get; private set; }
        public static int CurrentWaveCount { get; private set; }
        
        public static Action<GameState> GameStateChangedEvent = delegate {};
        public static Action<int> TimeSpeedChangedEvent = delegate {};

        private void Start()
        {
            CurrentGameState = GameState.None;
            CurrentWaveCount = 0;
            
            SetGameState(GameState.Build);

            resourceController.GameOverEvent.AddListener(OnGameOver);
            enemySpawner.WaveFinishedEvent.AddListener(OnWaveFinished);
            buildingMenu.WaveStartedEvent.AddListener(OnWaveStarted);
            hud.SpeedupEvent.AddListener(OnSpeedupEvent);
        }

        private void SetGameState(GameState newGameState)
        {
            if (CurrentGameState == newGameState)
            {
                return;
            }
            
            CurrentGameState = newGameState;

            if (CurrentGameState == GameState.Wave)
            {
                CurrentWaveCount++;
                enemySpawner.gameObject.SetActive(true);
                buildingMenu.gameObject.SetActive(false);
                hud.ActivateWaveUI(true);
                TimeSpeedChangedEvent.Invoke(1);
            }
            else if (CurrentGameState == GameState.Build)
            {
                enemySpawner.gameObject.SetActive(false);
                buildingMenu.gameObject.SetActive(true);
                hud.ActivateWaveUI(false);
            }
            else if (CurrentGameState == GameState.GameOver)
            {
                enemySpawner.gameObject.SetActive(false);
                buildingMenu.gameObject.SetActive(false);
                hud.SpawnGameOver();
                hud.ActivateWaveUI(false);
            }
            
            GameStateChangedEvent.Invoke(newGameState);
        }
        
        private void OnSpeedupEvent(bool isSpeedup)
        {
            if (isSpeedup)
            {
                TimeSpeedChangedEvent.Invoke(SpeedupSpeed);
            }
            else
            {
                TimeSpeedChangedEvent.Invoke(1);
            }
        }

        private void OnGameOver()
        {
            SetGameState(GameState.GameOver);
        }
        
        private void OnWaveFinished()
        {
            StartCoroutine(WaveFinishedDelay());
        }

        private void OnWaveStarted()
        {
            SetGameState(GameState.Wave);
        }

        private IEnumerator WaveFinishedDelay()
        {
            yield return new WaitForSeconds(1);
            if (CurrentGameState != GameState.GameOver)
            {
                SetGameState(GameState.Build);
                enemySpawner.Reset();
            }
            
        }
    }
}