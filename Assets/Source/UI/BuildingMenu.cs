using Source.BuildingSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Source.UI
{
    public class BuildingMenu : MonoBehaviour
    {
        [SerializeField] private BuildingController buildingController;
        [SerializeField] private ResourceController resourceController;
        
        private VisualElement root;
        private Button cancelButton;
        private Button placeButton;
        private Button startButton;
        private Button[] buyButtons = new Button[3];
        private Button constantQuitButton;

        [HideInInspector] public UnityEvent WaveStartedEvent;
        
        private void OnEnable()
        {
            root = GetComponent<UIDocument>().rootVisualElement;

            Button tower1Button = root.Q<Button>("Tower1");
            tower1Button.clicked += () => OnTowerButton(1);
            buyButtons[0] = tower1Button;

            Button tower2Button = root.Q<Button>("Tower2");
            tower2Button.clicked += () => OnTowerButton(2);
            buyButtons[1] = tower2Button;
            
            Button tower3Button = root.Q<Button>("Tower3");
            tower3Button.clicked += () => OnTowerButton(3);
            buyButtons[2] = tower3Button;
            
            startButton = root.Q<Button>("Start");
            startButton.clicked += OnStartButton;
            
            cancelButton = root.Q<Button>("Cancel");
            cancelButton.clicked += OnCancelButton;

            placeButton = root.Q<Button>("Place");
            placeButton.clicked += OnPlaceButton;
            
            constantQuitButton = root.Q<Button>("ConstantQuit");
            constantQuitButton.clicked += OnQuitButton;
            
            ActivatePlacementButtons(false);
            
            resourceController.UpdateCoinsEvent.AddListener(OnUpdateCoins);
           
        }

        private void OnQuitButton()
        {
            Application.Quit();
        }

        private void OnUpdateCoins(int arg0)
        {
            CheckBuyButtons();
        }

        private void CheckBuyButtons()
        {
            for (int i = 0; i < buildingController.towerPrefabs.Length; i++)
            {
                if (!resourceController.CanBuy(buildingController.towerPrefabs[i]))
                {
                    buyButtons[i].SetEnabled(false);
                }
                else
                {
                    buyButtons[i].SetEnabled(true);
                }
            }
        }

        private void OnPlaceButton()
        {
            bool tryToPlace = buildingController.TryToPlace();

            if (tryToPlace)
            {
                ActivatePlacementButtons(false);
            }
        }

        private void OnCancelButton()
        {
            resourceController.RestoreCoins(buildingController.objectToPlace);
            buildingController.CancelPlacement();
            ActivatePlacementButtons(false);
        }

        private void OnStartButton()
        {
            WaveStartedEvent.Invoke();
        }

        private void OnTowerButton(int tower)
        {
            bool tryToSpawn = buildingController.TryToSpawnObject(tower);
            if (tryToSpawn)
            {
                ActivatePlacementButtons(true);
            }
        }

        private void ActivatePlacementButtons(bool activate)
        {
            cancelButton.visible = activate;
            placeButton.visible = activate;
            startButton.SetEnabled(!activate);
        }
    }
}