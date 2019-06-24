using System;
using Newtonsoft.Json;

namespace DerDieDas.Models
{
    public class DeutschWort
    {
        [JsonProperty("Id")]
        public long Id { get; set; }
        [JsonProperty("Artikel")]
        public string Artikel { get; set; }
        [JsonProperty("Wort")]
        public string Wort { get; set; }
        [JsonProperty("Plural")]
        public string Plural { get; set; }
        [JsonProperty("Ubersetzung")]
        public string Ubersetzung { get; set; }
    }
}
