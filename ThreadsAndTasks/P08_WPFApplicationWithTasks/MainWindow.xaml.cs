using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace P08_WPFApplicationWithTasks {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void button5_Click(object sender, RoutedEventArgs e) {
            //a solution is to use the ConfigureAwait 
            Task<string> t = calculateAndMoreAsyncConfigureAwait(2);
            //we wait for the result, blocking this line of code
            //until we get the result.
            //we are on the synchronizationcontext
            //that the task does not care about.
            //that task does not wait for us to let it go.
            //Hence, no deadlock.
            labelMessage.Content = t.Result;
        }

        private async Task<string> calculateAndMoreAsyncConfigureAwait(int seconds) {
            //This function uses await, but it does not 
            //lock the SynchronizationContext.
            //It will continue on the Thread where the result was retrieved
            int result = await calculateSlowlyAsync01(seconds).ConfigureAwait(false);
            return $"result {result} retrieved on Thread {Thread.CurrentThread.ManagedThreadId}";
        }

        private void button4_Click(object sender, RoutedEventArgs e) {
            Task<string> t = calculateAndMoreAsync(2);
            //we wait synchronously for the result, 
            //blocking this line of code
            //until we get it.
            //unfortunately, we are on the synchronizationcontext
            //that the task would like to use.
            //that task waits for us to let it go.
            //we're waiting for the task to give us a result.
            //Hence, a deadlock.
            labelMessage.Content = t.Result;
        }

        private async Task<string> calculateAndMoreAsync(int seconds) {
            //this function uses await, meaning that
            //it locks the SynchronizationContext.
            //It won't continue until the SynchronizationContext is freed
            int result = await calculateSlowlyAsync01(seconds);
            return $"result {result} retrieved on Thread {Thread.CurrentThread.ManagedThreadId}";
        }

        private void button2_Click(object sender, RoutedEventArgs e) {
            //we can reproduce the await by passing the correct task scheduler
            labelMessage.Content = "busy...";
            calculateSlowlyAsync01(5).ContinueWith(t=> {
                labelMessage.Content = $"{t.Result} retrieved on Thread {Thread.CurrentThread.ManagedThreadId}";
            },TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async void button1_Click(object sender, RoutedEventArgs e) {
            //on the other hand the await just works!
            labelMessage.Content = "busy...";
            int result = await calculateSlowlyAsync01(5);
            labelMessage.Content = $"{result} retrieved on Thread {Thread.CurrentThread.ManagedThreadId}";
        }

        private void button3_Click(object sender, RoutedEventArgs e) {
            //if we continue we explode because 
            //the continuation callback gets invoked
            //on a different thread
            labelMessage.Content = "busy...";
            calculateSlowlyAsync01(5).ContinueWith(t => {
                labelMessage.Content = $"{t.Result} retrieved on Thread {Thread.CurrentThread.ManagedThreadId}";
            });
        }

        //or we could use the compiler
        private async Task<int> calculateSlowlyAsync02(int seconds) {
            int result = calculateSlowly(seconds);
            return result;
        }

        //we can create an async version like this
        private Task<int> calculateSlowlyAsync01(int seconds) {
            return Task.Run(() => calculateSlowly(seconds));
        }

        //this function is very slow
        private int calculateSlowly(int seconds) {
            for (int i = 0; i < seconds * 10000000; i++) {

            }
            return seconds;
        }

    }
}
