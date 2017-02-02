using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

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
        private BlockingCollection<MessageEventArgs> messageQueue = new BlockingCollection<MessageEventArgs>();

        /// <summary>
        /// 
        /// </summary>
        private ManualResetEvent mre = new ManualResetEvent(true);

        /// <summary>
        /// 
        /// </summary>
        private int amountOfConsumers;

        /// <summary>
        /// 
        /// </summary>
        private object o = new object();
        
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<MessageEventArgs> OnMessage;

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
        /// <param name="args"></param>
        private void HandleMessage(MessageEventArgs args)
        {
            if (OnMessage != null)
            {
                OnMessage.Invoke(args.Sender, args);
            }
        }
        
        /// <summary>
        /// Based on: http://stackoverflow.com/a/15232041 
        /// </summary>
        /// <param name="message"></param>
        internal void EnqueueMessage(MessageEventArgs message)
        {
            messageQueue.Add(message);

            mre.WaitOne();
            if (Math.Floor((double)messageQueue.Count / 100)+1 > amountOfConsumers)
            {
                Interlocked.Increment(ref amountOfConsumers);
    
                var task = Task.Factory.StartNew(() =>
                {
                    MessageEventArgs msg;
                    bool repeat = true;
    
                    while (repeat)
                    {
                        while ((messageQueue.Count > 0) && (Math.Floor((double)((messageQueue.Count + 50) / 100)) + 1 >= amountOfConsumers))
                        {
                            msg = messageQueue.Take();
                            HandleMessage(msg);
                        }
    
                        lock (o)
                        {
                            mre.Reset();
    
                            if ((messageQueue.Count == 0) || (Math.Ceiling((double)((messageQueue.Count + 51) / 100)) < amountOfConsumers))
                            {
                                ConsumerQuit();
                                repeat = false;
                            }
    
                            mre.Set();
                        }
                    }
                });
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        private void ConsumerQuit()
        {
            Interlocked.Decrement(ref amountOfConsumers);
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
