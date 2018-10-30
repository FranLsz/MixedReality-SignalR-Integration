using Newtonsoft.Json;

namespace SignalRService.Models
{
    public class TransformModel
    {
        [JsonProperty("n")]
        public string Name { get; set; }
        [JsonProperty("i")]
        public int Index { get; set; }
        [JsonProperty("px")]
        public float PositionX { get; set; }
        [JsonProperty("py")]
        public float PositionY { get; set; }
        [JsonProperty("pz")]
        public float PositionZ { get; set; }
    }
}