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

namespace WpfApplication {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void btnSingleThreaded_Click(object sender, RoutedEventArgs e) {
            //this long running operation
            //is going to freeze the user interface
            lblMessage.Content = "Thinking (single Threaded)";
            for (long i = 0; i < 10000000000; i++) {

            }
            lblMessage.Content = "Done.";
        }

        private void btnMultiThreaded_Click(object sender, RoutedEventArgs e) {
            //this long running operation
            //is not going to freeze the ui
            void runThis(){
                for (long i = 0; i < 10000000000; i++) {

                }
                //BUT as we try to update the ui.....
                //boom!
                lblMessage.Content = "Done.";
                //this is because we're running in a thread 
                //that in not the one that created
                //(and owns) the UI
            };
            lblMessage.Content = "Thinking (single Threaded)";
            new Thread(runThis).Start();
        }

        private void btnSynchronizationContext_Click(object sender, RoutedEventArgs e) {
            //let's save the SyncronizationContext 
            //so that we can retrieve it in the other Thread:
            SynchronizationContext mainSyncContext = SynchronizationContext.Current;

            void runThis(){
                for (long i = 0; i < 10000000000; i++) {

                }
                //we have to go back to the thread that
                //started it all
                mainSyncContext.Post(_ => {
                    //we'running int the correct thread now!
                    lblMessage.Content = "Done.";
                }, null);
            };

            lblMessage.Content = "Thinking (single Threaded)";
            new Thread(runThis).Start();
        }

        private void btnAsyncDelegate_Click(object sender, RoutedEventArgs e) {
            //if you want to get the result of method
            //run in another thread, you can't do that
            //with a Thread (you can only pass methods 
            //that return void!)
            //But you could use a delegate and get the result in a callback:
            Func<string, int> calculate = s => s.Length;

            SynchronizationContext ctx = SynchronizationContext.Current;

            void callback(IAsyncResult asyncresult){
                int result = calculate.EndInvoke(asyncresult);
                //but don't forget to go back to the main Thread!
                ctx.Post(_ => lblMessage.Content = $"{Thread.CurrentThread.ManagedThreadId} shows the result: {result}",null);
            };

            calculate.BeginInvoke("supercalifragilisticexpialidocious", callback, null);
        }

        private async void btnStep1_Click(object sender, RoutedEventArgs e) {
            btnStep2.IsEnabled = true;
            await btnStep2.ClickAsync();
            btnStep3.IsEnabled = true;
            await btnStep3.ClickAsync();
            MessageBox.Show("DONE!");
        }
    }
}
