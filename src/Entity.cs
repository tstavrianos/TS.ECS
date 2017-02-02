using System;
using System.Collections.Generic;
using System.Linq;

namespace TS.ECS
{
    /// <summary>
    /// 
    /// </summary>
    public class Entity
    {
        /// <summary>
        /// 
        /// </summary>
        internal Guid Id;
        
        /// <summary>
        /// 
        /// </summary>
        public Entity()
        {
            Id = Guid.NewGuid();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <param name="manager"></param>
        /// <param name="component"></param>
        public void AddComponent<C>(Manager manager, C component) where C: Component
        {
            manager.AddComponent(this, component);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <param name="component"></param>
        public void AddComponent<C>(C component) where C: Component
        {
            AddComponent(Manager.Instance, component);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <param name="manager"></param>
        /// <param name="component"></param>
        public void RemoveComponent<C>(Manager manager, C component) where C: Component
        {
            manager.RemoveComponent(this, component);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <param name="component"></param>
        public void RemoveComponent<C>(C component) where C: Component
        {
            RemoveComponent(Manager.Instance, component);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <param name="manager"></param>
        /// <returns></returns>
        public C GetComponent<C>(Manager manager) where C: Component
        {
            return (C)manager.GetComponent(this, typeof(C));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <returns></returns>
        public C GetComponent<C>() where C: Component
        {
            return GetComponent<C>(Manager.Instance);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <param name="manager"></param>
        /// <returns></returns>
        public IEnumerable<C> GetComponents<C>(Manager manager) where C: Component
        {
            return manager.GetComponents(this, typeof(C)).Cast<C>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <returns></returns>
        public IEnumerable<C> GetComponents<C>() where C: Component
        {
            return GetComponents<C>(Manager.Instance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public IEnumerable<Component> GetAllComponents(Manager manager)
        {
            return manager.GetAllComponents(this);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <returns></returns>
        public IEnumerable<Component> GetAllComponents<C>()
        {
            return GetAllComponents(Manager.Instance);
        }
    }
}
