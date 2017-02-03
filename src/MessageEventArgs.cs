using System;

namespace TS.ECS
{
    /// <summary>
    /// Class inheriting from EventArgs which is used to broadcast messages to systems
    /// </summary>
    public class MessageEventArgs: EventArgs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="messageType">The message type. Integer type used for speed during lookups</param>
        /// <param name="messageData">The data that we want to follow the message to its destination</param>
        /// <param name="manager">The manager that broadcast the message</param>
		/// <param name="sender">The external object that triggered the broadcast command (can be null)</param>
        internal MessageEventArgs(int messageType, object messageData, Manager manager, object sender, bool async)
        {
            MessageType = messageType;
            MessageData = messageData;
            Manager = manager;
            Sender = sender;
			Async = async;
        }
        
        /// <summary>
        /// The message type. Integer type used for speed during lookups
        /// </summary>
        public int MessageType { get; }

        /// <summary>
        /// The data that we want to follow the message to its destination
        /// </summary>
        public object MessageData { get; }

        /// <summary>
        /// The manager that broadcast the message
        /// </summary>
        public Manager Manager { get; }

        /// <summary>
        /// The external object that triggered the broadcast command (can be null)
        /// </summary>
        internal object Sender { get; }

		/// <summary>
		/// Was this message broacast asynchronously?
		/// </summary>
		internal bool Async { get; }
    }
}
