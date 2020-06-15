namespace UploadToBlobViaVM
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
            this.ButtonBrowse = new System.Windows.Forms.Button();
            this.ButtonUpload = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TextBoxSource
            // 
            this.TextBoxSource.Location = new System.Drawing.Point(12, 12);
            this.TextBoxSource.Name = "TextBoxSource";
            this.TextBoxSource.Size = new System.Drawing.Size(422, 22);
            this.TextBoxSource.TabIndex = 0;
            // 
            // ButtonBrowse
            // 
            this.ButtonBrowse.Location = new System.Drawing.Point(440, 11);
            this.ButtonBrowse.Name = "ButtonBrowse";
            this.ButtonBrowse.Size = new System.Drawing.Size(34, 23);
            this.ButtonBrowse.TabIndex = 1;
            this.ButtonBrowse.Text = "...";
            this.ButtonBrowse.UseVisualStyleBackColor = true;
            this.ButtonBrowse.Click += new System.EventHandler(this.ButtonBrowse_Click);
            // 
            // ButtonUpload
            // 
            this.ButtonUpload.Location = new System.Drawing.Point(12, 49);
            this.ButtonUpload.Name = "ButtonUpload";
            this.ButtonUpload.Size = new System.Drawing.Size(462, 36);
            this.ButtonUpload.TabIndex = 2;
            this.ButtonUpload.Text = "Upload";
            this.ButtonUpload.UseVisualStyleBackColor = true;
            this.ButtonUpload.Click += new System.EventHandler(this.ButtonUpload_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(486, 115);
            this.Controls.Add(this.ButtonUpload);
            this.Controls.Add(this.ButtonBrowse);
            this.Controls.Add(this.TextBoxSource);
            this.Name = "MainForm";
            this.Text = "Blob Storage - MSI";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TextBoxSource;
        private System.Windows.Forms.Button ButtonBrowse;
        private System.Windows.Forms.Button ButtonUpload;
    }
}

