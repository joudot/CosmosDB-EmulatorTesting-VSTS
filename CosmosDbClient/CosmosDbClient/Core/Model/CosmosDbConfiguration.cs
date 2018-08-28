using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReviewWebsite.Core.Model
{
    public class CosmosDbConfiguration
    {
        public string EndPointUrl { get; set; }

        public string AuthorizationKey { get; set; }

        public string DatabaseName { get; set; }

        public string CollectionName { get; set; }
    }
}
