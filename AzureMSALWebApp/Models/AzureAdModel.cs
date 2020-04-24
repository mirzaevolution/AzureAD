namespace AzureMSALWebApp.Models
{
    public class AzureAdModel
    {
        public string Authority { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scopes { get; set; }
        public string TargetApiBaseAddress { get; set; }
        public string Resource { get; set; }
        public string CallbackPath { get; set; }
    }
}
