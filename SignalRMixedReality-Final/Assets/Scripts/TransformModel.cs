using System;
using Newtonsoft.Json;

namespace Assets.Scripts
{
    public class TransformModel : EventArgs, ICloneable
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

        public bool IsSameTransform(TransformModel otherTransformModel, float tolerance)
        {
            if (otherTransformModel == null) return false;

            if (Math.Abs(PositionX - otherTransformModel.PositionX) > tolerance) return false;
            if (Math.Abs(PositionY - otherTransformModel.PositionY) > tolerance) return false;
            if (Math.Abs(PositionZ - otherTransformModel.PositionZ) > tolerance) return false;

            return true;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
