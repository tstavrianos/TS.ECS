﻿using System;
using System.Collections.Generic;

namespace TS.ECS
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BaseSystem
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="messageType"></param>
        public void Subscribe(Manager manager, int messageType)
        {
            manager.Subscribe(messageType, this);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageType"></param>
        public void Subscribe(int messageType)
        {
            Subscribe(Manager.Instance, messageType);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageType"></param>
        public void Unsubscribe(int messageType)
        {
            Unsubscribe(Manager.Instance, messageType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="messageType"></param>
        public void Unsubscribe(Manager manager, int messageType)
        {
            manager.Unsubscribe(messageType, this);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="msSinceLastTick"></param>
        public abstract void Tick(Manager manager, long msSinceLastTick);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="messageType"></param>
        /// <param name="messageData"></param>
        internal void HandleMessage(object sender, int messageType, object messageData)
        {
            if (OnMessage != null)
            {
                var args = new MessageEventArgs(messageType, messageData, sender);
                OnMessage.Invoke(sender, args);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        internal Queue<MessageEventArgs> queuedMessages = new Queue<MessageEventArgs>();
        
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<MessageEventArgs> OnMessage;
        
        /// <summary>
        /// 
        /// </summary>
        public void TriggerQueuedMessages()
        {
            while(queuedMessages.Count != 0)
            {
                var args = queuedMessages.Dequeue();
                if (OnMessage != null)
                {
                    OnMessage.Invoke(args.Sender, args);
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        public void RegisterSystem(Manager manager)
        {
            manager.RegisterSystem(this);
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void RegisterSystem()
        {
            RegisterSystem(Manager.Instance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        public void UnregisterSystem(Manager manager)
        {
            manager.UnregisterSystem(this);
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void UnregisterSystem()
        {
            UnregisterSystem(Manager.Instance);
        }
    }
}