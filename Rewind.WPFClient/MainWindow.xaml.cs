using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace Rewind.WPFClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _authority;
        private string _clientId;
        private string _clientSecret;
        private string _redirectUri;
        private string _resourceUri;
        private string _targetApiBaseUri;
        private AuthenticationResult _authenticationResult;
        private AuthenticationContext _authenticationContext;
        public MainWindow()
        {
            InitializeComponent();
            _authority = ConfigurationManager.AppSettings["ADAuthority"];
            _clientId = ConfigurationManager.AppSettings["ADClientId"];
            _clientSecret = ConfigurationManager.AppSettings["ADClientSecret"];
            _redirectUri = ConfigurationManager.AppSettings["ADRedirectUri"];
            _resourceUri = ConfigurationManager.AppSettings["ADResourceUri"];
            _targetApiBaseUri = ConfigurationManager.AppSettings["TargetAPIBaseUri"];
            _authenticationContext = new AuthenticationContext(_authority, true, new GenericFileTokenCache());

            this.Loaded += MainWindowLoadedHandler;
        }

        private async void MainWindowLoadedHandler(object sender, RoutedEventArgs e)
        {
            await AuthenticateUserAsync();
        }

        private async Task AuthenticateUserAsync(bool hideWindow = false)
        {
            if (hideWindow)
                this.Hide();

            try
            {

                _authenticationResult = await _authenticationContext.AcquireTokenSilentAsync(_resourceUri, _clientId);
                MessageBox.Show("Successfully authenticated from the cache", "Authenticated", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (AdalException ex)
            {
                if (ex.ErrorCode == AdalError.UserInteractionRequired ||
                    ex.ErrorCode == AdalError.FailedToAcquireTokenSilently ||
                    ex.ErrorCode == AdalError.InteractionRequired)
                {
                    await UIAuthentication();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (hideWindow)
                    this.Show();
            }
        }
        private async Task UIAuthentication()
        {
            _authenticationResult = await _authenticationContext.AcquireTokenAsync(_resourceUri, _clientId, new Uri(_redirectUri), new PlatformParameters(PromptBehavior.Always));
        }
        private async void ButtonCallAPIClickHandler(object sender, RoutedEventArgs e)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(_targetApiBaseUri);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationResult.AccessToken);
                    var httpResponse = await httpClient.GetAsync("/api/LayerOne/AccessReadTest");
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        string responseText = await httpResponse.Content.ReadAsStringAsync();
                        MessageBox.Show($"Content: {responseText}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        await UIAuthentication();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private async void ButtonCallAPI2ClickHandler(object sender, RoutedEventArgs e)
        {

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(_targetApiBaseUri);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationResult.AccessToken);
                    var httpResponse = await httpClient.GetAsync("/api/LayerOne/GetLayerTwoMessage");
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        string responseText = await httpResponse.Content.ReadAsStringAsync();
                        MessageBox.Show($"Content: {responseText}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        await UIAuthentication();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
    }
}
