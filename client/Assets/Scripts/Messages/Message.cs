namespace Thubg.Messages
{
    public record Message
    {
        public virtual string MessageType { get; } = "";
    }
}