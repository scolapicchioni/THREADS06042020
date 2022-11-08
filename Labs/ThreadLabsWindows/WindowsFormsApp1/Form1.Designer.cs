namespace WindowsFormsApp1 {
    partial class Form1 {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.btnParallelFor = new System.Windows.Forms.Button();
            this.btnTasks = new System.Windows.Forms.Button();
            this.btnAsyncAwait02 = new System.Windows.Forms.Button();
            this.btnAsyncAwait01 = new System.Windows.Forms.Button();
            this.btnParallelInvoke = new System.Windows.Forms.Button();
            this.btnTasksResult = new System.Windows.Forms.Button();
            this.btnTasksContinueWith = new System.Windows.Forms.Button();
            this.btnContinueWithFromCurrentyncContext = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(192, 67);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 22);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = "5";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(192, 121);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 22);
            this.textBox2.TabIndex = 1;
            this.textBox2.Text = "3";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(192, 177);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(100, 22);
            this.textBox3.TabIndex = 2;
            this.textBox3.Text = "4";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(363, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 17);
            this.label1.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(366, 121);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 17);
            this.label2.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(366, 177);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 17);
            this.label3.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(369, 240);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(0, 17);
            this.label4.TabIndex = 6;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(484, 67);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(114, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "ThreadPool";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnParallelFor
            // 
            this.btnParallelFor.Location = new System.Drawing.Point(484, 150);
            this.btnParallelFor.Name = "btnParallelFor";
            this.btnParallelFor.Size = new System.Drawing.Size(114, 23);
            this.btnParallelFor.TabIndex = 9;
            this.btnParallelFor.Text = "Parallel For";
            this.btnParallelFor.UseVisualStyleBackColor = true;
            this.btnParallelFor.Click += new System.EventHandler(this.btnParallelFor_Click);
            // 
            // btnTasks
            // 
            this.btnTasks.Location = new System.Drawing.Point(484, 193);
            this.btnTasks.Name = "btnTasks";
            this.btnTasks.Size = new System.Drawing.Size(114, 23);
            this.btnTasks.TabIndex = 10;
            this.btnTasks.Text = "Tasks";
            this.btnTasks.UseVisualStyleBackColor = true;
            this.btnTasks.Click += new System.EventHandler(this.btnTasks_Click);
            // 
            // btnAsyncAwait02
            // 
            this.btnAsyncAwait02.Location = new System.Drawing.Point(484, 282);
            this.btnAsyncAwait02.Name = "btnAsyncAwait02";
            this.btnAsyncAwait02.Size = new System.Drawing.Size(114, 23);
            this.btnAsyncAwait02.TabIndex = 11;
            this.btnAsyncAwait02.Text = "async await 2";
            this.btnAsyncAwait02.UseVisualStyleBackColor = true;
            this.btnAsyncAwait02.Click += new System.EventHandler(this.btnAsyncAwait02_Click);
            // 
            // btnAsyncAwait01
            // 
            this.btnAsyncAwait01.Location = new System.Drawing.Point(484, 237);
            this.btnAsyncAwait01.Name = "btnAsyncAwait01";
            this.btnAsyncAwait01.Size = new System.Drawing.Size(114, 23);
            this.btnAsyncAwait01.TabIndex = 12;
            this.btnAsyncAwait01.Text = "async await 1";
            this.btnAsyncAwait01.UseVisualStyleBackColor = true;
            this.btnAsyncAwait01.Click += new System.EventHandler(this.btnAsyncAwait01_Click);
            // 
            // btnParallelInvoke
            // 
            this.btnParallelInvoke.Location = new System.Drawing.Point(484, 106);
            this.btnParallelInvoke.Name = "btnParallelInvoke";
            this.btnParallelInvoke.Size = new System.Drawing.Size(114, 23);
            this.btnParallelInvoke.TabIndex = 13;
            this.btnParallelInvoke.Text = "Parallel Invoke";
            this.btnParallelInvoke.UseVisualStyleBackColor = true;
            this.btnParallelInvoke.Click += new System.EventHandler(this.btnParallelInvoke_Click);
            // 
            // btnTasksResult
            // 
            this.btnTasksResult.Location = new System.Drawing.Point(604, 193);
            this.btnTasksResult.Name = "btnTasksResult";
            this.btnTasksResult.Size = new System.Drawing.Size(110, 23);
            this.btnTasksResult.TabIndex = 14;
            this.btnTasksResult.Text = "Tasks Result";
            this.btnTasksResult.UseVisualStyleBackColor = true;
            this.btnTasksResult.Click += new System.EventHandler(this.btnTasksResult_Click);
            // 
            // btnTasksContinueWith
            // 
            this.btnTasksContinueWith.Location = new System.Drawing.Point(720, 193);
            this.btnTasksContinueWith.Name = "btnTasksContinueWith";
            this.btnTasksContinueWith.Size = new System.Drawing.Size(154, 23);
            this.btnTasksContinueWith.TabIndex = 15;
            this.btnTasksContinueWith.Text = "Tasks ContinueWith";
            this.btnTasksContinueWith.UseVisualStyleBackColor = true;
            this.btnTasksContinueWith.Click += new System.EventHandler(this.btnTasksContinueWith_Click);
            // 
            // btnContinueWithFromCurrentyncContext
            // 
            this.btnContinueWithFromCurrentyncContext.Location = new System.Drawing.Point(880, 193);
            this.btnContinueWithFromCurrentyncContext.Name = "btnContinueWithFromCurrentyncContext";
            this.btnContinueWithFromCurrentyncContext.Size = new System.Drawing.Size(239, 23);
            this.btnContinueWithFromCurrentyncContext.TabIndex = 16;
            this.btnContinueWithFromCurrentyncContext.Text = "Tasks ContinueWith SyncContext";
            this.btnContinueWithFromCurrentyncContext.UseVisualStyleBackColor = true;
            this.btnContinueWithFromCurrentyncContext.Click += new System.EventHandler(this.btnContinueWithFromCurrentyncContext_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1148, 450);
            this.Controls.Add(this.btnContinueWithFromCurrentyncContext);
            this.Controls.Add(this.btnTasksContinueWith);
            this.Controls.Add(this.btnTasksResult);
            this.Controls.Add(this.btnParallelInvoke);
            this.Controls.Add(this.btnAsyncAwait01);
            this.Controls.Add(this.btnAsyncAwait02);
            this.Controls.Add(this.btnTasks);
            this.Controls.Add(this.btnParallelFor);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnParallelFor;
        private System.Windows.Forms.Button btnTasks;
        private System.Windows.Forms.Button btnAsyncAwait02;
        private System.Windows.Forms.Button btnAsyncAwait01;
        private System.Windows.Forms.Button btnParallelInvoke;
        private System.Windows.Forms.Button btnTasksResult;
        private System.Windows.Forms.Button btnTasksContinueWith;
        private System.Windows.Forms.Button btnContinueWithFromCurrentyncContext;
    }
}

