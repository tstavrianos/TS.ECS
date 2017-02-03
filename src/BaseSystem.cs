using System;

namespace TS.ECS
{
    /// <summary>
    /// Base system. All systems should extend this class.
    /// </summary>
    public abstract class BaseSystem
    {
        /// <summary>
        /// Event triggered when a message type that we are subscribed to gets broadcast
        /// </summary>
        public event EventHandler<MessageEventArgs> OnMessage;

		/// <summary>
		/// Called by each manager we are registered to, everytime their Tick function is called
		/// </summary>
		/// <param name="manager">The manager whose Tick function was triggered</param>
		/// <param name="msSinceLastTick">The time (is milliseconds) since the last call by this 
		/// manager</param>
		/// <param name="async">Was this called through a Task?</param>
		public abstract void Tick(Manager manager, long msSinceLastTick, bool async = false);

        /// <summary>
        /// Internal function to trigger the OnMessage event.
        /// </summary>
        /// <param name="args"></param>
        internal void HandleMessage(MessageEventArgs args)
		{
			OnMessage?.Invoke(args.Sender, args);
		}

		/// <summary>
		/// Add us to the specified manager's list of subscribers for the specified message type
		/// </summary>
		/// <param name="manager">The manager that holds the subscriptions</param>
		/// <param name="messageType">The message type that we are interested in</param>
		public void Subscribe(Manager manager, int messageType)
        {
            manager.Subscribe(messageType, this);
        }
        
        /// <summary>
		/// Add us to the default (singleton) manager's list of subscribers for the specified 
		/// message type
        /// </summary>
        /// <param name="messageType">The message type that we are interested in</param>
        public void Subscribe(int messageType)
        {
            Subscribe(Manager.Instance, messageType);
        }
        
        /// <summary>
		/// Remove us from the specified manager's list of subscribers for the specified message 
		/// type
		/// </summary>
		/// <param name="manager">The manager that holds the subscriptions</param>
		/// <param name="messageType">The message type that we are not interested in anymore</param>
        public void Unsubscribe(Manager manager, int messageType)
        {
            manager.Unsubscribe(messageType, this);
        }
        
        /// <summary>
		/// Remove us from the default (singleton) manager's list of subscribers for the specified 
		/// message type
		/// </summary>
		/// <param name="messageType">The message type that we are not interested in anymore</param>
        public void Unsubscribe(int messageType)
		{
			Unsubscribe(Manager.Instance, messageType);
		}

		/// <summary>
		/// Add us to the specified mananger's Tick function
		/// </summary>
		/// <param name="manager">The manager we want to be added to</param>
		public void RegisterSystem(Manager manager)
        {
            manager.RegisterSystem(this);
        }
        
        /// <summary>
        /// Add us to the default (singleton) mananger's Tick function
        /// </summary>
        public void RegisterSystem()
        {
            RegisterSystem(Manager.Instance);
        }

        /// <summary>
        /// Remove us from the specified mananger's Tick function
        /// </summary>
        /// <param name="manager">The manage we want to be removed from</param>
        public void UnregisterSystem(Manager manager)
        {
            manager.UnregisterSystem(this);
        }
        
        /// <summary>
        /// Remove us from the default (singleton) mananger's Tick function
        /// </summary>
        public void UnregisterSystem()
        {
            UnregisterSystem(Manager.Instance);
        }
    }
}
