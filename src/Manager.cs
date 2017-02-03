using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TS.ECS
{
    /// <summary>
    /// 
    /// </summary>
    public class Manager
    {
        /// <summary>
        /// Variable holding the default manager
        /// </summary>
        private static Manager instance;

        /// <summary>
        /// Map of message types to list of interested systems
        /// </summary>
        private readonly Dictionary<int, HashSet<BaseSystem>> messageSubscribers = new Dictionary<int, HashSet<BaseSystem>>();

        /// <summary>
        /// Map of entities to list of attached components
        /// </summary>
        private readonly Dictionary<Guid, Tuple<Entity, List<IComponent>>> entityComponentMap = new Dictionary<Guid, Tuple<Entity, List<IComponent>>>();

        /// <summary>
        /// List of systems to update through our Tick function
        /// </summary>
        private readonly HashSet<BaseSystem> registeredSystems = new HashSet<BaseSystem>();

        /// <summary>
        /// Stopwatch for calculating the interval between calls to Tick
        /// </summary>
        private readonly Stopwatch sw;

		/// <summary>
		/// Occurs when a new component is added.
		/// </summary>
		public event EventHandler<ComponentAddedEventArgs> OnComponentAdded;

		/// <summary>
		/// Occurs when a component is removed.
		/// </summary>
		public event EventHandler<ComponentRemovedEventArgs> OnComponentRemoved;
  
		/// <summary>
		/// Occurs when an entity is created.
		/// </summary>
		public event EventHandler<Entity> OnCreateEntity;

		/// <summary>
        /// Create a new instance of type Manager
        /// </summary>
        public Manager()
        {
            sw = new Stopwatch();
            sw.Start();
        }

        /// <summary>
        /// Singleton
        /// </summary>
        public static Manager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Manager();
                }
                return instance;
            }
        }
        
        /// <summary>
        /// Should be called in your game/processing loop. It will go through all the registered
		/// systems and call their Tick function.
        /// </summary>
        public void Tick()
		{
			sw.Stop();
			var dt = sw.ElapsedMilliseconds;
			foreach (var system in registeredSystems)
			{
				system.Tick(this, dt, false);
			}
			sw.Restart();
		}

		/// <summary>
		/// Should be called in your game/processing loop. It will go through all the registered
		/// systems and call their Tick function.
		/// </summary>
		public async Task TickAsync()
		{
			sw.Stop();
			var dt = sw.ElapsedMilliseconds;
			await Task.WhenAll(registeredSystems.Select(s => Task.Factory.StartNew(() => s.Tick(this, dt, true))).ToArray());
			sw.Restart();
		}

		/// <summary>
		/// Send a message of the specified type and with the speficied data to all subscribers
		/// </summary>
		/// <param name="sender">Who is broadcasting the message?</param>
		/// <param name="messageType">The message type that will be broadcast</param>
		/// <param name="messageData">The data to attach to the broadcasted message</param>
		public void Broadcast(object sender, int messageType, object messageData, bool async = false)
		{
			if (!messageSubscribers.ContainsKey(messageType))
			{
				return;
			}

			foreach (var system in messageSubscribers[messageType])
			{
				system.HandleMessage(new MessageEventArgs(messageType, messageData, this, sender, false));
			}
		}

		/// <summary>
		/// Send a message of the specified type and with the speficied data to all subscribers
		/// </summary>
		/// <param name="sender">Who is broadcasting the message?</param>
		/// <param name="messageType">The message type that will be broadcast</param>
		/// <param name="messageData">The data to attach to the broadcasted message</param>
		public async Task BroadcastAsync(object sender, int messageType, object messageData, bool async = false)
		{
			if (!messageSubscribers.ContainsKey(messageType))
			{
				return;
			}

			await Task.WhenAll(messageSubscribers[messageType].Select(s => Task.Factory.StartNew(() => s.HandleMessage(new MessageEventArgs(messageType, messageData, this, sender, true)))).ToArray());
		}

		/// <summary>
		/// Add the specified system to our list of subscribers for the specified message type
		/// </summary>
		/// <typeparam name="T">type extending BaseSystem</typeparam>
		/// <param name="messageType">The message type that the system will be added to</param>
		/// <param name="system">The system that will be added to our list of subscribers</param>
		public void Subscribe<T>(int messageType, T system) where T: BaseSystem
        {
            if (!messageSubscribers.ContainsKey(messageType))
            {
                messageSubscribers.Add(messageType, new HashSet<BaseSystem>());
            }
            if (!messageSubscribers[messageType].Contains(system)) 
            {
                messageSubscribers[messageType].Add(system);
            }
        }
        
		/// <summary>
		/// Remove the specified system from our list of subscribers for the specified message type
		/// </summary>
		/// <typeparam name="T">type extending BaseSystem</typeparam>
		/// <param name="messageType">The message type that the system will be removed from</param>
		/// <param name="system">The system that will be removed from our list of subscribers</param>
		public void Unsubscribe<T>(int messageType, T system) where T: BaseSystem
        {
            if (messageSubscribers.ContainsKey(messageType))
            {
                if (messageSubscribers[messageType].Contains(system))
                {
                    messageSubscribers[messageType].Remove(system);
                }
            }
        }
        
        /// <summary>
        /// Create a new Entity and trigger the OnCreateEntity event
        /// </summary>
        /// <returns>the new Entity</returns>
        public Entity CreateEntity()
        {
            var entity = new Entity();
            entityComponentMap.Add(entity.Id, new Tuple<Entity, List<IComponent>>(entity, new List<IComponent>()));
			OnCreateEntity?.Invoke(this, entity);
            return entity;
        }

        /// <summary>
        /// Create a new Entity and trigger the OnCreateEntity event
        /// </summary>
        /// <typeparam name="T">type extending Entity</typeparam>
        /// <returns>the new Entity</returns>
        public T CreateEntity<T>() where T: Entity
        {
            var entity = Activator.CreateInstance<T>();
            entityComponentMap.Add(entity.Id, new Tuple<Entity, List<IComponent>>(entity, new List<IComponent>()));
			OnCreateEntity?.Invoke(this, entity);
            return entity;
        }
        
        /// <summary>
        /// Link the entity and component together
        /// </summary>
        /// <typeparam name="E">type extending Entity</typeparam>
        /// <typeparam name="C">type implementing IComponent</typeparam>
        /// <param name="entity">The entity that the component will be added to</param>
        /// <param name="component">The component being added</param>
        public void AddComponent<E, C>(E entity, C component) where E: Entity where C: IComponent
        {
            if (entityComponentMap.ContainsKey(entity.Id))
            {
                entityComponentMap[entity.Id].Item2.Add(component);
				OnComponentAdded?.Invoke(this, new ComponentAddedEventArgs(entity, component));
            }
        }
        
        /// <summary>
        /// Unlink the entity and component
        /// </summary>
        /// <typeparam name="E">type extending Entity</typeparam>
        /// <typeparam name="C">type implementing IComponent</typeparam>
        /// <param name="entity">The entity that the component will be removed from</param>
        /// <param name="component">The component being removed</param>
        public void RemoveComponent<E, C>(E entity, C component) where E: Entity where C: IComponent
        {
            if (entityComponentMap.ContainsKey(entity.Id))
            {
				if (entityComponentMap[entity.Id].Item2.Contains(component))
				{
					entityComponentMap[entity.Id].Item2.Remove(component);
					OnComponentRemoved?.Invoke(this, new ComponentRemovedEventArgs(entity, component));
				}
            }
        }
        
        /// <summary>
        /// Find the first component of type C belonging to the specified entity
        /// </summary>
        /// <typeparam name="E">type extending Entity</typeparam>
        /// <typeparam name="C">type implementing IComponent</typeparam>
        /// <param name="entity">the entity that the component belongs to</param>
        /// <returns>The first component that matches the requested type or Null if none are found</returns>
        public C GetComponent<E, C>(E entity) where E: Entity where C: IComponent
        {
            return (C)GetComponent(entity, typeof(C));
        }

        /// <summary>
        /// Find the first component of the specified type belonging to the specified entity
        /// </summary>
        /// <typeparam name="E">type extending Entity</typeparam>
        /// <param name="entity">the entity that the component belongs to</param>
        /// <param name="componentType">type implementing IComponent</param>
        /// <returns>The first component that matches the requested type or Null if none are found</returns>
        internal IComponent GetComponent<E>(E entity, Type componentType) where E: Entity
        {
			if (!componentType.GetInterfaces().Any(t => t == typeof(IComponent)))
			{
				throw new NotSupportedException(nameof(componentType));
			}

			if (entityComponentMap.ContainsKey(entity.Id))
            {
                return entityComponentMap[entity.Id].Item2.FirstOrDefault(c => c.GetType() == componentType);
            }
            return null;
        }

		/// <summary>
		/// Find all the components of type C belonging to the specified entity
		/// </summary>
		/// <typeparam name="E">type extending Entity</typeparam>
		/// <typeparam name="C">type implementing IComponent</typeparam>
		/// <param name="entity">the entity that the components belong to</param>
		/// <returns>All the components that match the requested type or and empty List if none 
		/// are found</returns>
		public IEnumerable<C> GetComponents<E, C>(E entity) where E: Entity where C: IComponent
        {
            if (entityComponentMap.ContainsKey(entity.Id))
            {
                return entityComponentMap[entity.Id].Item2.Where(c => c is C).Cast<C>();
            }
            return new List<C>();
        }

		/// <summary>
		/// Find all the components of the specified typeparamref name=""/> belonging to the 
		/// specified entity
		/// </summary>
		/// <typeparam name="E">type extending Entity</typeparam>
		/// <param name="entity">the entity that the components belong to</param>
		/// <param name="componentType">type implementing IComponent</param>
		/// <returns>All the components that match the requested type or and empty List if none 
		/// are found</returns>
		internal IEnumerable<IComponent> GetComponents<E>(E entity, Type componentType) where E: Entity
        {
			if (!componentType.GetInterfaces().Any(t => t == typeof(IComponent)))
			{
				throw new NotSupportedException(nameof(componentType));
			}
   
			if (entityComponentMap.ContainsKey(entity.Id))
            {
                return entityComponentMap[entity.Id].Item2.Where(c => c.GetType() == componentType);
            }
            return new List<IComponent>();
        }
        
        /// <summary>
        /// Find all the components belonging to the specified entity
        /// </summary>
        /// <typeparam name="E">type extending Entity</typeparam>
        /// <param name="entity">The entity that we are interested in</param>
        /// <returns>All the components linked to the specified entity</returns>
        public IEnumerable<IComponent> GetAllComponents<E>(E entity) where E: Entity
        {
            if (entityComponentMap.ContainsKey(entity.Id))
            {
                return entityComponentMap[entity.Id].Item2.AsReadOnly();
            }
            return new List<IComponent>();
        }
        
        /// <summary>
        /// Find all entities that have at least one component of type C
        /// </summary>
        /// <typeparam name="E">type extending Entity</typeparam>
        /// <typeparam name="C">type implementing IComponent</typeparam>
        /// <returns>All the entities that have at least one component of the requested type</returns>
        public IEnumerable<E> EntitiesWithComponent<E, C>() where E : Entity where C : IComponent
		{
			return entityComponentMap.Values.Where(x => x.Item2.Any(c => c is C)).Select(x => x.Item1).Cast<E>();
		}

		/// <summary>
		/// Add the specified system to our list of systems updated with Tick
		/// </summary>
		/// <typeparam name="S">type extending BaseSystem</typeparam>
		/// <param name="system">The system added to our list</param>
		public void RegisterSystem<S>(S system) where S: BaseSystem
        {
            if (!registeredSystems.Contains(system))
            {
                registeredSystems.Add(system);
            }
        }
        
		/// <summary>
		/// Remove the specified system from our list of systems updated with Tick
		/// </summary>
		/// <typeparam name="S">type extending BaseSystem</typeparam>
		/// <param name="system">The system removed from our list</param>
        public void UnregisterSystem<S>(S system) where S: BaseSystem
        {
            if (registeredSystems.Contains(system))
            {
                registeredSystems.Remove(system);
            }
        }
    }
}
