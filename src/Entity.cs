using System;
using System.Collections.Generic;
using System.Linq;

namespace TS.ECS
{
    /// <summary>
    /// Main entity class. Ideally all data should be delegated to components and all logic to 
	/// systems.
    /// </summary>
    public class Entity
    {
        /// <summary>
        /// Internal id used to reference this entity. Used to speed-up dictionary/hash lookups.
        /// </summary>
        internal Guid Id = Guid.NewGuid();
        
        /// <summary>
        /// Adds the specified component to this entity using the specified manager.
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <param name="manager">The manager that handles the actual linking between this entity 
		/// and the component</param>
        /// <param name="component">The component that you are adding to this entity</param>
        public void AddComponent<C>(Manager manager, C component) where C: IComponent
        {
            manager.AddComponent(this, component);
        }
        
        /// <summary>
        /// Adds the specified component to this entity using the default (singleton) manager.
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <param name="component">The component that you are adding to this entity</param>
        public void AddComponent<C>(C component) where C: IComponent
        {
            AddComponent(Manager.Instance, component);
        }

        /// <summary>
        /// Removes the specified component from this entity using the specified manager. No 
		/// exception is thrown if the component was never added to this entity originally.
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <param name="manager">The manager that handles the actual removal of the component from 
		/// this entity</param>
        /// <param name="component">The component that you are removing from this entity</param>
        public void RemoveComponent<C>(Manager manager, C component) where C: IComponent
        {
            manager.RemoveComponent(this, component);
        }
        
        /// <summary>
        /// Removes the specified component from this entity using the default (singleton) manager. 
		/// No exception is thrown if the component was never added to this entity originally.
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <param name="component">The component that you are removing from this entity</param>
        public void RemoveComponent<C>(C component) where C: IComponent
        {
            RemoveComponent(Manager.Instance, component);
        }
        
        /// <summary>
        /// Grab the first component, linked to this entity through the specified manager, which is 
		/// of type C.
        /// </summary>
        /// <typeparam name="C">Generic type which inherits from IComponent</typeparam>
        /// <param name="manager">The manager through which the entity and component were linked</param>
        /// <returns>The first component of type C which was linked through the specified manager</returns>
        public C GetComponent<C>(Manager manager) where C: IComponent
        {
            return (C)manager.GetComponent(this, typeof(C));
        }
        
        /// <summary>
		/// Grab the first component, linked to this entity through the default (singleton) manager,
		/// which is of type C.
        /// </summary>
        /// <typeparam name="C">Generic type which inherits from IComponent</typeparam>
        /// <returns>The first component of type C which was linked through the default (singleton) 
		/// manager</returns>
        public C GetComponent<C>() where C: IComponent
        {
            return GetComponent<C>(Manager.Instance);
        }
        
        /// <summary>
        /// Grab all the components, linked to this entity through the specified manager, which are 
		/// of type C.
        /// </summary>
        /// <typeparam name="C">Generic type which inherits from IComponent</typeparam>
        /// <param name="manager">The manager through which the entity and components were linked</param>
        /// <returns>The components of type C which were linked through the specified manager</returns>
        public IEnumerable<C> GetComponents<C>(Manager manager) where C: IComponent
        {
            return manager.GetComponents(this, typeof(C)).Cast<C>();
        }

		/// <summary>
		/// Grab all the components, linked to this entity through the default (singleton) manager, 
		/// which are of type C.
		/// </summary>
		/// <typeparam name="C">Generic type which inherits from IComponent</typeparam>
		/// <returns>The components of type C which were linked through the default (singleton) 
		/// manager</returns>
		public IEnumerable<C> GetComponents<C>() where C: IComponent
        {
            return GetComponents<C>(Manager.Instance);
        }

        /// <summary>
        /// Grab all the components linked to this entity through the specified manager
        /// </summary>
        /// <param name="manager">The manager which linked the entity with the components</param>
        /// <returns>The components linked to this entity through the specified manager</returns>
        public IEnumerable<IComponent> GetAllComponents(Manager manager)
        {
            return manager.GetAllComponents(this);
        }
        
        /// <summary>
        /// Grab all the components linked to this entity through the default (singleton) manager
        /// </summary>
        /// <returns>The components linked to this entity through the default (singleton) manager</returns>
        public IEnumerable<IComponent> GetAllComponents()
        {
            return GetAllComponents(Manager.Instance);
        }
    }
}
