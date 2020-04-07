using SlowLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1 {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        #region Simo01
        private void button1_Click(object sender, EventArgs e)
        {
            //threadpool
            //countdownevent
            //synchronizationcontext
            //interlocked

            int sum = 0;
            CountdownEvent countdown = new CountdownEvent(3);
            SynchronizationContext ctx = SynchronizationContext.Current;
            int[] values = new int[] { int.Parse(textBox1.Text), int.Parse(textBox2.Text), int.Parse(textBox3.Text) };
            Label[] labels = new Label[] { label1,label2,label3 };

            for (int i = 0; i < 3; i++)
            {
                ThreadPool.QueueUserWorkItem((myI) => {
                    int index = (int)myI;
                    int result = SlowMath.SlowSquare04(values[index]);
                    Interlocked.Add(ref sum, result);
                    
                    countdown.Signal();

                    ctx.Post(res=> {
                        labels[index].Text = res.ToString();
                    },result);

                },i);
            }

            ThreadPool.QueueUserWorkItem(_=> {
                countdown.Wait();
                ctx.Post(o => {
                    label4.Text = sum.ToString();
                }, null); 
            });
        }

        private void btnParallel_Click(object sender, EventArgs e)
        {
            //threadpool
            //Parallel.For
            //synchronizationcontext
            //interlocked

            int sum = 0;
            SynchronizationContext ctx = SynchronizationContext.Current;
            int[] values = new int[] { int.Parse(textBox1.Text), int.Parse(textBox2.Text), int.Parse(textBox3.Text) };
            Label[] labels = new Label[] { label1, label2, label3 };

            ThreadPool.QueueUserWorkItem(_ => {
                
                Parallel.For(0, 3, (myI) => {
                    int index = (int)myI;
                    int result = SlowMath.SlowSquare04(values[index]);
                    Interlocked.Add(ref sum, result);

                    ctx.Post(res => {
                        labels[index].Text = res.ToString();
                    }, result);

                });

                ctx.Post(o => {
                    label4.Text = sum.ToString();
                }, null);
            },null);
        }

        private void btnTasks_Click(object sender, EventArgs e)
        {
            Task<int> t1 = SlowMath.SlowSquareAsync(int.Parse(textBox1.Text));
            Task<int> t2 = SlowMath.SlowSquareAsync(int.Parse(textBox2.Text));
            Task<int> t3 = SlowMath.SlowSquareAsync(int.Parse(textBox3.Text));

            //t1.ContinueWith(tres => label1.Text = tres.Result.ToString()); //BOOM!

            //SynchronizationContext ctx = SynchronizationContext.Current;
            //t1.ContinueWith(tres => {
            //    ctx.Post(_ => label1.Text = tres.Result.ToString(), null);
            //}); //this works, but we can write less code with the following overload of the ContinueWith:


            t1.ContinueWith(tres => label1.Text = tres.Result.ToString(), TaskScheduler.FromCurrentSynchronizationContext()); //better 
            t2.ContinueWith(tres => label2.Text = tres.Result.ToString(), TaskScheduler.FromCurrentSynchronizationContext()); 
            t3.ContinueWith(tres => label3.Text = tres.Result.ToString(), TaskScheduler.FromCurrentSynchronizationContext()); 

            Task.WhenAll(t1,t2,t3).ContinueWith(tres => label4.Text = tres.Result.Sum().ToString(), TaskScheduler.FromCurrentSynchronizationContext()); 
        }

        private async void btnAsyncAwait01_Click(object sender, EventArgs e)
        {
            // with this solution we don't block the ui, 
            // but we don't see the result as soon as possible

            Task<int> t1 = SlowMath.SlowSquareAsync(int.Parse(textBox1.Text));
            Task<int> t2 = SlowMath.SlowSquareAsync(int.Parse(textBox2.Text));
            Task<int> t3 = SlowMath.SlowSquareAsync(int.Parse(textBox3.Text));

            int r1 = await t1;
            label1.Text = r1.ToString();
            int r2 = await t2;
            label2.Text = r2.ToString();
            int r3 = await t3;
            label3.Text = r3.ToString();

            label4.Text = (r1 + r2 + r3).ToString();
        }

        private async void btnAsyncAwait02_Click(object sender, EventArgs e)
        {
            //let's set the scene.
            //we're going to need:

            //1) an array with all the input of the SlowSquareAsync
            int[] numbers = { int.Parse(textBox1.Text), int.Parse(textBox2.Text), int.Parse(textBox3.Text) };

            //2) an array with the output labels where we want to see the results as soon as they arrive
            Label[] labels = { label1, label2, label3 };

            //3) a method that actually waits for the task and prints the result on the label
            async Task waitAndPrint(Task<int> t, int index) => labels[index].Text = (await t).ToString();

            // We're going to have some fun with LINQ 
            // to invoke our two methods SlowSquareAsync and waitAndPrint
            // (we could use a foreach or a for, but why not LINQ?)

            // Let's go!

            // This calls the SlowSquareAsync 3 times
            // (once for each number we have in the array)
            // without waiting for it to finish
            // and gives us back all the returned tasks in an array.
            // We're going to use the array in the following query.
            Task<int>[] tasks = numbers.Select(SlowMath.SlowSquareAsync).ToArray();
            
            //it is the same as to say:
            //Task<int>[] tasks = new Task<int>[numbers.Length];
            //for (int i = 0; i < numbers.Length; i++)
            //{
            //    tasks[i] = SlowMath.SlowSquareAsync(numbers[i]);
            //}
            

            // This invokes the waitAndPrint 3 times
            // (once for each task returned by the previous query)
            // passing the Task as first parameter and an int as second parameter
            // (it's the overload of the Select that passes the index).
            // The waitAndPrint will start waiting for the task,
            // writing the result on the corresponding label as soon as it arrives
            tasks.Select(waitAndPrint).ToArray();

            //it is the same as to say:
            //for (int i = 0; i < numbers.Length; i++)
            //{
            //    waitAndPrint(tasks[i], i);
            //}


            // In the meantime we can continue.
            // We can wait for all the SlowSquareAsync to be completed,
            // then sum the results (again, let's use LINQ) 
            // and print the sum on label4
            int[] res = await Task.WhenAll(tasks);
            label4.Text = res.Sum().ToString();
        }

        #endregion

        #region Alexander
        private int _sumAlexander = 0;
        private void btnAlexander_Click(object sender, EventArgs e)
        {
            _sumAlexander = 0;
            SynchronizationContext maincontext = SynchronizationContext.Current;
            CountdownEvent cd = new CountdownEvent(3);

            runThread(maincontext, cd, label1, int.Parse(textBox1.Text));
            runThread(maincontext, cd, label2, int.Parse(textBox2.Text));
            runThread(maincontext, cd, label3, int.Parse(textBox3.Text));

            ThreadPool.QueueUserWorkItem((object _) => {
                cd.Wait();

                maincontext.Post((object o)=> {
                    label4.Text += $" {_sumAlexander}";
                },null);
            }, null);
        }

        private void runThread(SynchronizationContext ctx, CountdownEvent countdownEvent, Label lbl, int n) {
            ThreadPool.QueueUserWorkItem((object _)=> {
                int result = SlowMath.SlowSquare04(n);
                Interlocked.Add(ref _sumAlexander, result);

                ctx.Post((object o)=> {
                    lbl.Text += $" {result}";
                },null);
                countdownEvent.Signal();
            },null);
        }



        #endregion

        
    }
}
