using System;

namespace DispatchChallenge
{
    public abstract class DispatcherBase
    {
        public void OnReceivedMessage (object sender, IMessage message)
        {
            // add required logic
            DefaultAction(message);
            Console.WriteLine($"You have reached {sender} - {message.Code} ");
        }

        public virtual void DefaultAction(IMessage message)
        {
            // all unmapped messages should land here
        }
    }
}
