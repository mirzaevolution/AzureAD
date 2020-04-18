namespace AzureADMSALWinForm
{
    partial class FormMain
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
            this.ButtonApiEndpoint = new System.Windows.Forms.Button();
            this.ButtonApi2Endpoint = new System.Windows.Forms.Button();
            this.ButtonLogin = new System.Windows.Forms.Button();
            this.ButtonLogout = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ButtonApiEndpoint
            // 
            this.ButtonApiEndpoint.Enabled = false;
            this.ButtonApiEndpoint.Location = new System.Drawing.Point(33, 81);
            this.ButtonApiEndpoint.Name = "ButtonApiEndpoint";
            this.ButtonApiEndpoint.Size = new System.Drawing.Size(270, 46);
            this.ButtonApiEndpoint.TabIndex = 0;
            this.ButtonApiEndpoint.Text = "Api 1 Endpoint";
            this.ButtonApiEndpoint.UseVisualStyleBackColor = true;
            this.ButtonApiEndpoint.Click += new System.EventHandler(this.ButtonApi1EndpointClickHandler);
            // 
            // ButtonApi2Endpoint
            // 
            this.ButtonApi2Endpoint.Enabled = false;
            this.ButtonApi2Endpoint.Location = new System.Drawing.Point(33, 153);
            this.ButtonApi2Endpoint.Name = "ButtonApi2Endpoint";
            this.ButtonApi2Endpoint.Size = new System.Drawing.Size(270, 46);
            this.ButtonApi2Endpoint.TabIndex = 1;
            this.ButtonApi2Endpoint.Text = "Api 2 Endpoint";
            this.ButtonApi2Endpoint.UseVisualStyleBackColor = true;
            this.ButtonApi2Endpoint.Click += new System.EventHandler(this.ButtonApi2EndpointClickHandler);
            // 
            // ButtonLogin
            // 
            this.ButtonLogin.Location = new System.Drawing.Point(12, 12);
            this.ButtonLogin.Name = "ButtonLogin";
            this.ButtonLogin.Size = new System.Drawing.Size(104, 37);
            this.ButtonLogin.TabIndex = 2;
            this.ButtonLogin.Text = "Login";
            this.ButtonLogin.UseVisualStyleBackColor = true;
            this.ButtonLogin.Click += new System.EventHandler(this.ButtonLoginClickHandler);
            // 
            // ButtonLogout
            // 
            this.ButtonLogout.Enabled = false;
            this.ButtonLogout.Location = new System.Drawing.Point(226, 12);
            this.ButtonLogout.Name = "ButtonLogout";
            this.ButtonLogout.Size = new System.Drawing.Size(104, 37);
            this.ButtonLogout.TabIndex = 3;
            this.ButtonLogout.Text = "Logout";
            this.ButtonLogout.UseVisualStyleBackColor = true;
            this.ButtonLogout.Click += new System.EventHandler(this.ButtonLogoutClickHandler);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 261);
            this.Controls.Add(this.ButtonLogout);
            this.Controls.Add(this.ButtonLogin);
            this.Controls.Add(this.ButtonApi2Endpoint);
            this.Controls.Add(this.ButtonApiEndpoint);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form Main";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ButtonApiEndpoint;
        private System.Windows.Forms.Button ButtonApi2Endpoint;
        private System.Windows.Forms.Button ButtonLogin;
        private System.Windows.Forms.Button ButtonLogout;
    }
}

