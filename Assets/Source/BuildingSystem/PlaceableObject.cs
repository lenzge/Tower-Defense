using UnityEngine;
using UnityEngine.Events;

namespace Source.BuildingSystem
{
    /// <summary>
    /// Makes an Object placeable by the <see cref="BuildingController"/>
    /// </summary>
    public class PlaceableObject : MonoBehaviour
    {
        [SerializeField] private BoxCollider collider;

        public Vector3Int Size;

        [HideInInspector] public UnityEvent placedEvent;

        /// <summary>
        /// Get Position to move from there.
        /// </summary>
        /// <returns>current object position</returns>
        public Vector3 GetStartPosition()
        {
            return transform.position;
        }

        /// <summary>
        /// Places the object and removes the objectDrag, so it can't be moved anymore.
        /// </summary>
        public void Place()
        {
            ObjectDrag objectDrag = gameObject.GetComponent<ObjectDrag>();
            Destroy(objectDrag);
            collider.enabled = false;
            
            placedEvent.Invoke();
        }

    }
}