namespace Rewind.WebApp1.Models
{
    public class AzureAdModel
    {
        public string Authority { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Resource { get; set; }
        public string CallbackPath { get; set; }
    }
    public class TargetApiModel
    {
        public string Address { get; set; }
    }
}
