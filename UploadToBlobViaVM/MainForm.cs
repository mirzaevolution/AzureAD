using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using System.Configuration;
namespace UploadToBlobViaVM
{
    public partial class MainForm : Form
    {
        private string _accessToken = string.Empty;
        private CloudBlobClient _cloudBlobClient;

        public MainForm()
        {
            InitializeComponent();
            this.Load += MainForm_Load;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                AzureServiceTokenProvider azureServiceTokenProvider;
                azureServiceTokenProvider = new AzureServiceTokenProvider();
                _accessToken = azureServiceTokenProvider.GetAccessTokenAsync("https://storage.azure.com").GetAwaiter().GetResult();
                MessageBox.Show("Access token has been copied to clipboard");
                Clipboard.SetText(_accessToken);
                if (string.IsNullOrEmpty(_accessToken))
                {
                    MessageBox.Show("Access token is empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ButtonBrowse.Enabled = false;
                    ButtonUpload.Enabled = false;
                }
                else
                {
                    TokenCredential tokenCredential = new TokenCredential(_accessToken);
                    StorageCredentials storageCredentials = new StorageCredentials(tokenCredential);
                    _cloudBlobClient = new CloudStorageAccount(storageCredentials, "storageevolution", "core.windows.net", true).CreateCloudBlobClient();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.FileName = "";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    TextBoxSource.Text = openFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void ButtonUpload_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(TextBoxSource.Text))
                {
                    await Upload(TextBoxSource.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async Task Upload(string path)
        {
            var container = _cloudBlobClient.GetContainerReference("core");
            string fileName = Path.GetFileName(path);
            var blob = container.GetBlockBlobReference(fileName);
            await blob.UploadFromFileAsync(path);
            MessageBox.Show("Upload success. Url has been copied to clipboard!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Clipboard.SetText(blob.Uri.ToString());
        }
    }
}
