using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureQuest.Web
{
    public class Auth0Settings
    {
        public bool Enabled { get; set; }

        public string Domain { get; set; }

        public string CallbackUrl { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }
    }
}
