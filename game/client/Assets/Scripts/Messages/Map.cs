using System.Collections.Generic;

namespace Thubg.Sdk
{
    public record Map : Message
    {
        public override string MessageType { get; } = "MAP";
        public List<PositionInt> chunks { get; }
        public Circle poisonousCircle { get; }
        public record Circle
        {
            public Position position { get; }
            public float radius { get; }
        }
    }
}