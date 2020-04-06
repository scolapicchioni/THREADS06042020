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
            Console.WriteLine($"The sum is {sum}");
        }
    }
}
