using System;

namespace TS.ECS
{
    /// <summary>
    /// 
    /// </summary>
    public class MessageEventArgs: EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="messageData"></param>
        /// <param name="sender"></param>
        public MessageEventArgs(int messageType, object messageData, Manager manager, object sender = null)
        {
            MessageType = messageType;
            MessageData = messageData;
            Manager = manager;
            Sender = sender;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public int MessageType { get; }

        /// <summary>
        /// 
        /// </summary>
        public object MessageData { get; }

        /// <summary>
        /// 
        /// </summary>
        public Manager Manager { get; }

        /// <summary>
        /// 
        /// </summary>
        internal object Sender { get; }
    }
}
