using Newtonsoft.Json;

namespace SaveThePony.Models.APIModels
{
    public class APIGameState
    {
        public string state { get; set; }
        [JsonProperty(PropertyName = "state-result")]
        public string stateResult { get; set; }
    }
}