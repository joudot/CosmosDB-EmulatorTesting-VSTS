using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReviewWebsite.Core.Model
{
    public class BaseDocument
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}
