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
