using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Source.UI
{
    public class HUD : MonoBehaviour
    {
        [SerializeField] private ResourceController resourceController;
        
        private VisualElement root;
        private Button speedButton;
        private Label coinLabel ;
        private Label heartLabel;
        private Label waveLabel;
        private Button constantQuitButton;
        private VisualElement GameOverPanel;
        private bool isSpeedup;
        
        [HideInInspector] public UnityEvent<bool> SpeedupEvent;

        private void OnEnable()
        {
            root = GetComponent<UIDocument>().rootVisualElement;

            speedButton = root.Q<Button>("Speed");
            speedButton.clicked += OnSpeedButton;

            constantQuitButton = root.Q<Button>("ConstantQuit");
            constantQuitButton.clicked += OnQuitButton;

            coinLabel = root.Q<Label>("Coins");
            heartLabel = root.Q<Label>("Hearts");
            waveLabel = root.Q<Label>("WaveCount");

            GameOverPanel = root.Q<VisualElement>("GameOver");
            Button restartButton = root.Q<Button>("Restart");
            restartButton.clicked += OnRestartButton;
            Button quitButton = root.Q<Button>("Quit");
            quitButton.clicked += OnQuitButton;
            GameOverPanel.visible = false;
            
            resourceController.UpdateCoinsEvent.AddListener(OnUpdateCoins);
            resourceController.UpdateHeartsEvent.AddListener(OnUpdateHearts);

        }

        private void OnQuitButton()
        {
            Application.Quit();
        }

        private void OnRestartButton()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void OnSpeedButton()
        {
            isSpeedup = !isSpeedup;
            SpeedupEvent.Invoke(isSpeedup);
            UpdateSpeedupButton();
        }

        private void OnUpdateCoins(int coins)
        {
            coinLabel.text = $"$ {coins}";
        }

        private void OnUpdateHearts(int hearts)
        {
            heartLabel.text = $"<3 {hearts}";
        }

        public void SpawnGameOver()
        {
            speedButton.visible = false;
            GameOverPanel.visible = true;
        }

        public void ActivateWaveUI(bool activate)
        {
            constantQuitButton.visible = activate;
            waveLabel.visible = activate;
            waveLabel.text = $"W {GameManager.CurrentWaveCount}";
            speedButton.visible = activate;
            isSpeedup = false;
            UpdateSpeedupButton();
        }

        private void UpdateSpeedupButton()
        {
            if (isSpeedup)
            {
                speedButton.text = ">";
            }
            else
            {
                speedButton.text = ">>";
            }
        }
    }
}