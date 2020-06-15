namespace SenderReceiver
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TextBoxSource = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ButtonSend = new System.Windows.Forms.Button();
            this.ListboxReceiver1 = new System.Windows.Forms.ListBox();
            this.ListboxReceiver2 = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ListboxLogs = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // TextBoxSource
            // 
            this.TextBoxSource.Enabled = false;
            this.TextBoxSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBoxSource.Location = new System.Drawing.Point(12, 45);
            this.TextBoxSource.Name = "TextBoxSource";
            this.TextBoxSource.Size = new System.Drawing.Size(555, 27);
            this.TextBoxSource.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(187, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Enter message to send:";
            // 
            // ButtonSend
            // 
            this.ButtonSend.Location = new System.Drawing.Point(589, 39);
            this.ButtonSend.Name = "ButtonSend";
            this.ButtonSend.Size = new System.Drawing.Size(99, 41);
            this.ButtonSend.TabIndex = 2;
            this.ButtonSend.Text = "Send";
            this.ButtonSend.UseVisualStyleBackColor = true;
            this.ButtonSend.Click += new System.EventHandler(this.ButtonSendClickHandler);
            // 
            // ListboxReceiver1
            // 
            this.ListboxReceiver1.FormattingEnabled = true;
            this.ListboxReceiver1.ItemHeight = 16;
            this.ListboxReceiver1.Location = new System.Drawing.Point(12, 130);
            this.ListboxReceiver1.Name = "ListboxReceiver1";
            this.ListboxReceiver1.Size = new System.Drawing.Size(331, 308);
            this.ListboxReceiver1.TabIndex = 3;
            // 
            // ListboxReceiver2
            // 
            this.ListboxReceiver2.FormattingEnabled = true;
            this.ListboxReceiver2.ItemHeight = 16;
            this.ListboxReceiver2.Location = new System.Drawing.Point(363, 130);
            this.ListboxReceiver2.Name = "ListboxReceiver2";
            this.ListboxReceiver2.Size = new System.Drawing.Size(325, 308);
            this.ListboxReceiver2.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(8, 103);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "Receiver #1: ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(359, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(108, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "Receiver #2: ";
            // 
            // ListboxLogs
            // 
            this.ListboxLogs.BackColor = System.Drawing.Color.Black;
            this.ListboxLogs.ForeColor = System.Drawing.Color.Lime;
            this.ListboxLogs.FormattingEnabled = true;
            this.ListboxLogs.ItemHeight = 16;
            this.ListboxLogs.Location = new System.Drawing.Point(12, 458);
            this.ListboxLogs.Name = "ListboxLogs";
            this.ListboxLogs.Size = new System.Drawing.Size(676, 148);
            this.ListboxLogs.TabIndex = 7;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(699, 623);
            this.Controls.Add(this.ListboxLogs);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ListboxReceiver2);
            this.Controls.Add(this.ListboxReceiver1);
            this.Controls.Add(this.ButtonSend);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TextBoxSource);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Event Hub - MSI";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TextBoxSource;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ButtonSend;
        private System.Windows.Forms.ListBox ListboxReceiver1;
        private System.Windows.Forms.ListBox ListboxReceiver2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox ListboxLogs;
    }
}

