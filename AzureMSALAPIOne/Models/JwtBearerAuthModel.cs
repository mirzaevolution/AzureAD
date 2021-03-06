﻿namespace AzureMSALAPIOne.Models
{
    public class JwtBearerAuthModel
    {
        public string Instance { get; set; }
        public string TenantId { get; set; }
        public string Authority { get; set; }
        public string Audience { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string TargetApiBaseAddress { get; set; }
        public string TargetApiScopes { get; set; }
    }
}
