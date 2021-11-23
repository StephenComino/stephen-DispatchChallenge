namespace DispatchChallenge
{
    public class Message : IMessage
    {
        public string Code { get; private set; }
        public string Payload { get; private set; }

        public Message(string code, string payload)
        {
            this.Code = code; 
            this.Payload = payload;
        }
    }

    public interface IMessage
    {
        string Code { get; }
        string Payload { get; }
    }
}
