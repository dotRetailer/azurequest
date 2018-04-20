using System;
using System.Collections.Generic;
using System.Text;

namespace AzureQuest.Common.SecureRequest
{
    public class AuthenticationRequestData
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string audience { get; set; }
        public string grant_type { get; set; }
    }
}
