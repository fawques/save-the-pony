using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaveThePony.Models.APIModels
{
    public class APIMaze
    {
        public int[] pony { get; set; }
        public int[] domokun { get; set; }
        [JsonProperty(PropertyName = "end-point")]
        public int[] endPont { get; set; }
        public int[] size { get; set; }
        public int difficulty { get; set; }
        public string[][] data { get; set; }

        public string maze_id { get; set; }
        [JsonProperty(PropertyName = "game-state")]
        public APIGameState GameState { get; set; }
    }
}
