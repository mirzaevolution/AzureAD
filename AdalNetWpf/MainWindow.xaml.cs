using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace AdalNetWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AuthenticationContext _authenticationContext;
        private AuthenticationResult _authenticationResult;
        private readonly string _authority = "https://login.microsoftonline.com/mscevo.onmicrosoft.com";
        private readonly string _clientId = "a256d239-94d3-4e20-a667-e106e2ee9f98";
        private readonly string _redirectUri = "https://login.microsoftonline.com/common/oauth2/nativeclient";
        private readonly string _resourceUri = "api://core";
        private readonly string _apiResourceUri = "https://localhost:5001";

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += OnFormLoaded;
        }

        private async void OnFormLoaded(object sender, RoutedEventArgs e)
        {
            await AuthenticateAsync();
        }

        public async Task AuthenticateAsync()
        {

            this.Hide();
            _authenticationContext = new AuthenticationContext(_authority, true, new FileCache());
            try
            {
                _authenticationResult =
                    await _authenticationContext.AcquireTokenSilentAsync(
                            _resourceUri,
                            _clientId
                        );
                MessageBox.Show("Successfully authenticated from the cache", "Authenticated", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (AdalException ex)
            {
                if (ex.ErrorCode == AdalError.FailedToAcquireTokenSilently ||
                    ex.ErrorCode == AdalError.InteractionRequired ||
                    ex.ErrorCode == AdalError.UserInteractionRequired)
                {
                    _authenticationResult = await _authenticationContext.AcquireTokenAsync(
                       resource: _resourceUri,
                       clientId: _clientId,
                       redirectUri: new Uri(_redirectUri),
                       parameters: new PlatformParameters(PromptBehavior.Auto));

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            finally
            {
                this.Show();
            }
        }

        private async void CallWebApiClickHandler(object sender, RoutedEventArgs e)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var authHeader = _authenticationResult.CreateAuthorizationHeader();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationResult.AccessToken);
                    HttpResponseMessage httpResponseMessage = await client.GetAsync($"{_apiResourceUri}/api/hello/readmessage");
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        string message = await httpResponseMessage.Content.ReadAsStringAsync();
                        ResultTextBlock.Text = message;
                    }
                    else
                    {
                        MessageBox.Show($"Error: {httpResponseMessage.StatusCode}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
