using UnityEngine;

namespace Source.Util
{
    /// <summary>
    /// Base Class for all entities that are time dependent -> all entities that are used during wave state and have
    /// speed or delays.
    /// </summary>
    public abstract class TimeDependentBehaviour : MonoBehaviour
    {
        public virtual void Awake()
        {
            GameManager.TimeSpeedChangedEvent += OnTimeSpeedChanged;
        }

        /// <summary>
        /// Adjusts values that are time dependent (like speed, delays and timer)
        /// </summary>
        /// <param name="timeScale">new time scale, changed through UI Button</param>
        protected abstract void OnTimeSpeedChanged(int timeScale);
    }
}