using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Cache;
using Microsoft.Identity.Client.Extensibility;
using System.IO;

namespace AzureADMSALWinForm
{
    public partial class FormMain : Form
    {
        private readonly string _authority = "https://login.microsoftonline.com/mscevo.onmicrosoft.com";
        private readonly string _clientId = "a256d239-94d3-4e20-a667-e106e2ee9f98";
        private readonly string _redirectUri = "https://login.microsoftonline.com/common/oauth2/nativeclient";
        private readonly string[] _scopes = { "api://core/Access.Read" };

        private readonly string _apiResourceUri = "https://localhost:44318";
        private IPublicClientApplication _appClient;
        private bool _isLoggedIn = false;
        private AuthenticationResult _authResult = null;
        private static object _synLock = new object();
        private string _fileCacheLocation = "cache.bin";
        public FormMain()
        {
            InitializeComponent();
            this.Load += OnFormLoaded;
        }

        private void OnFormLoaded(object sender, EventArgs e)
        {
            _appClient = PublicClientApplicationBuilder.Create(_clientId)
                .WithRedirectUri(_redirectUri)
                .WithAuthority(_authority)
                .WithDebugLoggingCallback()
                .Build();
            _appClient.UserTokenCache.SetBeforeAccessAsync(BeforeAccessTokenHandler);
            _appClient.UserTokenCache.SetAfterAccessAsync(AfterAccessTokenHandler);
        }

        private Task AfterAccessTokenHandler(TokenCacheNotificationArgs arg)
        {
            if (arg.HasStateChanged)
            {
                lock (_synLock)
                {
                    var data = arg.TokenCache.SerializeMsalV3();
                    File.WriteAllBytes(_fileCacheLocation, data);
                }
            }
            return Task.CompletedTask;

        }

        private Task BeforeAccessTokenHandler(TokenCacheNotificationArgs arg)
        {
            byte[] dataFromStream = File.Exists(_fileCacheLocation) ? File.ReadAllBytes(_fileCacheLocation) : null;
            arg.TokenCache.DeserializeMsalV3(dataFromStream);
            return Task.CompletedTask;
        }

        private async Task Login()
        {

            var account = (await _appClient.GetAccountsAsync()).FirstOrDefault();

            try
            {
                _authResult = await _appClient.AcquireTokenSilent(_scopes, account).ExecuteAsync();

            }
            catch (MsalUiRequiredException ex)
            {
                try
                {
                    _authResult = await _appClient
                        .AcquireTokenInteractive(_scopes)
                        .WithAccount(account)
                        .ExecuteAsync();
                }
                catch (MsalException msalEx)
                {
                    MessageBox.Show(msalEx.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async Task Logout()
        {
            try
            {
                var accounts = await _appClient.GetAccountsAsync();
                if (accounts.Any())
                {
                    await _appClient.RemoveAsync(accounts.FirstOrDefault());

                }
            }
            catch (MsalException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
        private async void ButtonApi1EndpointClickHandler(object sender, EventArgs e)
        {
            if (_authResult != null)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(_apiResourceUri);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authResult.AccessToken);
                        var response = await client.GetAsync("/api/CoreApi/GetMessage");
                        if (response.IsSuccessStatusCode)
                        {
                            string message = await response.Content.ReadAsStringAsync();
                            MessageBox.Show(message, "Response", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {

                            await Login();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private async void ButtonApi2EndpointClickHandler(object sender, EventArgs e)
        {
            if (_authResult != null)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(_apiResourceUri);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authResult.AccessToken);
                        var response = await client.GetAsync("/api/CoreApi/GetMessageFrom2ndApi");
                        if (response.IsSuccessStatusCode)
                        {
                            string message = await response.Content.ReadAsStringAsync();
                            MessageBox.Show(message, "Response", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {

                            await Login();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private async void ButtonLoginClickHandler(object sender, EventArgs e)
        {
            if (!_isLoggedIn)
            {
                await Login();
                this.ButtonLogin.Enabled = false;
                this.ButtonApiEndpoint.Enabled = true;
                this.ButtonApi2Endpoint.Enabled = true;
                this.ButtonLogout.Enabled = true;
                _isLoggedIn = true;
            }
        }

        private async void ButtonLogoutClickHandler(object sender, EventArgs e)
        {
            if (_isLoggedIn)
            {
                await Logout();
                this.ButtonLogin.Enabled = true;
                this.ButtonApiEndpoint.Enabled = false;
                this.ButtonApi2Endpoint.Enabled = false;
                this.ButtonLogout.Enabled = false;
                _isLoggedIn = false;
            }
        }
    }
}

