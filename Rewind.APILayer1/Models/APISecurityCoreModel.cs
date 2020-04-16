namespace Rewind.APILayer1.Models
{
    public class APISecurityCoreModel
    {
        public JwtBearerOptions JwtBearer { get; set; }
        public TargetApiOptions TargetApi { get; set; }
        public CurrentApiCredsOptions CurrentApiCreds { get; set; }
    }
    public class JwtBearerOptions
    {
        public string Authority { get; set; }
        public string Audience { get; set; }
    }
    public class TargetApiOptions
    {
        public string Authority { get; set; }
        public string Address { get; set; }
        public string Resource { get; set; }
    }
    public class CurrentApiCredsOptions
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
