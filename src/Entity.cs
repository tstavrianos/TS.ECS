using System;
using System.Collections.Generic;
using System.Linq;

namespace TS.ECS
{
    /// <summary>
    /// Main entity class. Ideally all data should be delegated to components and all logic to 
	/// systems.
    /// </summary>
    public class Entity : IDisposable
    {
        /// <summary>
        /// Internal id used to reference this entity. Used to speed-up dictionary/hash lookups.
        /// </summary>
        internal readonly Guid Id;
        
		/// <summary>
		/// Manager that instatiated the Entity
		/// </summary>
		/// <value>the Manager that created this object</value>
		public Manager Parent { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TS.ECS.Entity"/> class.
		/// </summary>
		/// <param name="parent">Manager that called this function</param>
		internal Entity(Manager parent)
		{
			Parent = parent;
			Id = Guid.NewGuid();
		}

        /// <summary>
        /// Adds the specified component to this entity.
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <param name="component">The component that you are adding to this entity</param>
        public void AddComponent<C>(C component) where C: IComponent
        {
            Parent?.AddComponent(this, component);
        }
        
        /// <summary>
        /// Removes the specified component from this entity. No exception is thrown if the 
		/// component was never added to this entity originally.
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <param name="component">The component that you are removing from this entity</param>
        public void RemoveComponent<C>(C component) where C: IComponent
        {
            Parent?.RemoveComponent(this, component);
        }
        
        /// <summary>
        /// Grab the first component of type C, linked to this entity.
        /// </summary>
        /// <typeparam name="C">Generic type which inherits from IComponent</typeparam>
        /// <returns>The first component of type C which was linked to this entity</returns>
        public C GetComponent<C>() where C: IComponent
        {
            return (C)Parent?.GetComponent(this, typeof(C));
        }
        
        /// <summary>
        /// Grab all the components of type C, linked to this entity.
        /// </summary>
        /// <typeparam name="C">Generic type which inherits from IComponent</typeparam>
        /// <returns>The components of type C which were linked to this entity</returns>
        public IEnumerable<C> GetComponents<C>() where C: IComponent
        {
            return Parent?.GetComponents(this, typeof(C)).Cast<C>();
        }

        /// <summary>
        /// Grab all the components linked to this entity.
        /// </summary>
        /// <returns>The components linked to this entity</returns>
        public IEnumerable<IComponent> GetAllComponents()
        {
            return Parent?.GetAllComponents(this);
        }

		/// <summary>
		/// Public implementation of Dispose pattern callable by consumers
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			Parent?.DestroyedEntity(Id);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Protected implementation of Dispose pattern
		/// </summary>
		/// <param name="disposing">Do we have any managed objects to free?</param>
		protected virtual void Dispose(bool disposing)
		{
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the <see cref="T:TS.ECS.Entity"/> is
		/// reclaimed by garbage collection.
		/// </summary>
		~Entity()
		{
			Dispose(false);
			Parent?.DestroyedEntity(Id);
		}
	}
}
