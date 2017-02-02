﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace TS.ECS
{
    /// <summary>
    /// 
    /// </summary>
    public class Manager
    {
        /// <summary>
        /// 
        /// </summary>
        private static Manager instance;

        /// <summary>
        /// 
        /// </summary>
        private readonly Dictionary<int, HashSet<BaseSystem>> messageSubscribers = new Dictionary<int, HashSet<BaseSystem>>();

        /// <summary>
        /// 
        /// </summary>
        private readonly Dictionary<Guid, Tuple<Entity, List<Component>>> entityComponentMap = new Dictionary<Guid, Tuple<Entity, List<Component>>>();

        /// <summary>
        /// 
        /// </summary>
        private readonly HashSet<BaseSystem> registeredSystems = new HashSet<BaseSystem>();

        /// <summary>
        /// 
        /// </summary>
        private Stopwatch sw;

        /// <summary>
        /// 
        /// </summary>
        public Manager()
        {
            sw = new Stopwatch();
            sw.Start();
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageType"></param>
        /// <param name="system"></param>
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
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageType"></param>
        /// <param name="system"></param>
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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="messageType"></param>
        /// <param name="messageData"></param>
        public void Broadcast(object sender, int messageType, object messageData)
        {
            if (!messageSubscribers.ContainsKey(messageType))
            {
                return;
            }
            
            foreach (var system in messageSubscribers[messageType]) 
            {
                system.EnqueueMessage(new MessageEventArgs(messageType, messageData, this, sender));
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Entity CreateEntity()
        {
            var entity = new Entity();
            entityComponentMap.Add(entity.Id, new Tuple<Entity, List<Component>>(entity, new List<Component>()));
            return entity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T CreateEntity<T>() where T: Entity
        {
            var entity = Activator.CreateInstance<T>();
            entityComponentMap.Add(entity.Id, new Tuple<Entity, List<Component>>(entity, new List<Component>()));
            return entity;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="entity"></param>
        /// <param name="component"></param>
        public void AddComponent<E, C>(E entity, C component) where E: Entity where C: Component
        {
            if (entityComponentMap.ContainsKey(entity.Id))
            {
                entityComponentMap[entity.Id].Item2.Add(component);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="entity"></param>
        /// <param name="component"></param>
        public void RemoveComponent<E, C>(E entity, C component) where E: Entity where C: Component
        {
            if (entityComponentMap.ContainsKey(entity.Id))
            {
                if (entityComponentMap[entity.Id].Item2.Contains(component))
                    entityComponentMap[entity.Id].Item2.Remove(component);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <returns></returns>
        public IEnumerable<E> EntitiesWithComponent<E, C>() where E: Entity where C: Component
        {
            return entityComponentMap.Values.Where(x => x.Item2.Any(c => c is C)).Select(x => x.Item1).Cast<E>();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public C GetComponent<E, C>(E entity) where E: Entity where C: Component
        {
            return (C)GetComponent<E>(entity, typeof(C));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="entity"></param>
        /// <param name="componentType"></param>
        /// <returns></returns>
        internal Component GetComponent<E>(E entity, Type componentType) where E: Entity
        {
            if (entityComponentMap.ContainsKey(entity.Id))
            {
                return entityComponentMap[entity.Id].Item2.FirstOrDefault(c => c.GetType() == componentType);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IEnumerable<C> GetComponents<E, C>(E entity) where E: Entity where C: Component
        {
            if (entityComponentMap.ContainsKey(entity.Id))
            {
                return entityComponentMap[entity.Id].Item2.Where(c => c is C).Cast<C>();
            }
            return new List<C>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="entity"></param>
        /// <param name="componentType"></param>
        /// <returns></returns>
        internal IEnumerable<Component> GetComponents<E>(E entity, Type componentType) where E: Entity
        {
            if (entityComponentMap.ContainsKey(entity.Id))
            {
                return entityComponentMap[entity.Id].Item2.Where(c => c.GetType() == componentType);
            }
            return new List<Component>();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IEnumerable<Component> GetAllComponents<E>(E entity) where E: Entity
        {
            if (entityComponentMap.ContainsKey(entity.Id))
            {
                return entityComponentMap[entity.Id].Item2.AsReadOnly();
            }
            return new List<Component>();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="system"></param>
        public void RegisterSystem<S>(S system) where S: BaseSystem
        {
            if (!registeredSystems.Contains(system))
            {
                registeredSystems.Add(system);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="system"></param>
        public void UnregisterSystem<S>(S system) where S: BaseSystem
        {
            if (registeredSystems.Contains(system))
            {
                registeredSystems.Remove(system);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void Tick()
        {
            sw.Stop();
            var dt = sw.ElapsedMilliseconds;
            foreach (var system in registeredSystems) 
            {
                system.Tick(this, dt);
            }
            sw.Restart();
        }
    }
}
