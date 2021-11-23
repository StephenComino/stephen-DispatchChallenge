using System;

namespace DispatchChallenge
{
    public abstract class DispatcherBase
    {
        public void OnReceivedMessage (object sender, IMessage message)
        {
            // add required logic
            switch (message.Code)
            {
                case String "Init":
                    favoriteTask = "Write code";
                    break;
                case nameof(Manager):
                    favoriteTask = "Create meetings";
                    break;
                default:
                    favoriteTask = "Listen to music";
                    break;
            }
        }

        public virtual void DefaultAction(IMessage message)
        {
            // all unmapped messages should land here
        }
    }
}
