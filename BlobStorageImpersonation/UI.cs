using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Configuration;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Cache;
using Microsoft.Azure.Storage.Core;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage;
namespace BlobStorageImpersonation
{
    public partial class UI : Form
    {
        private readonly string _clientId;
        private readonly string _authority;
        private readonly string[] _scopes;
        private readonly string _redirectUri;
        private readonly string _storageAccountName;
        private readonly string _fileTokenCacheLocation = "cache.dat";
        private static object _syncLock = new object();
        private IPublicClientApplication _appClient;
        private AuthenticationResult _authenticationResult;
        private CloudBlobClient _blobClient;
        public UI()
        {
            InitializeComponent();
            _clientId = ConfigurationManager.AppSettings["ClientId"];
            _authority = ConfigurationManager.AppSettings["Authority"];
            _scopes = ConfigurationManager.AppSettings["Scopes"].Split(';').Where(c => !string.IsNullOrEmpty(c)).ToArray();
            _redirectUri = ConfigurationManager.AppSettings["RedirectUri"];
            _storageAccountName = ConfigurationManager.AppSettings["StorageAccountName"];

        }

        private void UIFirstLoadHandler(object sender, EventArgs e)
        {

        }
        private async void ButtonLoginClickHandler(object sender, EventArgs e)
        {
            DisableAccess();
            await AuthenticateAsync();
            if (_authenticationResult != null)

            {
                TokenCredential tokenCredential = new TokenCredential(_authenticationResult.AccessToken);
                StorageCredentials storageCredentials = new StorageCredentials(tokenCredential);
                _blobClient = new CloudStorageAccount(storageCredentials, _storageAccountName, "core.windows.net", true).CreateCloudBlobClient();
                EnableAccess();
            }
        }

        private async void ButtonLogoutClickHandler(object sender, EventArgs e)
        {
            var accounts = await _appClient.GetAccountsAsync();
            if (accounts.Any())
            {

                await _appClient.RemoveAsync(accounts.FirstOrDefault());
                DisableAccess();
            }
        }

        private void ButtonBrowseClickHandler(object sender, EventArgs e)
        {
            if (OpenFileDialogLocalFile.ShowDialog() == DialogResult.OK)
            {
                TextBoxFileLocation.Text = OpenFileDialogLocalFile.FileName;
            }
        }

        private async void ButtonUploadBlobClickHandler(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TextBoxFileLocation.Text))
            {
                if (!File.Exists(TextBoxFileLocation.Text))
                {
                    ShowError("File not found", "IO Error");
                }
                else
                {
                    string blobName = $"azuread/{Path.GetFileName(TextBoxFileLocation.Text)}";
                    try
                    {
                        CloudBlobContainer container = _blobClient.GetContainerReference("demo");
                        await container.CreateIfNotExistsAsync();
                        CloudBlockBlob blob = container.GetBlockBlobReference(blobName);
                        await blob.UploadFromFileAsync(TextBoxFileLocation.Text);
                        Clipboard.SetText(blob.Uri.ToString());
                        MessageBox.Show($"Copied to clipboard: {blob.Uri.ToString()}", "Blob Upload Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        TextBoxFileLocation.Text = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex.Message, "Error uploading the blob");
                    }
                }
            }

        }

        private void EnableAccess()
        {
            ButtonLogin.Enabled = false;
            ButtonLogout.Enabled =
            ButtonBrowse.Enabled =
            ButtonUploadBlob.Enabled =
            TextBoxFileLocation.Enabled = true;
        }
        private void DisableAccess()
        {
            ButtonLogin.Enabled = true;
            ButtonLogout.Enabled =
            ButtonBrowse.Enabled =
            ButtonUploadBlob.Enabled =
            TextBoxFileLocation.Enabled = false;
        }
        private async Task AuthenticateAsync()
        {
            _appClient = PublicClientApplicationBuilder.Create(_clientId)
                .WithAuthority(_authority)
                .WithRedirectUri(_redirectUri)
                .WithDebugLoggingCallback(Microsoft.Identity.Client.LogLevel.Verbose)
                .Build();
            _appClient.UserTokenCache.SetBeforeAccessAsync(OnBeforeAccessTokenCacheAsync);
            _appClient.UserTokenCache.SetAfterAccessAsync(OnAfterAccessTokenCacheAsync);
            IAccount account = (await _appClient.GetAccountsAsync()).FirstOrDefault();
            try
            {
                _authenticationResult = await _appClient.AcquireTokenSilent(_scopes, account)
                    .ExecuteAsync();
            }
            catch (MsalUiRequiredException ex)
            {
                try
                {
                    _authenticationResult = await _appClient.AcquireTokenInteractive(_scopes)
                        .WithAccount(account)
                        .ExecuteAsync();
                }
                catch (Exception msalUiError)
                {
                    ShowError(msalUiError.Message, "MSAL Error");
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message, "Error");
            }
        }

        private Task OnAfterAccessTokenCacheAsync(TokenCacheNotificationArgs arg)
        {
            if (arg.HasStateChanged)
            {
                lock (_syncLock)
                {
                    byte[] dataToSave = arg.TokenCache.SerializeMsalV3();
                    File.WriteAllBytes(_fileTokenCacheLocation, dataToSave);
                }
            }
            return Task.CompletedTask;
        }

        private Task OnBeforeAccessTokenCacheAsync(TokenCacheNotificationArgs arg)
        {
            byte[] dataToDeserialize = File.Exists(_fileTokenCacheLocation) ?
                File.ReadAllBytes(_fileTokenCacheLocation) : null;

            arg.TokenCache.DeserializeMsalV3(dataToDeserialize);

            return Task.CompletedTask;
        }

        private void ShowError(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
