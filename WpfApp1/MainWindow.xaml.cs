using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int numberOfDataSupplierThreads = 1;
        private int minSleepDuration = 1;
        private int maxSleepDuration = 6;
        private ManagerThread managerThread = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {

            txtResult.Text = "";

            if (!string.IsNullOrEmpty(txtNumOfThreads.Text))
            {
                numberOfDataSupplierThreads = Int32.Parse(txtNumOfThreads.Text);
            }

            if (!string.IsNullOrEmpty(txtMinSleepDuration.Text))
            {
                minSleepDuration = Int32.Parse(txtMinSleepDuration.Text);
            }

            if (!string.IsNullOrEmpty(txtMaxSleepDuration.Text))
            {
                maxSleepDuration = Int32.Parse(txtMaxSleepDuration.Text);
            }

            managerThread = new ManagerThread(numberOfDataSupplierThreads, minSleepDuration, maxSleepDuration);

            managerThread.NewDataProcessed += OnNewMessageArrived;

            var task = Task.Run(() => managerThread.Start());//running thread manager on separate thread to prevent blocking of UI thread

            btnStart.IsEnabled = false;
            btnStop.IsEnabled = true;


        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            if (managerThread != null)
            {
                managerThread.Stop();
            }

            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
        }

        private void OnNewMessageArrived(object sender, NewDataProcessedEventArgs args)
        {
            if (args.Message != null)
            {
                Dispatcher.Invoke(() => {
                    txtResult.Text += $"\r\nData Supplier: {args.Message.Message} | Message creation time: {args.Message.TimeToCreateMessage} seconds | Processing time: {args.Message.ProcessingTime} seconds";
                });
                
            }
        }
    }
}
