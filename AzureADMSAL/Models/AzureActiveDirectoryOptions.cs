﻿namespace AzureADMSAL.Models
{
    public class AzureActiveDirectoryOptions
    {
        public string Instance { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Domain { get; set; }
        public string CallbackPath { get; set; }
        public string Resource { get; set; }
    }
}
