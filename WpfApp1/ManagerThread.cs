using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

namespace WpfApp1
{
    public class ManagerThread : IDataSupplierThread
    {
        public event EventHandler<NewDataProcessedEventArgs> NewDataProcessed;

        private List<IDataSupplierThread> ThreadDataSuppliers;

        private ConcurrentQueue<DataMessage> messageQueue = new ConcurrentQueue<DataMessage>();
        

        private static int threadCount = 0;
        private bool isAllowedToRun = false;
        private int _minSleepDuration = 1;
        private int _maxSleepDuration = 10;

        private static EventWaitHandle ewh;
        

        public ManagerThread(int numberOfDataSuppliers, int minSleepDuration, int maxSleepDuration)
        {
            var me = this;
            isAllowedToRun = true;
            threadCount = numberOfDataSuppliers;
            _minSleepDuration = minSleepDuration;
            _maxSleepDuration = maxSleepDuration;
            ewh = new EventWaitHandle(false, EventResetMode.AutoReset);

            ThreadDataSuppliers = new List<IDataSupplierThread>();

            for (int i = 1; i <= numberOfDataSuppliers; i++)
            {
                ThreadDataSuppliers.Add(new DataSupplierThread(ref me, i, minSleepDuration, maxSleepDuration, ewh));
            }
        }

        /// <summary>
        /// Executes Run method in separate thread.
        /// Executes all data supplying threads. 
        /// </summary>
        public void Start()
        {
            Task.Run(() => this.Run());

            foreach (var prod in ThreadDataSuppliers)
            {
                prod.Start();
            }
        }

        /// <summary>
        /// Signals all the data supplier threads to block. 
        /// Stops the execution of Run method (executed in separate thread). 
        /// Stops the execution of data suplying threads.
        /// </summary>
        public void Stop()
        {
            isAllowedToRun = false;

            ewh.Reset();

            foreach (var prod in ThreadDataSuppliers)
            {
                prod.Stop();
            }

        }

        /// <summary>
        /// Blocks and wait for being signaled by one of the data supplying threads.
        /// When released, checks if there messages in the queue, fetchs a single message and "processes" it (emulated by sleeping for random number of seconds). 
        /// After then emits the event to notify the UI thread
        /// </summary>
        private void Run()
        {
            while (isAllowedToRun)
            {
                ewh.WaitOne();

                var sleepDuration = new Random().Next(_minSleepDuration, _maxSleepDuration);
                Thread.Sleep(sleepDuration * 1000);
                
                if (messageQueue.Count > 0)
                {
                    DataMessage message;
                    messageQueue.TryDequeue(out message);
                    

                    if (message != null)
                    {
                        message.ProcessingTime = sleepDuration;
                        NewDataProcessedEventArgs args = new NewDataProcessedEventArgs();
                        args.Message = message;

                        OnNewDataProcessed(args);
                    }

                }
            }
        }

        public void Enqueue(DataMessage message)
        {
            messageQueue.Enqueue(message);//push message into thread-safe queue

        }

        /// <summary>
        /// Emit event to notify UI thread
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnNewDataProcessed(NewDataProcessedEventArgs e)
        {
            NewDataProcessed?.Invoke(this, e);//Notify UI
        }
    }
}
