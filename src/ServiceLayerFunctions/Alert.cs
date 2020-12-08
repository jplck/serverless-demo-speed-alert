using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceLayerFunctions
{
    public class Alert
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("alertThreshold")]
        public int AlertThreshold { get; set; }
    }
}
