using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class DataSupplierThread: IDataSupplierThread
    {
        private string _name = "Thread";
        private bool _isAllowedToRun = false;
        private int _minSleepDuration = 1;
        private int _maxSleepDuration = 10;
        private ManagerThread _managerThread = null;
        private static EventWaitHandle _ewh;

        public DataSupplierThread(ref ManagerThread managerThread, int num, int minSleepDuration, int maxSleepDuration, EventWaitHandle ewh)
        {
            _name += num.ToString();
            _minSleepDuration = minSleepDuration;
            _maxSleepDuration = maxSleepDuration;
            _managerThread = managerThread;
            _ewh = ewh;
        }

        /// <summary>
        /// Executes Run method on separate thread
        /// </summary>
        public void Start()
        {
            _isAllowedToRun = true;

            var t = Task.Run(() => this.Run());

        }

        /// <summary>
        /// Stops the execution of Run method
        /// </summary>
        public void Stop()
        {
            _isAllowedToRun = false;
            
        }

        /// <summary>
        /// Being executed on separate thread. 
        /// Generates message (emulated by sleeping for random number of seconds) and passes it to shared manager thread, releasing the block.
        /// </summary>
        private void Run()
        {
            while (_isAllowedToRun)
            {
                
                var sleepDuration = new Random().Next(_minSleepDuration, _maxSleepDuration);

                Thread.Sleep(sleepDuration * 1000);

                DataMessage msg = new DataMessage(_name, sleepDuration);

                _managerThread.Enqueue(msg);

                _ewh.Set();
            }
        }

    }
}
