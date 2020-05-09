namespace BlobStorageImpersonation
{
    partial class UI
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
            this.ButtonLogin = new System.Windows.Forms.Button();
            this.ButtonLogout = new System.Windows.Forms.Button();
            this.ButtonBrowse = new System.Windows.Forms.Button();
            this.OpenFileDialogLocalFile = new System.Windows.Forms.OpenFileDialog();
            this.TextBoxFileLocation = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ButtonUploadBlob = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ButtonLogin
            // 
            this.ButtonLogin.Location = new System.Drawing.Point(12, 18);
            this.ButtonLogin.Name = "ButtonLogin";
            this.ButtonLogin.Size = new System.Drawing.Size(118, 39);
            this.ButtonLogin.TabIndex = 0;
            this.ButtonLogin.Text = "Login";
            this.ButtonLogin.UseVisualStyleBackColor = true;
            this.ButtonLogin.Click += new System.EventHandler(this.ButtonLoginClickHandler);
            // 
            // ButtonLogout
            // 
            this.ButtonLogout.Enabled = false;
            this.ButtonLogout.Location = new System.Drawing.Point(297, 18);
            this.ButtonLogout.Name = "ButtonLogout";
            this.ButtonLogout.Size = new System.Drawing.Size(118, 39);
            this.ButtonLogout.TabIndex = 1;
            this.ButtonLogout.Text = "Logout";
            this.ButtonLogout.UseVisualStyleBackColor = true;
            this.ButtonLogout.Click += new System.EventHandler(this.ButtonLogoutClickHandler);
            // 
            // ButtonBrowse
            // 
            this.ButtonBrowse.Enabled = false;
            this.ButtonBrowse.Location = new System.Drawing.Point(362, 107);
            this.ButtonBrowse.Name = "ButtonBrowse";
            this.ButtonBrowse.Size = new System.Drawing.Size(52, 39);
            this.ButtonBrowse.TabIndex = 3;
            this.ButtonBrowse.Text = "...";
            this.ButtonBrowse.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.ButtonBrowse.UseVisualStyleBackColor = true;
            this.ButtonBrowse.Click += new System.EventHandler(this.ButtonBrowseClickHandler);
            // 
            // TextBoxFileLocation
            // 
            this.TextBoxFileLocation.Enabled = false;
            this.TextBoxFileLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBoxFileLocation.Location = new System.Drawing.Point(11, 108);
            this.TextBoxFileLocation.Multiline = true;
            this.TextBoxFileLocation.Name = "TextBoxFileLocation";
            this.TextBoxFileLocation.Size = new System.Drawing.Size(345, 38);
            this.TextBoxFileLocation.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(143, 17);
            this.label1.TabIndex = 5;
            this.label1.Text = "Browse file to upload:";
            // 
            // ButtonUploadBlob
            // 
            this.ButtonUploadBlob.Enabled = false;
            this.ButtonUploadBlob.Location = new System.Drawing.Point(11, 163);
            this.ButtonUploadBlob.Name = "ButtonUploadBlob";
            this.ButtonUploadBlob.Size = new System.Drawing.Size(403, 39);
            this.ButtonUploadBlob.TabIndex = 6;
            this.ButtonUploadBlob.Text = "Upload Blob";
            this.ButtonUploadBlob.UseVisualStyleBackColor = true;
            this.ButtonUploadBlob.Click += new System.EventHandler(this.ButtonUploadBlobClickHandler);
            // 
            // UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(426, 228);
            this.Controls.Add(this.ButtonUploadBlob);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TextBoxFileLocation);
            this.Controls.Add(this.ButtonBrowse);
            this.Controls.Add(this.ButtonLogout);
            this.Controls.Add(this.ButtonLogin);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "UI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Blob User Impersonation";
            this.Load += new System.EventHandler(this.UIFirstLoadHandler);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ButtonLogin;
        private System.Windows.Forms.Button ButtonLogout;
        private System.Windows.Forms.Button ButtonBrowse;
        private System.Windows.Forms.OpenFileDialog OpenFileDialogLocalFile;
        private System.Windows.Forms.TextBox TextBoxFileLocation;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ButtonUploadBlob;
    }
}

