using UnityEngine;

namespace Source.Enemies
{
    /// <summary>
    /// Waypoints to follow by <see cref="EnemyController"/>
    /// </summary>
    public class Waypoints : MonoBehaviour
    {
        [SerializeField] private Vector3[] points;
        public Vector3[] Points => points;
        private Vector3 currentPosition;

        private void Start()
        {
            currentPosition = transform.position;
        }

        private void OnDrawGizmos()
        {
            if (transform.hasChanged)
            {
                currentPosition = transform.position;
            }

            for (int i = 0; i < points.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawWireSphere(points[i] + currentPosition, 0.5f);

                if (i < points.Length - 1)
                {
                    Gizmos.color = Color.gray;
                    Gizmos.DrawLine(points[i] + currentPosition, points[i+1] + currentPosition);
                }
            }
        }
    }
}