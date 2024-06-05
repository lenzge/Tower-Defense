using UnityEngine;
using UnityEngine.Tilemaps;

namespace Source.BuildingSystem
{
    /// <summary>
    /// 
    /// </summary>
    public class BuildingController : MonoBehaviour
    {
        public static BuildingController CurrentBuildingController;
        public GridLayout GridLayout;
        
        [SerializeField] private Tilemap mainTilemap;
        [SerializeField] private ResourceController resourceController;

        public GameObject[] towerPrefabs;
        public PlaceableObject objectToPlace { get; private set; }
        
        private Grid grid;
        private TileBase tileBase;

        private void Awake()
        {
            CurrentBuildingController = this;
            grid = GridLayout.gameObject.GetComponent<Grid>();
            tileBase = ScriptableObject.CreateInstance<Tile>();
            GameObject[] wayTiles = GameObject.FindGameObjectsWithTag("Way");
            foreach (var obj in wayTiles)
            {
                TakeArea(GridLayout.WorldToCell(obj.transform.position), new Vector3Int(1, 1, 1));
            }
        }

        /// <summary>
        /// Places the objectToPlace if the position is valid.
        /// </summary>
        /// <returns>true if it was placed, otherwise false</returns>
        public bool TryToPlace()
        {
            if (CanBePlaced(objectToPlace))
            {
                objectToPlace.Place();
                Vector3Int start = GridLayout.WorldToCell(objectToPlace.GetStartPosition());
                TakeArea(start, objectToPlace.Size);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Cancels the placement and destroys the current objectToPlace.
        /// </summary>
        public void CancelPlacement()
        {
            objectToPlace.GetComponent<ObjectDrag>().RemoveListener();
            Destroy(objectToPlace.gameObject);
        }

        /// <summary>
        /// Snaps a continuous Coordinate to a discrete center cell position.
        /// </summary>
        /// <param name="position">position to snap</param>
        /// <returns>snapped position</returns>
        public Vector3 SnapCoordinateToGrid(Vector3 position)
        {
            Vector3Int cellPosition = GridLayout.WorldToCell(position);
            Vector3 snappedPosition = grid.GetCellCenterWorld(cellPosition);
            snappedPosition = new Vector3(snappedPosition.x, snappedPosition.y + 0.5f, snappedPosition.z);
            return snappedPosition;
        }
        
        /// <summary>
        /// Checks if an object can be placed at the current position.
        /// </summary>
        /// <param name="placeableObject">object to check</param>
        /// <returns></returns>
        public bool CanBePlaced(PlaceableObject placeableObject)
        {
            BoundsInt area = new BoundsInt();
            area.position = GridLayout.WorldToCell(objectToPlace.GetStartPosition());
            area.size = placeableObject.Size;

            int counter = GetTilesBlock(area, mainTilemap);

            if (counter >= 1)
            {
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// Spawns the given tower if there are enough resourced to buy.
        /// </summary>
        /// <param name="tower">tower type to spawn</param>
        /// <returns>true if spawning is successful, otherwise false</returns>
        public bool TryToSpawnObject(int tower)
        {
            if (objectToPlace == null &&
                resourceController.TryToBuy(towerPrefabs[tower - 1].GetComponentInChildren<Tower.Tower>()))
            {
                InstantiateObject(towerPrefabs[tower - 1]);
                return true;
            }

            return false;
        }

        private int GetTilesBlock(BoundsInt area, Tilemap tilemap)
        {
            int counter = 0;

            foreach (Vector3Int v in area.allPositionsWithin)
            {
                Vector3Int position = new Vector3Int(v.x, v.y, 0);
                if (tilemap.GetTile(position) != null)
                {
                    counter++;
                }
            }

            return counter;
        }

        private void InstantiateObject(GameObject prefab)
        {
            Vector3 position = SnapCoordinateToGrid(Vector3.zero);

            GameObject obj = Instantiate(prefab, position, Quaternion.identity);
            objectToPlace = obj.GetComponent<PlaceableObject>();
        }

        private void TakeArea(Vector3Int start, Vector3Int size)
        {
            mainTilemap.BoxFill(start, tileBase, start.x, start.y, start.x + size.x - 1, start.y + size.y - 1);
            objectToPlace = null;
        }


    }
}
