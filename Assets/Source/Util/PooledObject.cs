using UnityEngine;
using UnityEngine.Events;

namespace Source.Util
{
    /// <summary>
    /// Base class for all objects that should be pooled by <see cref="ObjectPooler"/>. 
    /// </summary>
    public abstract class PooledObject : TimeDependentBehaviour
    {
        [HideInInspector] public UnityEvent<PooledObject> DespawnedEvent;
        
        /// <summary>
        /// Invoked by the ObjectPooler whenever the object spawns from the pool. Use for Initialisations and stuff.
        /// </summary>
        public virtual void OnSpawnInstanceFromPool(){}

        /// <summary>
        /// Invoked by the ObjectPooler whenever the object returns to the pool. Use for Resets and stuff.
        /// </summary>
        public virtual void OnReturnToPool()
        {
            DespawnedEvent.Invoke(this);
        }
    }
}