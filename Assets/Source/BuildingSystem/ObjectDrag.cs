using UnityEngine;

namespace Source.BuildingSystem
{
    /// <summary>
    /// Add to <see cref="Tower"/>, so they can be placed by the <see cref="BuildingSystem"/>.
    /// </summary>
    public class ObjectDrag : MonoBehaviour
    {
        [SerializeField] private Material wrongPosition;
        [SerializeField] private Material rightPosition;
        [SerializeField] private PlaceableObject placeableObject;
        [SerializeField] private SpriteRenderer positionIndicator;
        [SerializeField] private SpriteRenderer rangeIndicator;
        
        private Vector3 offset;
        private Vector3 oldGridPosition;

        private void Start()
        {
            oldGridPosition = transform.position;
            CheckColor();
            placeableObject.placedEvent.AddListener(OnPlacedEvent);
            GameManager.GameStateChangedEvent += OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState gameState)
        {
            if (gameState == GameState.Build)
            {
                rangeIndicator.gameObject.SetActive(true);
            }
            else if (gameState == GameState.Wave)
            {
                rangeIndicator.gameObject.SetActive(false);
            }
            else if (gameState == GameState.GameOver)
            {
                RemoveListener();
            }
        }

        private void OnPlacedEvent()
        {
            positionIndicator.color = Color.clear;
            placeableObject.placedEvent.RemoveListener(OnPlacedEvent);
        }

        private void OnMouseDown()
        {
            offset = transform.position - GetMouseWorldPosition();
        }

        private void OnMouseDrag()
        {
            Vector3 position = GetMouseWorldPosition() + offset;
            transform.position = BuildingController.CurrentBuildingController.SnapCoordinateToGrid(position);
            if (transform.position != oldGridPosition)
            {
                CheckColor();
            }

            oldGridPosition = transform.position;
        }

        private void CheckColor()
        {
            if (BuildingController.CurrentBuildingController.CanBePlaced(placeableObject))
            {
                positionIndicator.color = rightPosition.color;
            }
            else
            {
                positionIndicator.color = wrongPosition.color;
            }
        }

        public void RemoveListener()
        {
            GameManager.GameStateChangedEvent -= OnGameStateChanged;
        }
        
        private static Vector3 GetMouseWorldPosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                return raycastHit.point;
            }
            else
            {
                return Vector3.zero;
            }
        }
    }
}
